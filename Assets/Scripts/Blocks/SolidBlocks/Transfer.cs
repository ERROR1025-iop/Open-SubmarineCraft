using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scraft.BlockSpace
{
    public class Transfer : RotationBlock
    {
        bool isWork;
        bool isRun;
        protected float comsume;


        public Transfer(int id, GameObject parentObject, GameObject blockObject)
               : base(id, parentObject, blockObject)
        {
            initBlock("transfer", "industry");
            transmissivity = 3.2f;
            density = 10.3f;
            comsume = 0.2f;
            isWork = false;
            isRun = false;
            isCanChangeRedAndCrackTexture = false;
          
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Transfer block = new Transfer(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
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
            transferRule();
            setSpriteRect(isRun ? 1 : 0);
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
                IPoint toCoor = getRelativeDirPoint(Dir.right);
                if (BlocksEngine.instance.getBlock(toCoor).isAir())
                {
                    block.moveTo(toCoor);
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