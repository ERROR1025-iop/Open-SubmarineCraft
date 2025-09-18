using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class LogicGate : RotationBlock
    {
        public static float StandardVoltage = 500;
        public static float StandardHeightVoltage = 1001f;
        public static float StandardLowVoltage = 301f;

        float voltage;
        float voltage1;
        int inputStack;

        public LogicGate(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("logicGate", "logical", true);

            density = 2.9f;
            transmissivity = 7.32f;

            voltage = 0;
            voltage1 = 0;
            inputStack = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            LogicGate block = new LogicGate(blockId, parentObject, blockObject);
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
            if (getRelativeDir(Dir.left) == putterDir)
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
            voltage = (voltage1 > LogicGate.StandardVoltage) ? LogicGate.StandardLowVoltage : LogicGate.StandardHeightVoltage;
            blocksEngine.putWe(this, getRelativeNeighborBlock(Dir.right).getCoor(), voltage);
        }

        public override int isWeSystem()
        {
            return 1;
        }
    }
}
