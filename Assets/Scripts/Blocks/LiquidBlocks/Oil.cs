using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Oil : LiquidBlock
    {

        public Oil(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("oil", "material");
            thumbnailColor = new Color(0.1490196f, 0.07450981f, 0.1411765f);
            density = 0.87f;
            transmissivity = 1.45f;
            calorific = 1704.0f;

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Oil block = new Oil(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 180, 4, "dieselMushy", "oilSolid", 10);
            return block;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            fractionationRule(blocksEngine);
        }

        void fractionationRule(BlocksEngine blocksEngine)
        {
            if (gasChildCount > 0)
            {
                if (temperature > 90 && temperature < 160)
                {
                    createGasMethod(blocksEngine, blocksEngine.getBlocksManager().naturalGas);
                }
            }
            else
            {
                blocksEngine.createBlock(getCoor(), blocksEngine.getBlocksManager().asphalt);
            }

        }

        void createGasMethod(BlocksEngine blocksEngine, Block blockstatic)
        {
            Block up_block = getNeighborBlock(Dir.up);
            if (up_block.isAir())
            {
                blocksEngine.createBlock(up_block.getCoor(), blockstatic);
                gasChildCount--;
            }
        }

        public override bool isRootUnlock()
        {
            return true;
        }        
    }
}