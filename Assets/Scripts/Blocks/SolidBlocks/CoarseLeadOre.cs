using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class CoarseLeadOre : SolidBlock
    {


        public CoarseLeadOre(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("coarseLeadOre", "material");

            thumbnailColor = new Color(0.388f, 0.388f, 0.388f);
            density = 14.2f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanChangeRedAndCrackTexture = false;
            penetrationRate = 0.4f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            CoarseLeadOre block = new CoarseLeadOre(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "leadLiquid", 327, 810);
            return block;
        }


    }
}
