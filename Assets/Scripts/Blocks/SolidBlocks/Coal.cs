using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Coal : Wood
    {

        static public List<Coal> coals;

        public Coal(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("coal", "material");
            thumbnailColor = new Color(0.1215f, 0.1215f, 0.1215f);

            transmissivity = 2.02f;
            calorific = 1443;
            unityCalorific = 9;
            burningPoint = 330;
            density = 1.02f;
            burningAir = 200;
            max_storeAir = 0;
            penetrationRate = 0.7f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Coal block = new Coal(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "null", 3300, 985);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            return null;
        }

        public override void onRegister()
        {
            base.onRegister();

            if (coals == null)
            {
                coals = new List<Coal>();
            }
            else
            {
                coals.Clear();
            }
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            if (equalBlock(BlocksManager.instance.coal))
            {
                coals.Add(this);
            }
            
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            if (equalBlock(BlocksManager.instance.coal))
            {
                coals.Remove(this);
            }               
        }

        public static Coal getCoalBlock()
        {
            if (coals.Count > 0)
            {
                return coals[coals.Count - 1];
            }
            return null;
        }

        public override bool isRootUnlock()
        {
            return true;
        }

    }
}