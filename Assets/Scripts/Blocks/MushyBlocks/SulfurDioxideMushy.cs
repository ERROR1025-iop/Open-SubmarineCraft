using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulfurDioxideMushy : MushyBlock
    {

        public SulfurDioxideMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulfurDioxideMushy", "null");

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulfurDioxideMushy block = new SulfurDioxideMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 100, "asphalt", "asphaltGas");
            return block;
        }
    }
}