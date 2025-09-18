using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class WaterGas : GasBlock
    {

        public WaterGas(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("waterGas", "null");            
            transmissivity = 0.72f;
            heatCapacity = 4180f;
            density = 0.2f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            WaterGas block = new WaterGas(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, 100, "distilledWater");
            return block;
        }

    }
}