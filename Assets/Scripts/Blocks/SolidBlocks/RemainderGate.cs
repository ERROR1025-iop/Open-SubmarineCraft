using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scraft.BlockSpace
{
    public class RemainderGate : AndGate
    {
        public RemainderGate(int id, GameObject parentObject, GameObject blockObject)
             : base(id, parentObject, blockObject)
        {
            initBlock("remainderGate", "logical");

            density = 2.9f;
            transmissivity = 7.32f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            RemainderGate block = new RemainderGate(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            return block;
        }

        protected override void logicGateRule(BlocksEngine blocksEngine)
        {
            if (voltage2 != 0)
            {
                voltage = (int)(voltage1) % (int)(voltage2) + 0.99f;
                blocksEngine.putWe(this, getRelativeNeighborBlock(Dir.right).getCoor(), voltage);
            }
        }
    }
}
