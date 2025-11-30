using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class StoneFurnace : IronFurnace
    {

        public StoneFurnace(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("stoneFurnace", "industry");
            transmissivity = 1.72f;
            density = 13.5f;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            StoneFurnace block = new StoneFurnace(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "stoneLiquid", 1300, 940);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.stone.getId(), 8 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }
    }
}
