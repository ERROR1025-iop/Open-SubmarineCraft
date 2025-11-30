using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace{ public class StirlingEngine : LargeBlock
    {

        protected int powerDirection;

        IPoint startOffset;
        int startSprite;

        int updataFrameDelayPerUnit;
        int updataFrameDelayStack;
        int updataFrameStack;        
        protected float lastOutput;

        public StirlingEngine(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("stirlingEngine", "engine");

            thumbnailColor = new Color(0.2686f, 0.2686f, 0.2686f);
            density = 12.3f;

            powerDirection = 1;

            updataFrameStack = 0;
            updataFrameDelayPerUnit = 30000;
            updataFrameDelayStack = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            StirlingEngine block = new StirlingEngine(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            block.initLargeBlock(new IPoint(3, 3));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.fineSteel.getId(), 2 },
                    { blocksManager.steel.getId(), 9 },
                    { blocksManager.shaft.getId(), 3 }                    
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 9);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 36;
        }

        protected override void onLargeBlockInitFinish()
        {
            startOffset = getOffset();
            startSprite = ((size.y - 1) - startOffset.y) * size.x + startOffset.x;
        }

        public override bool frameUpdate()
        {
            if (updataFrameDelayStack < updataFrameDelayPerUnit)
            {
                updataFrameDelayStack++;
                return true;
            }
            updataFrameStack = updataFrameStack > 3 ? 0 : updataFrameStack;
            if (getOffset().y == 1)
            {
                setSpriteRect(startSprite + updataFrameStack * 9);
            }

            updataFrameStack++;
            updataFrameDelayStack = 0;
            return true;
        }

        public void setUpdataFrameDelayPerUnit(int unit)
        {
            updataFrameDelayPerUnit = unit;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            if (isOrigin())
            {
                stirlingRule(blocksEngine);
            }

            if (getIsBroken())
            {
                blocksEngine.putMe(this, getOriginPoint() + new IPoint(3, 1), 0);
                return;
            }
        }

        void stirlingRule(BlocksEngine blocksEngine)
        {
            Block block1 = blocksEngine.getBlock(getReviseBlockCoor(new IPoint(1, 0)));
            Block block2 = blocksEngine.getBlock(getReviseBlockCoor(new IPoint(1, 2)));
            // 缓存温度，避免重复访问属性
            float temp1 = block1.temperature;
            float temp2 = block2.temperature;
            // 一次性确定高温块和低温块
            Block highTempBlock = temp1 > temp2 ? block1 : block2;
            Block lowTempBlock  = temp1 <= temp2 ? block1 : block2;
            // 计算平均温度
            float highToLowRatio = 0.8f;
            float tarTemp = highTempBlock.temperature - 
                   (highTempBlock.temperature - lowTempBlock.temperature) * (1-highToLowRatio);
            // 单位热容量（每单位体积或质量的热容）
            float uhq = density * heatCapacity;
            // 可用热能（高温部分向平均温度传递时可释放的能量）
            float hq = (highTempBlock.temperature - tarTemp) * uhq * 0.20f;
            // 最终输出
            float output = hq * powerDirection * getEfficiency();
            highTempBlock.setTemperature(tarTemp);

            float realOutPut = Mathf.Abs(Mathf.Lerp(lastOutput, output, 0.01f));
            blocksEngine.putMe(this, getReviseBlockCoor(new IPoint(3, 1)), realOutPut);
            lastOutput = realOutPut;

            for (int offsetx = 0; offsetx < size.x; offsetx++)
            {
                for (int offsety = 0; offsety < size.y; offsety++)
                {
                    IPoint obOffset = new IPoint(offsetx, offsety);
                    StirlingEngine offsetBlock = blocksEngine.getBlock(getOriginPoint() + obOffset) as StirlingEngine;
                    if (offsetBlock == null || offsetBlock.equalBlock(this) == false)
                    {
                        continue;
                    }
                    if (realOutPut > 5000)
                    {
                        offsetBlock.setUpdataFrameDelayPerUnit(0);
                    }
                    else if (realOutPut > 0)
                    {
                        offsetBlock.setUpdataFrameDelayPerUnit((int)(5000 / realOutPut));
                    }
                    else
                    {
                        offsetBlock.setUpdataFrameDelayPerUnit(30000);
                    }
                }
            }
        }

        public override Color getThumbnailColor(JsonData blockData)
        {
            if (IUtils.getJsonValue2Bool(blockData, "ib") == false)
            {
                IPoint toffset = new IPoint(IUtils.getJsonValue2Int(blockData, "ox"), IUtils.getJsonValue2Int(blockData, "oy"));
                if (toffset.x == 1 && toffset.y == 1)
                {
                    return new Color(0.5686f, 0.5686f, 0.5686f);
                }
                if (toffset.equal(new IPoint(1, 0)))
                {
                    return new Color(1f, 0f, 0f);
                }
                if (toffset.equal(new IPoint(1, 2)))
                {
                    return new Color(0f, 0f, 1f);
                }
            }
            return base.getThumbnailColor(blockData);
        }
    }
}
