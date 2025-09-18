using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AddGate : AndGate
    {

        public AddGate(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("addGate", "logical", true);

            density = 2.9f;
            transmissivity = 7.32f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AddGate block = new AddGate(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            return block;
        }  

        protected override void logicGateRule(BlocksEngine blocksEngine)
        {
            voltage = (int)(voltage1) + (int)(voltage2) + 0.99f;
            blocksEngine.putWe(this, getRelativeNeighborBlock(Dir.right).getCoor(), voltage);
        }
    }
}