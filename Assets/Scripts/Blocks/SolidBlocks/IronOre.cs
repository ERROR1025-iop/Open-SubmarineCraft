using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class IronOre : SolidBlock
    {

        public IronOre(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("ironOre", "material");

            thumbnailColor = new Color(0.388f, 0.388f, 0.388f);
            density = 15f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            IronOre block = new IronOre(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override bool isRootUnlock()
        {
            return true;
        }
    }
}
