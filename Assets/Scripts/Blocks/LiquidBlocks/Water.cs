using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Water : LiquidBlock
    {


        public Water(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("water", "material");

            thumbnailColor = new Color(0f, 0f, 1f);
            canStoreInTag = 0; 
            density = 1f;
            transmissivity = 0.35f;
            maxDynamicBoilingPoint = 374;
            heatCapacity = 4180f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Water block = new Water(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 100, 0, "waterMushy", "ice", 20);
            return block;
        }

        public override bool isRootUnlock()
        {
            return true;
        }

        public override bool isCareerModeStoreUnlimit()
        {
            return true;
        }
    }
}