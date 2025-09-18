using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SteelLiquid : LiquidBlock
    {

        public SteelLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("steelLiquid", "null");

            density = 7.85f;
            transmissivity = 7.2f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SteelLiquid block = new SteelLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 2750, 1535, "steelMushy", "steel", 1);
            return block;
        }
    }
}