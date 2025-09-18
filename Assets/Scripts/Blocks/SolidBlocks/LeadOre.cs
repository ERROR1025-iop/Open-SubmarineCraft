using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class LeadOre : SolidBlock
    {

        public LeadOre(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("leadOre", "material");

            thumbnailColor = new Color(0.388f, 0.388f, 0.388f);
            density = 15f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
            penetrationRate = 0.5f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            LeadOre block = new LeadOre(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "leadLiquid", 327, 810);
            return block;
        }

        public override bool isRootUnlock()
        {
            return true;
        }
    }
}
