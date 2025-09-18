using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class HydrogenLiquid : LiquidBlock
    {

        public HydrogenLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("hydrogenLiquid", "null");

            transmissivity = 0.52f;
            heatCapacity = 1430f;
            density = 0.0899f;       
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            HydrogenLiquid block = new HydrogenLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, -252.77f, -259.2f, "hydrogenMushy", "null", 10);
            return block;
        }
    }
}