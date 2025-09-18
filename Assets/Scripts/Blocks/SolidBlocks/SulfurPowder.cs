using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulfurPowder : Sulfur
    {
        public SulfurPowder(int id, GameObject parentObject, GameObject blockObject)
                : base(id, parentObject, blockObject)
        {
            initBlock("sulfurPowder", "material");
            thumbnailColor = new Color(0.8784314f, 0.8392158f, 0.4784314f);

            transmissivity = 0.72f;
            calorific = 1932.0f;
            unityCalorific = 5;
            burningPoint = 168;
            density = 2.36f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulfurPowder block = new SulfurPowder(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelAsh", 112.8f, 1750);
            return block;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 2;
        }
    }
}
