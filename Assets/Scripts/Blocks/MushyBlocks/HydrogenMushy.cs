using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class HydrogenMushy : MushyBlock
    {
               
        public HydrogenMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("hydrogenMushy", "null");

            transmissivity = 0.52f;
            heatCapacity = 1430f;
            density = 0.0899f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            HydrogenMushy block = new HydrogenMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, -252.77f, "hydrogenLiquid", "hydrogen");
            return block;
        }
        
    }
}