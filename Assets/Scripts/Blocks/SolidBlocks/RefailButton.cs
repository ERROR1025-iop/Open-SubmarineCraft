using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class RefailButton : SolidBlock
    {

        bool isTurnOn;
        bool isStateChanged;
        float voltage;
        int refailStack;

        static AudioClip audioClip;

        public RefailButton(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("refailButton", "signal");
            isCanChangeRedAndCrackTexture = false;
            density = 3.1f;
            transmissivity = 2.85f;
            refailStack = 0;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            audioClip = Resources.Load("sounds/blocks/button", typeof(AudioClip)) as AudioClip;
        }


        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            RefailButton block = new RefailButton(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.switchGate.getId(), 1},
                    { blocksManager.steel.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onPreesButtonClick(bool isClick)
        {
            isTurnOn = true;
            isStateChanged = true;
            Pooler.instance.playSound(audioClip);
            refailStack = 0;
        }

        public override void onWorldModeClick()
        {
            isTurnOn = true;
            isStateChanged = true;
            Pooler.instance.playSound(audioClip);
            refailStack = 0;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            buttonRule(blocksEngine);
            checkStateRule(blocksEngine);
            if (isTurnOn)
            {
                if (refailStack > 1)
                {
                    isTurnOn = false;
                }
                else
                {
                    refailStack++;
                }
            }
        }

        void buttonRule(BlocksEngine blocksEngine)
        {
            voltage = isTurnOn ? LogicGate.StandardHeightVoltage : LogicGate.StandardLowVoltage;
            Block up_block = getNeighborBlock(Dir.up);
            Block right_block = getNeighborBlock(Dir.right);
            Block down_block = getNeighborBlock(Dir.down);
            Block left_block = getNeighborBlock(Dir.left);

            blocksEngine.putWe(this, up_block.getCoor(), voltage);
            blocksEngine.putWe(this, right_block.getCoor(), voltage);
            blocksEngine.putWe(this, down_block.getCoor(), voltage);
            blocksEngine.putWe(this, left_block.getCoor(), voltage);
            isStateChanged = true;
        }

        public override float getRomaoteMe()
        {
            return voltage;
        }

        protected void checkStateRule(BlocksEngine blocksEngine)
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
            return new int[4] { 1, 2, 4, 5 };
        }
    }
}
