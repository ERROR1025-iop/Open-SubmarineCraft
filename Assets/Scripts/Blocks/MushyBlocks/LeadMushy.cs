using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class LeadMushy : MushyBlock
    {

        public LeadMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("leadMushy", "null");
            density = 11.34f;
            transmissivity = 11.2f;
            canStoreInTag = 0;
            penetrationRate = 0.1f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            LeadMushy block = new LeadMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 1525, "leadLiquid", "leadGas");
            return block;
        }
    }
}