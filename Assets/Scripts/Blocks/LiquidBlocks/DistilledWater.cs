using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class DistilledWater : LiquidBlock
    {

        public DistilledWater(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("distilledWater", "material");

            thumbnailColor = new Color(0f, 0f, 1f);
            canStoreInTag = 2;
            density = 10f;
            transmissivity = 0.35f;
            maxDynamicBoilingPoint = 374;
            heatCapacity = 4180f;

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            DistilledWater block = new DistilledWater(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 100, 0, "distilledWaterMushy", "ice", 20);
            return block;
        }

        public override bool isRootUnlock()
        {
            return true;
        }
    }
}