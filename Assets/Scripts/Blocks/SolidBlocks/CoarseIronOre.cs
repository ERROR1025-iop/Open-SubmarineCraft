using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class CoarseIronOre : SolidBlock
    {


        public CoarseIronOre(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("coarseIronOre", "material");

            thumbnailColor = new Color(0.388f, 0.388f, 0.388f);
            density = 14.2f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            CoarseIronOre block = new CoarseIronOre(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        
    }
}
