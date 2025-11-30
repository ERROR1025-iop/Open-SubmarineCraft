using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Sulfur : Wood
    {
        public Sulfur(int id, GameObject parentObject, GameObject blockObject)
                : base(id, parentObject, blockObject)
        {
            initBlock("sulfur", "material");
            thumbnailColor = new Color(0.8784314f, 0.8392158f, 0.4784314f);

            transmissivity = 0.72f;
            calorific = 1932.0f;
            unityCalorific = 5;
            burningPoint = 168;
            density = 2.36f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {            
            return null;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Sulfur block = new Sulfur(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "sulfurLiquid", 112.8f, 744);
            return block;
        }   



        public override bool isRootUnlock()
        {
            return false;
        }
    }
}
