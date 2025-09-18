using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Semiconductor : SolidBlock
    {

        public Semiconductor(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("semiconductor", "material");

            thumbnailColor = new Color(0.5803f, 0.2627f, 0.0862f);
            density = 8.9f;
            transmissivity = 23.2f;
            heatCapacity = 390f;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Semiconductor block = new Semiconductor(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "null", 1357, 1120);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.silicon.getId(), 1},
                    { blocksManager.copper.getId(), 1 }                   
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 3);

            return syntInfos;
        }
    }
}