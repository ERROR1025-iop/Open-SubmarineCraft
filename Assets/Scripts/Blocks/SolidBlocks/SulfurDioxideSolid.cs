using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulfurDioxideSolid : SolidBlock
    {


        public SulfurDioxideSolid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulfurDioxideSolid", "null");

            thumbnailColor = new Color(0.8784314f, 0.8392158f, 0.4784314f);
            density = 2.9275f;
            transmissivity = 3.68f;
            max_storeAir = 0;
            isCanChangeRedAndCrackTexture = false;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulfurDioxideSolid block = new SulfurDioxideSolid(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "sulfurDioxideLiquid", -75.5f, 335);
            return block;
        }

        
    }
}
