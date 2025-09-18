using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SteelAsh : SolidBlock
    {

        public SteelAsh(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("steelAsh", "null");

            transmissivity = 6.2f;
            density = 2.3f;
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SteelAsh block = new SteelAsh(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "null", 1200, 153);
            return block;
        }
    }
}
