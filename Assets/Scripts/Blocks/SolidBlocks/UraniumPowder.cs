using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class UraniumPowder : SolidBlock
    {

        public UraniumPowder(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("uraniumPowder", "material");

            transmissivity = 9.32f;
            density = 18.95f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            UraniumPowder block = new UraniumPowder(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelAsh", 1535, 1750);
            return block;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 1;
        }
    }
}
