using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class FineLeadOre : SolidBlock
    {


        public FineLeadOre(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("fineLeadOre", "material");

            thumbnailColor = new Color(0.2156f, 0.2156f, 0.2156f);
            density = 10.2f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanChangeRedAndCrackTexture = false;
            penetrationRate = 0.3f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            FineLeadOre block = new FineLeadOre(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "leadLiquid", 327, 810);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.leadPowder.getId(), 3 }


            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

    }
}
