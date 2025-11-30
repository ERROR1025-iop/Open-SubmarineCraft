using System.Collections;
using System.Collections.Generic;
using Scraft.StationSpace;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class ItemDepositor : RotationBlock
    {
        bool isWork;
        bool isRun;
        protected float comsume;
        float nt;

        public ItemDepositor(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("itemDepositor", "industry");
            density = 5.4f;
            transmissivity = 2.85f;
            currentSettingValue = 0;
            isWork = false;
            isRun = false;
            comsume = 1.0f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            ItemDepositor block = new ItemDepositor(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.fineSteel.getId(), 2 },
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.smallElectorEngine.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

                public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            isWork = getCurrentBindId() == 6;
        }


        public override void onWorldModeClick()
        {
            base.onWorldModeClick();
            isWork = !isWork;
        }

        public override void onPreesButtonClick(bool isClick)
        {
            isWork = isClick;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            depositRule(blocksEngine);
        }

        void depositRule(BlocksEngine blocksEngine)
        {
            if (isWork)
            {
                float receive = Pooler.instance.requireElectric(this, comsume);
                if (receive > comsume * 0.9f)
                {
                    transferMethod(blocksEngine);
                    isRun = true;
                }
                else
                {
                    isRun = false;
                }
            }
            else
            {
                isRun = false;
            }
        }

        protected void transferMethod(BlocksEngine blocksEngine)
        {
            if (Station.isSatyInStation && Station.satyStation != null)
            {
                Station station = Station.satyStation;
                Block block = getRelativeNeighborBlock(Dir.up);
                if (block.isCanStoreInWarehouse() > 0)
                {
                    if (station.isCanAddCargos(block.getId(), 1) > 0)
                    {
                        blocksEngine.removeBlock(block.getCoor());
                        station.addCargos(block.getId(), 1);
                    }
                }
            }
        }

        public override void onReciverWe(float voltage, int putterDir, Block putter)
        {
            base.onReciverWe(voltage, putterDir, putter);
            isWork = voltage > LogicGate.StandardVoltage;
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
            return new int[3] { 4, 5, 6 };
        }

        public override bool isCargohold()
        {
            return true;
        }
    }
}

