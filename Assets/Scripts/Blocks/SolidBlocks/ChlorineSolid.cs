using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class ChlorineSolid : SolidBlock
    {


        public ChlorineSolid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulphuricAcidSolid", "null");

            thumbnailColor = new Color(0.8313726f, 0.937255f, 0.5686275f);
            density = 3.21f;
            heatCapacity = 3500f;
            transmissivity = 0.35f;
            max_storeAir = 0;
            canStoreInTag = 0;
            isCanbeCorrosion = false;
            isCanChangeRedAndCrackTexture = false;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            ChlorineSolid block = new ChlorineSolid(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "chlorineLiquid", -34f, 122);
            return block;
        }       
    }
}
