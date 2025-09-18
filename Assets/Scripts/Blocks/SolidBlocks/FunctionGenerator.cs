using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class FunctionGenerator : RotationBlock
    {


        protected bool isWork;
        protected float outputValue;


        public FunctionGenerator(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("functionGenerator", "logical");
            isCanChangeRedAndCrackTexture = false;
            density = 15.1f;

            outputValue = currentSettingValue;
            isWork = true;

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            FunctionGenerator block = new FunctionGenerator(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            block.initRotationBlock();
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.circuitBoard.getId(), 1 },
                    { blocksManager.silicon.getId(), 2 },
                    { blocksManager.steel.getId(), 1 },
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            outputValue = currentSettingValue + 0.9f;
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

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();
            outputValue = currentSettingValue + 0.9f;
        }     


        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            functionGeneratorRule(blocksEngine);
        }

        protected virtual void functionGeneratorRule(BlocksEngine blocksEngine)
        {
            if (isWork)
            {
                blocksEngine.putWe(this, getRelativeDirPoint(Dir.right), outputValue);
                setSpriteRect(1);
            }
            else
            {
                setSpriteRect(0);
            }
        }

        public override float getRomaoteMe()
        {
            return outputValue;
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            if (getRelativeDir(Dir.right) != putterDir)
            {
                isWork = value > LogicGate.StandardVoltage;
            }
        }

        public override int isCanSettingValue()
        {
            return 2;
        }

        public override string getSettingValueName()
        {
            return "output value";
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
            return new int[2] { 4, 5 };
        }
    }
}
