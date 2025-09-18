using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class SidePropeller : LargeBlock
    {

        IPoint startOffset;
        int startSprite;

        int updataFrameDelayPerUnit;
        int updataFrameDelayStack;
        int updataFrameStack;
        float getMe;


        public SidePropeller(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sidePropeller", "machine");

            thumbnailColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            transmissivity = 7.2f;
            density = 3.5f;
            updataFrameStack = 0;
            updataFrameDelayPerUnit = 30000;
            updataFrameDelayStack = 0;
            getMe = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SidePropeller block = new SidePropeller(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            block.initLargeBlock(new IPoint(3, 3));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.shaft.getId(), 1},               
                    { blocksManager.steel.getId(), 9 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 9);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 18;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            MainSubmarine.sideForceBlocks.Add(this);
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            MainSubmarine.sideForceBlocks.Remove(this);
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
            updataFrameStack = updataFrameStack > 1 ? 0 : updataFrameStack;
            setSpriteRect(startSprite + updataFrameStack * 9);

            updataFrameStack++;
            updataFrameDelayStack = 0;
            return true;
        }

        public override void onReciverMe(float me, int putterDir, Block putter)
        {
            getMe = me * getBlockCount();

            float absMe = Mathf.Abs(me);

            for (int offsetx = 0; offsetx < size.x; offsetx++)
            {
                for (int offsety = 0; offsety < size.y; offsety++)
                {
                    IPoint obOffset = new IPoint(offsetx, offsety);
                    SidePropeller offsetBlock = BlocksEngine.instance.getBlock(getOriginPoint() + obOffset) as SidePropeller;
                    if (offsetBlock == null || offsetBlock.equalBlock(this) == false)
                    {
                        continue;
                    }
                    if (absMe > 300)
                    {
                        offsetBlock.setUpdataFrameDelayPerUnit(0);
                    }
                    else if (absMe > 0)
                    {
                        offsetBlock.setUpdataFrameDelayPerUnit((int)(300 / absMe));
                    }
                    else
                    {
                        offsetBlock.setUpdataFrameDelayPerUnit(30000);
                    }
                }
            }
        }

        public void setUpdataFrameDelayPerUnit(int unit)
        {
            updataFrameDelayPerUnit = unit;
        }

        public override float getSideForce()
        {
            return getMe * 0.01f;
        }

        public override Color getThumbnailColor(JsonData blockData)
        {
            if (IUtils.getJsonValue2Bool(blockData, "ib") == false)
            {
                IPoint toffset = new IPoint(IUtils.getJsonValue2Int(blockData, "ox"), IUtils.getJsonValue2Int(blockData, "oy"));
                if (toffset.x == 1 || toffset.y == 1)
                {
                    return new Color(0.4286f, 0.4286f, 0.4286f);
                }
            }

            return base.getThumbnailColor(blockData);
        }

        public override bool isCanReceiveMe()
        {
            return true;
        }
    }
}
