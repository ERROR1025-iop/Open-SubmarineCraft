using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class SteamTurbine : LargeBlock
    {
        private float lastOutput;
        private float output;

        public SteamTurbine(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("steamTurbine", "engine", true);

            thumbnailColor = new Color(0f, 0.294f, 0.682f);
            transmissivity = 3.2f;
            density = 15.7f;

            lastOutput = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SteamTurbine block = new SteamTurbine(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            block.initLargeBlock(new IPoint(6, 3));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.fineSteel.getId(), 18 },
                    { blocksManager.gearSet.getId(), 9 },
                    { blocksManager.shaft.getId(), 6 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 18);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 18;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            if (isOrigin())
            {
                steamTurbineRule(blocksEngine);
            }

            if (getIsBroken())
            {
                blocksEngine.putMe(this, getOriginPoint() + new IPoint(3, 1), 0);
                return;
            }
        }

        void steamTurbineRule(BlocksEngine blocksEngine)
        {
            Block inBlock = blocksEngine.getBlock(getReviseBlockCoor(new IPoint(1, -1)));
            Block outBlock = blocksEngine.getBlock(getReviseBlockCoor(new IPoint(2, 3)));

            output = 0;
            if (outBlock.isAir())
            {
                if (!inBlock.isAir() && inBlock.equalPState(PState.gas))
                {
                    float dt = inBlock.getTemperature() - 25;
                    float dp = inBlock.getPress() - 100;                  
                    inBlock.setTemperature(inBlock.getTemperature() - dt);
                    inBlock.setPress(100);
                    inBlock.moveTo(outBlock.getCoor());
                    float outputT = inBlock.heatCapacity * inBlock.density * dt;
                    float outputP = Mathf.Abs(dp - 100) * 10;
                    output = outputT + outputP;
                    Debug.Log("outputT:" + outputT + ",outputP:" + outputP  + ",output:" + output);
                }
                else if (inBlock.equalPState(PState.liquild) || inBlock.equalPState(PState.mushy))
                {
                    inBlock.moveTo(outBlock.getCoor());
                }
            }

            float realOutPut = Mathf.Abs(Mathf.Lerp(lastOutput, output, 0.01f));
            //Debug.Log("output:" + output + ",realOutPut:" + realOutPut); 
            blocksEngine.putMe(this, getReviseBlockCoor(new IPoint(6, 1)), realOutPut * getEfficiency());
            lastOutput = realOutPut;

        }

        public override Color getThumbnailColor(JsonData blockData)
        {
            if (IUtils.getJsonValue2Bool(blockData, "ib") == false)
            {
                IPoint toffset = new IPoint(IUtils.getJsonValue2Int(blockData, "ox"), IUtils.getJsonValue2Int(blockData, "oy"));
                if (toffset.y == 2)
                {
                    return new Color(0.647f, 0.647f, 0.647f);
                }
                if (toffset.equal(new IPoint(1, 0)))
                {
                    return new Color(1f, 1f, 0f);
                }
                if (toffset.equal(new IPoint(2, 5)) || toffset.equal(new IPoint(2, 6)))
                {
                    return new Color(0f, 0f, 0f, 0f);
                }
            }
            return base.getThumbnailColor(blockData);
        }
    }
}
