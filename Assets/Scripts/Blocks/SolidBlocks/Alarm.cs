using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Alarm : SolidBlock
    {
        float voltage;
        static AudioClip[] audioClipArr;
        int selectedSound;
        int outputStack;
        int[] soundOutputMaxStack;
        bool isOutputHightVoltage;
        float outPutVoltage;

        public Alarm(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("alarm", "signal");
            thumbnailColor = new Color(0.886f, 0.886f, 0.886f);
            density = 3.1f;
            transmissivity = 2.85f;
            currentSettingValue = 0;
            selectedSound = currentSettingValue;
            voltage = 0;
            audioClipArr = new AudioClip[3];
            outputStack = 200;
            soundOutputMaxStack = new int[2];
            isOutputHightVoltage = false;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Alarm block = new Alarm(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.steel.getId(), 1 },                  
                     { blocksManager.circuitBoard.getId(), 1 },
                    { blocksManager.semiconductor.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onWorldModeDestroy()
        {
            Pooler.instance.stopSound();
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            selectedSound = currentSettingValue;

            initSound();
        }

        void initSound()
        {
            if (selectedSound == 0 && audioClipArr[0] == null)
            {
                audioClipArr[0] = Resources.Load("sounds/blocks/alarm1", typeof(AudioClip)) as AudioClip;
                soundOutputMaxStack = new int[] { 6, 4 };
            }
            if (selectedSound == 1 && audioClipArr[1] == null)
            {
                audioClipArr[1] = Resources.Load("sounds/blocks/alarm2", typeof(AudioClip)) as AudioClip;
                soundOutputMaxStack = new int[] { 10, 8 };
            }
        }

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();
            selectedSound = currentSettingValue;

            initSound();
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {

            if (voltage >= value || value <= 0)
            {
                return;
            }
            voltage = value;

            outPutVoltage = isOutputHightVoltage ? LogicGate.StandardHeightVoltage : LogicGate.StandardLowVoltage;


            Block up_block = getNeighborBlock(Dir.up);
            Block right_block = getNeighborBlock(Dir.right);
            Block down_block = getNeighborBlock(Dir.down);
            Block left_block = getNeighborBlock(Dir.left);

            putWeMethod(up_block, putter);
            putWeMethod(right_block, putter);
            putWeMethod(down_block, putter);
            putWeMethod(left_block, putter);
        }

        public override float getRomaoteMe()
        {
            return outPutVoltage;
        }

        void putWeMethod(Block block, Block putter)
        {
            if (block.getCoor() != putter.getCoor())
            {
                BlocksEngine.instance.putWe(this, block.getCoor(), outPutVoltage);
            }
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            checkStateRule(blocksEngine);
            voltage = 0;
        }

        protected virtual void checkStateRule(BlocksEngine blocksEngine)
        {
            if (voltage > LogicGate.StandardVoltage)
            {
                Pooler.instance.playSound(audioClipArr[selectedSound], true);
                outputStack++;
                if (outputStack > soundOutputMaxStack[0])
                {
                    isOutputHightVoltage = true;
                    setSpriteRect(2);
                    outputStack = 0;
                }
                else if (outputStack > soundOutputMaxStack[1])
                {
                    isOutputHightVoltage = false;
                    setSpriteRect(1);
                }
            }
            else
            {
                setSpriteRect(0);
                Pooler.instance.stopSound();
                outputStack = 200;
                isOutputHightVoltage = false;
            }
        }

        public override int isWeSystem()
        {
            return 1;
        }

        public override int isCanSettingValue()
        {
            return 0;
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 1 };
        }

        public override string getSettingValueName()
        {
            return "sound type";
        }      
    }

}