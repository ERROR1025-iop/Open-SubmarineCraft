using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scraft.BlockSpace
{
    public class OilSolid : SolidBlock
    {
        public OilSolid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("oilSolid", "null");

            thumbnailColor = new Color(0.1490196f, 0.07450981f, 0.1411765f);
            density = 0.87f;
            transmissivity = 1.45f;           
            max_storeAir = 0;
            isCanChangeRedAndCrackTexture = false;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            OilSolid block = new OilSolid(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "oil", 4, 335);
            return block;
        }
    }
}
