using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AsphaltGas : GasBlock
    {

        public AsphaltGas(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("asphaltGas", "null");
            density = 0.115f;
            transmissivity = 1.22f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AsphaltGas block = new AsphaltGas(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, 998, "asphaltLiquid");
            return block;
        }

    }
}