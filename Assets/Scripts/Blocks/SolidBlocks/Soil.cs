using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Soil : SolidBlock
    {

        public Soil(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("soil", "material");

            thumbnailColor = new Color(0.462f, 0.262f, 0.152f);
            density = 15f;
            transmissivity = 0.42f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Soil block = new Soil(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "stoneLiquid", 1200, 410);
            return block;
        }

        public override bool isRootUnlock()
        {
            return true;
        }
    }
}
