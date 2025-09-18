using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class CoarseCopperOre : SolidBlock
    {


        public CoarseCopperOre(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("coarseCopperOre", "material");

            thumbnailColor = new Color(0.388f, 0.388f, 0.388f);
            density = 12.7f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            CoarseCopperOre block = new CoarseCopperOre(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            return block;
        }

        
    }
}
