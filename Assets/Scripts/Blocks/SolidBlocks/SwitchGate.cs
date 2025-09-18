using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SwitchGate : RotationBlock
    {
        protected bool isWork;
        protected float leftValue;
        protected float rightValue;

        public SwitchGate(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("switchGate", "logical");
            isCanChangeRedAndCrackTexture = false;
            density = 15.1f;
            
            isWork = false;

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SwitchGate block = new SwitchGate(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            block.initRotationBlock();
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.cable.getId(), 2 },
                    { blocksManager.steel.getId(), 1 }                   
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 2);

            return syntInfos;
        }

        public override void onPreesButtonClick(bool isClick)
        {
            if (isClick)
            {
                isWork = !isWork;
            }
        }

        public override void onWorldModeClick()
        {
            base.onWorldModeClick();
            isWork = !isWork;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            isWork = getCurrentBindId() == 6;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            switchGateRule(blocksEngine);
        }

        protected virtual void switchGateRule(BlocksEngine blocksEngine)
        {
            if (isWork)
            {
                blocksEngine.putWe(this, getRelativeDirPoint(Dir.right), leftValue - Cable.Voltage_Drop);
                blocksEngine.putWe(this, getRelativeDirPoint(Dir.left), rightValue - Cable.Voltage_Drop);
                setSpriteRect(1);
            }
            else
            {
                setSpriteRect(0);
            }
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            if (getRelativeDir(Dir.left) == putterDir)
            {
                leftValue = value;
            }
            else if(getRelativeDir(Dir.right) == putterDir)
            {
                rightValue = value;
            }
            else
            {
                isWork = value > LogicGate.StandardVoltage;
            }
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
    }
}

