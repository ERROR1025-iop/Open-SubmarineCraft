using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Lead : SolidBlock
    {


        public Lead(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("lead", "material");

            thumbnailColor = new Color(0.2156863f, 0.2156863f, 0.2156863f);
            density = 11.34f;
            transmissivity = 11.2f;
            max_storeAir = 0;
            penetrationRate = 0.5f;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Lead block = new Lead(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "leadLiquid", 327, 810);
            return block;
        }

        
    }
}
