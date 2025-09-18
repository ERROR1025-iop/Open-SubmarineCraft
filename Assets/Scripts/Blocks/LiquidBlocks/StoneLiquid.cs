using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class StoneLiquid : LiquidBlock
    {

        public StoneLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("stoneLiquid", "null");

            density = 15f;
            transmissivity = 3.72f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            StoneLiquid block = new StoneLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 2750, 1535, "stoneMushy", "stone", 1);
            return block;
        }
    }
}