using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class LeadLiquid : LiquidBlock
    {

        public LeadLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("leadLiquid", "null");

            density = 11.34f;
            transmissivity = 11.2f;
            canStoreInTag = 0;
            penetrationRate = 0.1f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            LeadLiquid block = new LeadLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 1525, 327, "leadMushy", "lead", 1);
            return block;
        }
    }
}