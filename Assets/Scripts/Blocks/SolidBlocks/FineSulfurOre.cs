using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class FineSulfurOre : SolidBlock
    {


        public FineSulfurOre(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("fineSulfurOre", "material");

            thumbnailColor = new Color(0.8901961f, 0.8156863f, 0.1764706f);
            density = 3.35f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            FineSulfurOre block = new FineSulfurOre(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "sulfurLiquid", 112.8f, 744);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.sulfurPowder.getId(), 3 }


            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

    }
}
