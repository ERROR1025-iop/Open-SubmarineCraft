using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Cooler : RotationBlock
    {

        public Cooler(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("cooler", "null");

            thumbnailColor = new Color(0.5803f, 0.2627f, 0.0862f);
            density = 8.9f;
            transmissivity = 23.2f;

            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Cooler block = new Cooler(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            return block;
        }
    }
}