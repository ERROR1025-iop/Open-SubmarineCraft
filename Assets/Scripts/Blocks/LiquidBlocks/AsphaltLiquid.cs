using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AsphaltLiquid : LiquidBlock
    {

        public AsphaltLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("asphaltLiquid", "null");

            density = 1.15f;
            transmissivity = 1.22f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AsphaltLiquid block = new AsphaltLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 998, 485, "asphaltMushy", "asphalt", 1);
            return block;
        }
    }
}