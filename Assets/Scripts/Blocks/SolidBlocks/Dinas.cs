using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Dinas : SolidBlock
    {


        public Dinas(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("dinas", "material");

            thumbnailColor = new Color(0.882353f, 0.6941177f, 0.5882353f);
            density = 11.3f;
            transmissivity = 0.32f;
            max_storeAir = 0;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Dinas block = new Dinas(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "stoneLiquid", 1300, 940);
            return block;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            if (!isNeedDelete())
            {
                dinasMoveRule(blocksEngine);                
            }

        }      

 
    }
}
