using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scraft.BlockSpace
{
    public class Selector : RotationBlock
    {
        bool isWork;
        bool isRun;
        int targetDir;
        protected float comsume;        


        public Selector(int id, GameObject parentObject, GameObject blockObject)
               : base(id, parentObject, blockObject)
        {
            initBlock("selector", "industry");
            transmissivity = 3.2f;
            density = 10.3f;
            comsume = 0.2f;
            isWork = false;
            isRun = false;
            isCanChangeRedAndCrackTexture = false;

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Selector block = new Selector(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.transfer.getId(), 1 },
                    { blocksManager.advGoodsSensor.getId(), 1 }
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


        public override void onBuilderModeCreated()
        {
            base.onBuilderModeCreated();
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

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            transferRule();
            setSpriteRect((targetDir == 1 ? 0 : 2) + (isRun ? 1 : 0));
        }       

        protected void transferRule()
        {
            if (isWork)
            {
                float receive = Pooler.instance.requireElectric(this, comsume);
                if (receive > comsume * 0.9f)
                {
                    transferMethod();
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

        protected void transferMethod()
        {
            Block block = getRelativeNeighborBlock(Dir.left);
            if (block != null && !block.isAir())
            {
                if (block.equalBlock(getRelativeNeighborBlock(targetDir == 1 ? 0 : 2)))
                {
                    IPoint toCoor = getRelativeDirPoint(Dir.right);
                    if (BlocksEngine.instance.getBlock(toCoor).isAir())
                    {
                        block.moveTo(toCoor);
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

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();
            setTargetDir(currentSettingValue + 1);
        }

        void setTargetDir(int dir)
        {
            targetDir = dir;
            setSpriteRect((targetDir == 1 ? 0 : 2) + (isRun ? 1 : 0));
        }

        public override int isCanSettingValue()
        {
            return 0;
        }

        public override string getSettingValueName()
        {
            return "detect direction";
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 1 };
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