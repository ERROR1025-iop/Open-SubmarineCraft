using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class ComparatorGate : RotationBlock
    {

        float voltage;
        float voltage1;
        int inputStack;
        float comparatorValue;

        public ComparatorGate(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("comparatorGate", "logical", true);
            isCanChangeRedAndCrackTexture = false;
            density = 2.9f;
            transmissivity = 7.32f;

            voltage = 0;
            voltage1 = 0;
            inputStack = 0;
            comparatorValue = currentSettingValue;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            ComparatorGate block = new ComparatorGate(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
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

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            if (getRelativeDir(Dir.right) != putterDir)
            {
                voltage1 = value;
            }
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            checkStateRule(blocksEngine);
            if (inputStack > 1)
            {
                voltage1 = 0;
                inputStack = 0;
            }
            inputStack++;
        }

        protected void checkStateRule(BlocksEngine blocksEngine)
        {
            comparatorValue = currentSettingValue;
            voltage = (voltage1 > comparatorValue) ? LogicGate.StandardHeightVoltage : LogicGate.StandardLowVoltage;
            blocksEngine.putWe(this, getRelativeNeighborBlock(Dir.right).getCoor(), voltage);
            setSpriteRect(voltage > LogicGate.StandardVoltage ? 1 : 0);
        }

        public override int isWeSystem()
        {
            return 1;
        }

        public override int isCanSettingValue()
        {
            return 2;
        }

        public override string getSettingValueName()
        {
            return "comparate value";
        }
    }
}