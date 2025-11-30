using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class CopperLiquid : LiquidBlock
    {

        public CopperLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("copperLiquid", "null");
            density = 8.9f;
            transmissivity = 5.62f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            CopperLiquid block = new CopperLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 2750, 1357, "copperMushy", "copper", 1);
            return block;
        }
    }
}