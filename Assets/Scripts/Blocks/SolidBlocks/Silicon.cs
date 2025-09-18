using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Silicon : SolidBlock
    {

        public Silicon(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("silicon", "material");

            thumbnailColor = new Color(0.8784314f, 0.8392158f, 0.4784314f);
            density = 2.32f;
            transmissivity = 0.72f;
            max_storeAir = 0;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Silicon block = new Silicon(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "stoneLiquid", 1410, 455);
            return block;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            if (!isNeedDelete())
            {               
                if (dinasReduceRule(blocksEngine)) return;
            }

        }

        protected virtual bool dinasReduceRule(BlocksEngine blocksEngine)
        {
            if (temperature > 2000)
            {
                blocksEngine.createBlock(getCoor(), blocksEngine.getBlocksManager().siliconCarbide, temperature, press);
            }
            return false;
        }
    }
}
