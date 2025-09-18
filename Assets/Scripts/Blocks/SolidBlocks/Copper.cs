using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Copper : SolidBlock
    {

        public Copper(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("copper", "structure");

            thumbnailColor = new Color(0.5803f, 0.2627f, 0.0862f);
            density = 8.9f;
            transmissivity = 23.2f;
            heatCapacity = 390f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Copper block = new Copper(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            return block;
        }
    }
}