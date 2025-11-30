using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class Valve : RotationBlock
    {

        bool isTurnOn;
        bool isStateChanged;

        public Valve(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("valve", "machine");
            isCanChangeRedAndCrackTexture = false;
            thumbnailColor = new Color(0.498f, 0.498f, 0.498f);
            density = 7.85f;
            transmissivity = 7.2f;

            isTurnOn = false;
            isStateChanged = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Valve block = new Valve(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.steel.getId(), 2 },
                    { blocksManager.gearSet.getId(), 1 },

            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 2);

            return syntInfos;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            isTurnOn = getCurrentBindId() == 6;
            isStateChanged = true;
        }

        public override void onPreesButtonClick(bool isClick)
        {
            if (isTurnOn != isClick)
            {
                isStateChanged = true;
            }
            isTurnOn = isClick;
        }

        public override void onWorldModeClick()
        {
            base.onWorldModeClick();
            isTurnOn = !isTurnOn;
            isStateChanged = true;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            valveRule(blocksEngine);
            checkStateRule();
        }

        void valveRule(BlocksEngine blocksEngine)
        {
            if (isTurnOn == false)
                return;
            if (dir == Dir.up || dir == Dir.down)
            {
                Block upBlock = getNeighborBlock(Dir.up);
                Block downBlock = getNeighborBlock(Dir.down);
                if (upBlock.equalPState(PState.liquild) || upBlock.equalPState(PState.mushy))
                {
                    if (downBlock.isAir())
                    {
                        upBlock.moveTo(downBlock.getCoor());
                    }
                }

                if (downBlock.equalPState(PState.gas) && !downBlock.isAir())
                {
                    if (upBlock.isAir())
                    {
                        downBlock.moveTo(upBlock.getCoor());
                    }
                }
            }
            else
            {
                Block leftBlock = getNeighborBlock(Dir.left);
                Block rightBlock = getNeighborBlock(Dir.right);

                if (leftBlock.isFluid())
                {
                    if (rightBlock.isAir())
                    {
                        leftBlock.moveTo(rightBlock.getCoor());
                    }
                }
                else if (rightBlock.isFluid())
                {
                    if (leftBlock.isAir())
                    {
                        rightBlock.moveTo(leftBlock.getCoor());
                    }
                }
            }
        }

        protected void checkStateRule()
        {
            if (isStateChanged)
            {
                if (isTurnOn)
                {
                    setSpriteRect(1);
                }
                else
                {
                    setSpriteRect(0);
                }

                isStateChanged = false;
            }
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            isStateChanged = true;
            isTurnOn = value > LogicGate.StandardVoltage;
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
    }
}
