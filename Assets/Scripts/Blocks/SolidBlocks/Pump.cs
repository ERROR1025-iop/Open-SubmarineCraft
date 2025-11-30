using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class Pump : RotationBlock
    {

        protected bool isWork;
        protected float comsume;
        protected float dpressComsume;

        public Pump(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("pump", "machine");
            density = 11.1f;
            isCanChangeRedAndCrackTexture = false;
            isWork = false;
            dpressComsume = 0.02f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Pump block = new Pump(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            block.initRotationBlock();
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.smallElectorEngine.getId(), 1 },
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.shaft.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 2);

            return syntInfos;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            if (getCurrentBindId() == 6)
            {
                isWork = true;
            }
        }

        public override void onPreesButtonClick(bool isClick)
        {
            isWork = isClick;
        }

        public override void onWorldModeClick()
        {
            base.onWorldModeClick();
            isWork = !isWork;

        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            pumpRule(blocksEngine);
        }

        protected virtual void pumpRule(BlocksEngine blocksEngine)
        {
            if (isWork)
            {
                Block inBlock = getRelativeNeighborBlock(Dir.down);
                Block outBlock = getRelativeNeighborBlock(Dir.up);
                float dpress = outBlock.getPress() - inBlock.getPress();
                comsume = dpress > 500 ? dpress * dpressComsume : 0;

                float receive = Pooler.instance.requireElectric(this, comsume);
                if (comsume == 0 || receive > comsume * 0.9f)
                {
                    Block air = blocksEngine.getBlocksManager().air;

                    if (inBlock.isFluid())
                    {
                        if (outBlock.equalBlock(air))
                        {
                            inBlock.moveTo(outBlock.getCoor());
                        }
                        else if (outBlock.isFluid())
                        {
                            pumpCompressMethod(blocksEngine, inBlock, outBlock);
                        }
                    }      
                    setSpriteRect(1);
                }
                else
                {
                    setSpriteRect(0);
                }
            }
            else
            {
                setSpriteRect(0);
            }
        }

        void pumpCompressMethod(BlocksEngine blocksEngine, Block inBlock, Block outBlock)
        {

            Block air = blocksEngine.getBlocksManager().air;

            if (outBlock.equalPState(PState.liquild))
            {
                if (((LiquidBlock)outBlock).addCompressChild(inBlock))
                {
                    blocksEngine.createBlockBase(inBlock.getCoor(), air, false); 
                }
            }
            else if (outBlock.equalPState(PState.gas))
            {
                if (((GasBlock)outBlock).addCompressChild(inBlock))
                {
                    blocksEngine.createBlockBase(inBlock.getCoor(), air, false); 
                }
            }
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);
            IUtils.keyValue2Writer(writer, "isWork", isWork);
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            isWork = IUtils.getJsonValue2Bool(blockData, "isWork");

        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            isWork = value > LogicGate.StandardVoltage;
        }

        public override int isWeSystem()
        {
            return 1;
        }

        public override bool isCanBind()
        {
            return true;
        }

        public override int[] getBindArr()
        {
            return new int[5] { 1, 2, 4, 5, 6 };
        }

        public override bool isCargohold()
        {
            return true;
        }
    }
}
