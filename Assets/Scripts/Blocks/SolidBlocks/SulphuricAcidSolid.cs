using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulphuricAcidSolid : SolidBlock
    {


        public SulphuricAcidSolid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulphuricAcidSolid", "null");

            thumbnailColor = new Color(0.498f, 0.498f, 0.498f);
            density = 5.8305f;
            transmissivity = 0.35f;
            heatCapacity = 3500f;
            max_storeAir = 0;
            canStoreInTag = 0;
            isCanbeCorrosion = false;
            isCanChangeRedAndCrackTexture = false;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulphuricAcidSolid block = new SulphuricAcidSolid(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "sulphuricAcid", 10.4f, 122);
            return block;
        }
    }
}
