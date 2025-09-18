using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Charcoal : Coal
    {  
        public Charcoal(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("charcoal", "material");
            thumbnailColor = new Color(0.1215f, 0.1215f, 0.1215f);

            transmissivity = 0.32f;
         
            calorific = 1115;
            unityCalorific = 7;
            burningPoint = 330;
            density = 0.75f;
            burningAir = 200;
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Charcoal block = new Charcoal(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "null", 3300, 985);
            return block;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            if (equalBlock(BlocksManager.instance.charcoal))
            {
                coals.Add(this);
            }
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            if (equalBlock(BlocksManager.instance.charcoal))
            {
                coals.Remove(this);
            }
        }     
    }
}