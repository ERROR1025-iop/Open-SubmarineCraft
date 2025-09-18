using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class CopperOre : SolidBlock
    {

        public CopperOre(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("copperOre", "material");

            thumbnailColor = new Color(0.388f, 0.388f, 0.388f);
            density = 15f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            CopperOre block = new CopperOre(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            return block;
        }

        public override bool isRootUnlock()
        {
            return true;
        }
    }
}
