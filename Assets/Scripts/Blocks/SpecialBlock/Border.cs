using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class Border : Block
    {

        public Border(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("border", "null");

            price = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Border block = new Border(blockId, parentObject, blockObject);
            return block;
        }

        public override float getMass()
        {
            return 0;
        }

        public override void update(BlocksEngine blocksEngine)
        {

        }

        public override void threadUpdate(BlocksEngine blocksEngine)
        {

        }

        public override bool isCollider()
        {
            return true;
        }
    }
}
