using System.Collections;
using System.Collections.Generic;
using Scraft.StationSpace;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class ItemExtractor : RotationBlock
    {
        bool isWork;
        bool isRun;
        protected float comsume;
        int targetDir;

        public ItemExtractor(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("itemExtractor", "industry");
            density = 5.4f;
            transmissivity = 2.85f;
            currentSettingValue = 0;
            isWork = false;
            isRun = false;
            comsume = 1.0f;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            ItemExtractor block = new ItemExtractor(blockId, parentObject, blockObject);
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
            setTargetDir(currentSettingValue + 1);
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


        public override void onBuilderModeCreated()
        {
            base.onBuilderModeCreated();
            setTargetDir(currentSettingValue + 1);
        }

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();
            setTargetDir(currentSettingValue + 1);
        }

        void setTargetDir(int dir)
        {
            targetDir = dir;
            setSpriteRect(targetDir - 1);
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            extractRule(blocksEngine);
        }

        void extractRule(BlocksEngine blocksEngine)
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
                if (getRelativeNeighborBlock(Dir.up).isAir())
                {
                    Station station = Station.satyStation;
                    Block targetBlock = getRelativeNeighborBlock(targetDir);
                    if (targetBlock.isCanStoreInWarehouse() > 0)
                    {
                        if (station.IsContainCargo(targetBlock.getId(), 1))
                        {
                            station.removeCargos(targetBlock.getId(), 1);
                            blocksEngine.createBlockBase(getRelativeDirPoint(Dir.up), targetBlock, false);
                        }
                    }
                }
            }
        }

        public override int isCanSettingValue()
        {
            return 0;
        }

        public override string getSettingValueName()
        {
            return "direction";
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 2 };
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

