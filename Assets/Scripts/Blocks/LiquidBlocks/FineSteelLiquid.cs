using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class FineSteelLiquid : LiquidBlock
    {

        public FineSteelLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("fineSteelLiquid", "null");

            density = 7.85f;
            transmissivity = 7.2f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            FineSteelLiquid block = new FineSteelLiquid(blockId, parentObject, blockObject);
            // 10% 概率使用 "steelMushy"，其余 90% 使用 "steel"
            if (Random.value < 0.1f)
            {
                block.initLiquidBlock(blocksManager, 2750, 1535, "steelMushy", "fineSteel", 1);
            }
            else
            {
                block.initLiquidBlock(blocksManager, 2750, 1535, "steelMushy", "steel", 1);
            }
            return block;
        }
    }
}