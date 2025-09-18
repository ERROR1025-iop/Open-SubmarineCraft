using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class DistilledWaterMushy : MushyBlock
    {

        public DistilledWaterMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("distilledWaterMushy", "null");

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            DistilledWaterMushy block = new DistilledWaterMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 100, "distilledWater", "waterGas");
            return block;
        }
    }
}