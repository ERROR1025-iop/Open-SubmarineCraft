using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class OrGate : AndGate
    {

        public OrGate(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("orGate", "logical", true);

            density = 2.9f;
            transmissivity = 7.32f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            OrGate block = new OrGate(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            return block;
        }

        protected override void logicGateRule(BlocksEngine blocksEngine)
        {
            voltage = ((voltage1 > LogicGate.StandardVoltage) || (voltage2 > LogicGate.StandardVoltage)) ? LogicGate.StandardHeightVoltage : LogicGate.StandardLowVoltage;
            blocksEngine.putWe(this, getRelativeNeighborBlock(Dir.right).getCoor(), voltage);
        }
    }
}
