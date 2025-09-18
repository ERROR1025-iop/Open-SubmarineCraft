using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SteelMushy : MushyBlock
    {

        public SteelMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("steelMushy", "null");
            density = 7.85f;
            transmissivity = 7.2f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SteelMushy block = new SteelMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 2750, "fineSteelLiquid", "fineSteelGas");
            return block;
        }
    }
}