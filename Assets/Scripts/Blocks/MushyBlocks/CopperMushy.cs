using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class CopperMushy : MushyBlock
    {

        public CopperMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("copperMushy", "null");

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            CopperMushy block = new CopperMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 2750, "copperLiquid", "air");
            return block;
        }
    }
}