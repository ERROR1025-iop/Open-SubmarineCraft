using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class CopperPowder : SolidBlock
    {

        public CopperPowder(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("copperPowder", "material");

            thumbnailColor = new Color(0.5803f, 0.2627f, 0.0862f);
            density = 8.9f;
            transmissivity = 11.2f;
            heatCapacity = 390f;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            CopperPowder block = new CopperPowder(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelAsh", 1357, 1750);
            return block;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 1;
        }
    }
}