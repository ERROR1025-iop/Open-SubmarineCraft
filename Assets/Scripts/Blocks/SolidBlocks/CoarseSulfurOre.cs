using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class CoarseSulfurOre : SolidBlock
    {


        public CoarseSulfurOre(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("coarseSulfurOre", "material");

            thumbnailColor = new Color(0.498f, 0.498f, 0.498f);
            density = 5.35f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            CoarseSulfurOre block = new CoarseSulfurOre(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "sulfurLiquid", 112.8f, 744);
            return block;
        }

        
    }
}
