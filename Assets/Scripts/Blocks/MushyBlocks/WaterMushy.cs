using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class WaterMushy : MushyBlock
    {

        public WaterMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("waterMushy", "null");

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            WaterMushy block = new WaterMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 100, "water", "waterGas");
            return block;
        }
    }
}