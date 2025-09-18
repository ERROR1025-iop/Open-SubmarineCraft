using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class LeadPowder : SolidBlock
    {

        public LeadPowder(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("leadPowder", "material");

            thumbnailColor = new Color(0.215f, 0.215f, 0.215f);
            density = 15f;
            transmissivity = 7.2f;
            max_storeAir = 0;
            isCanChangeRedAndCrackTexture = false;
            penetrationRate = 0.5f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            LeadPowder block = new LeadPowder(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelAsh", 1535, 1750);
            return block;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 1;
        }
    }
}
