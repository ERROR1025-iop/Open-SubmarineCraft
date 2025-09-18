using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class ChlorineLiquid : LiquidBlock
    {
        public ChlorineLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("chlorineLiquid", "null");

            thumbnailColor = new Color(0.8313726f, 0.937255f, 0.5686275f);
            canStoreInTag = 2;
            density = 3.21f;
            heatCapacity = 3500f;
            transmissivity = 0.35f;
            maxDynamicBoilingPoint = 334;           
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            ChlorineLiquid block = new ChlorineLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, -34, -101f, "chlorineMushy", "chlorineSolid", 10);
            return block;
        }
    }
}
