using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class NeutronReflector : SolidBlock
    {


        public NeutronReflector(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("neutronReflector", "nuclear");

            thumbnailColor = new Color(0.1411765f, 0.6627451f, 0.5333334f);
            density = 12.35f;
            transmissivity = 7.2f;
            max_storeAir = 0;
            penetrationRate = 0.001f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            NeutronReflector block = new NeutronReflector(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "fineSteelLiquid", 1535, 3150);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.lead.getId(), 3 },
                    { blocksManager.fineSteel.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }
    }
}
