using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class CoalPowder : Coal
    {               

        public CoalPowder(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("coalPowder", "material");
            thumbnailColor = new Color(0.1215f, 0.1215f, 0.1215f);

            transmissivity = 2.02f;
            calorific = 1720;
            unityCalorific = 30;
            burningPoint = 330;
            density = 1.02f;
            burningAir = 500;
            max_storeAir = 0;
            penetrationRate = 0.6f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            CoalPowder block = new CoalPowder(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelAsh", 1535, 1750);
            return block;
        } 

        public override bool isRootUnlock()
        {
            return false;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 2;
        }
    }
}