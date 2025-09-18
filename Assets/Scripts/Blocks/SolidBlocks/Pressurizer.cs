using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Pressurizer : RotationBlock
    {

        protected bool isWork;
        protected float comsume;
        protected float addPress;
        protected bool isFollowSettingValue;

        public Pressurizer(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("pressurizer", "null");
            thumbnailColor = new Color(0.717f, 0.792f, 0.929f);
            density = 15.1f;
            isWork = false;
            addPress = currentSettingValue;
            comsume = 0.06f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Pressurizer block = new Pressurizer(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            block.initRotationBlock();
            return block;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            addPress = currentSettingValue;
            isFollowSettingValue = !(addPress == 0);
        }

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();
            addPress = currentSettingValue;
            isFollowSettingValue = !(addPress == 0);
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
            //pressurizerRule(blocksEngine); 废弃的
        }

        protected virtual void pressurizerRule(BlocksEngine blocksEngine)
        {
            if (isWork)
            {
                float receive = Pooler.instance.requireElectric(this, Mathf.Abs(comsume * addPress));
                if (receive > 0)
                {
                    Block upBlock = getRelativeNeighborBlock(Dir.up);
                    Block downBlock = getRelativeNeighborBlock(Dir.down);

                    if (upBlock.getPress() < addPress)
                    {
                        upBlock.setPress(addPress);
                    }

                    if (downBlock.getPress() < addPress)
                    {
                        downBlock.setPress(addPress);
                    }

                }
            }
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            if (isFollowSettingValue)
            {
                isWork = value > LogicGate.StandardVoltage;
            }
            else
            {
                addPress = value;
                isWork = true;
            }

        }

        public override int isCanSettingValue()
        {
            return 2;
        }

        public override string getSettingValueName()
        {
            return "press";
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

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 50000 };
        }
    }
}
