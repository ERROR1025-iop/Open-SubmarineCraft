using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulfurDioxideLiquid : LiquidBlock
    {

        public SulfurDioxideLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulfurDioxideLiquid", "null");
            thumbnailColor = new Color(0.8784314f, 0.8392158f, 0.4784314f);
            density = 2.9275f;
            transmissivity = 3.68f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulfurDioxideLiquid block = new SulfurDioxideLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, -10f, -75.5f, "SulfurDioxideMushy", "SulfurDioxideSolid", 10);
            return block;
        }
    }
}