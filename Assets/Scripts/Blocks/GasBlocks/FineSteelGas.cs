using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class FineSteelGas : GasBlock
    {

        public FineSteelGas(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("fineSteelGas", "null");
            density = 7.85f;
            transmissivity = 7.2f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            FineSteelGas block = new FineSteelGas(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, 2750, "fineSteelLiquid");
            return block;
        }

    }
}