using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class Ice : SolidBlock
    {

        public Ice(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("ice", "null");

            density = 0.9f;
            transmissivity = 1.42f;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Ice block = new Ice(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "water", 0, 550);
            return block;
        }
    }
}
