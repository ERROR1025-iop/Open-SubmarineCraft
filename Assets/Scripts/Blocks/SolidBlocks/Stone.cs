using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Stone : SolidBlock
    {

        public Stone(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("stone", "material");

            thumbnailColor = new Color(0.388f, 0.388f, 0.388f);
            density = 15f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Stone block = new Stone(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "stoneLiquid", 1300, 940);
            return block;
        }

        public override bool isRootUnlock()
        {
            return true;
        }
    }
}
