using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Asphalt : SolidBlock
    {


        public Asphalt(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("asphalt", "structure");

            thumbnailColor = new Color(0.498f, 0.498f, 0.498f);
            density = 1.15f;
            transmissivity = 1.22f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Asphalt block = new Asphalt(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "asphaltLiquid", 485, 740);
            return block;
        }

        
    }
}
