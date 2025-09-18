using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class IronPowder : SolidBlock
    {

        public IronPowder(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("ironPowder", "material");

            thumbnailColor = new Color(0.388f, 0.388f, 0.388f);
            density = 15f;
            transmissivity = 7.2f;
            max_storeAir = 0;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            IronPowder block = new IronPowder(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelAsh", 1535, 1750);
            return block;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 1;
        }
    }
}
