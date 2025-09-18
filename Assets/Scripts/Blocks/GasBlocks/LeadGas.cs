using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class LeadGas : GasBlock
    {

        public LeadGas(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("leadGas", "null");
            density = 11.34f;
            transmissivity = 11.2f;
            canStoreInTag = 0;
            penetrationRate = 0.3f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            LeadGas block = new LeadGas(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, 1525, "leadLiquid");
            return block;
        }

    }
}