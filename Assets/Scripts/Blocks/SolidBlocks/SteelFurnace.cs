using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SteelFurnace : IronFurnace
    {

        public SteelFurnace(int id, GameObject parentObject, GameObject blockObject)
       : base(id, parentObject, blockObject)
        {
            initBlock("steelFurnace", "industry");
            transmissivity = 1.72f;
            density = 13.5f;
            unityFuelRate = 0.45f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SteelFurnace block = new SteelFurnace(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.steel.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        protected override bool absorbRule(BlocksEngine blocksEngine)
        {

            if (storeFuel <= 0)
            {
                if (absorbMethod(blocksEngine, Dir.down)) return true;
                if (absorbMethod(blocksEngine, Dir.right)) return true;
                if (absorbMethod(blocksEngine, Dir.left)) return true;
                if (absorbMethod(blocksEngine, Dir.up)) return true;
                if (absorbCoalMethod(blocksEngine)) return true;

            }
            return false;
        }

        protected override bool absorbCoalMethod(BlocksEngine blocksEngine)
        {
            GasBlock gasBlock = GasBlock.getGasFuel();
            if (gasBlock != null)
            {
                fuelTotalCalorific = gasBlock.getCalorific();
                storeFuel = fuelTotalCalorific;
                blocksEngine.removeBlock(gasBlock.getCoor());
                return true;
            }
            return false;
        }
    }
}
