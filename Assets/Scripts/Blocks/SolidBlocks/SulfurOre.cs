using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulfurOre : Wood
    {
        public SulfurOre(int id, GameObject parentObject, GameObject blockObject)
                : base(id, parentObject, blockObject)
        {
            initBlock("sulfurOre", "material");
            thumbnailColor = new Color(0.388f, 0.388f, 0.388f);

            transmissivity = 0.72f;
            calorific = 1932.0f;
            unityCalorific = 5;
            burningPoint = 168;
            density = 2.36f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulfurOre block = new SulfurOre(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "sulfurLiquid", 112.8f, 985);
            return block;
        }   

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {            
            return null;
        }

        public override bool isRootUnlock()
        {
            return true;
        }
    }
}
