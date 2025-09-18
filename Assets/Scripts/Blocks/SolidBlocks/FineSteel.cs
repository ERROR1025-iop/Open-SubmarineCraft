using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class FineSteel : SolidBlock
    {


        public FineSteel(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("fineSteel", "structure");

            thumbnailColor = new Color(0.08235294f, 0.227451f, 0.2352941f);
            density = 12.35f;
            transmissivity = 7.2f;
            max_storeAir = 0;
            penetrationRate = 0.5f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            FineSteel block = new FineSteel(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "fineSteelLiquid", 1535, 3150);
            return block;
        }

        
    }
}
