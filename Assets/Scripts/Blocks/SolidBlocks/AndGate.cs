using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AndGate : RotationBlock
    {

        protected float voltage;
        protected float voltage1;
        protected float voltage2;
        int inputStack;

        public AndGate(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("andGate", "logical", true);

            density = 2.9f;
            transmissivity = 7.32f;

            voltage = 0;
            voltage1 = 0;
            voltage2 = 0;
            inputStack = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AndGate block = new AndGate(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.semiconductor.getId(), 3 },
                    { blocksManager.silicon.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 2);

            return syntInfos;
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            if (getRelativeDir(Dir.up) == putterDir)
            {
                voltage1 = value;
            }
            if (getRelativeDir(Dir.down) == putterDir)
            {
                voltage2 = value;
            }
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            logicGateRule(blocksEngine);
            if (inputStack > 1)
            {
                voltage1 = 0;
                voltage2 = 0;
                inputStack = 0;
            }
            inputStack++;

        }

        protected virtual void logicGateRule(BlocksEngine blocksEngine)
        {

            voltage = ((voltage1 > LogicGate.StandardVoltage) && (voltage2 > LogicGate.StandardVoltage)) ? LogicGate.StandardHeightVoltage : LogicGate.StandardLowVoltage;
            blocksEngine.putWe(this, getRelativeNeighborBlock(Dir.right).getCoor(), voltage);
        }

        public override int isWeSystem()
        {
            return 1;
        }
    }
}