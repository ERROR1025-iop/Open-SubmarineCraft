using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulfurLiquid : LiquidBlock
    {

        public SulfurLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulfurLiquid", "null");

            density = 2.36f;
            transmissivity = 0.72f;
            calorific = 1932.0f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulfurLiquid block = new SulfurLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 112.8f, 444.6f, "sulfurMushy", "sulfur", 10);
            return block;
        }
    }
}