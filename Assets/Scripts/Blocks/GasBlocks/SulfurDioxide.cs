using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulfurDioxide : GasBlock
    {

        protected float burningPoint;
        protected float burningAir;

        public SulfurDioxide(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulfurDioxide", "material");
            thumbnailColor = new Color(0.8784314f, 0.8392158f, 0.4784314f);
            density = 0.29275f;
            transmissivity = 3.68f;           
            canStoreInTag = 2;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulfurDioxide block = new SulfurDioxide(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, -10, "sulfurDioxideLiquid");
            return block;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (chemicalRule(blocksEngine)) return;
        }

        bool chemicalRule(BlocksEngine blocksEngine)
        {
            chemicalMethod(blocksEngine, BlocksManager.instance.water, BlocksManager.instance.sulphuricAcid);
            return false;
        }       

        bool chemicalMethod(BlocksEngine blocksEngine, Block reaction, Block product)
        {
            if (chemicalMethod(blocksEngine, Dir.down, reaction, product)) return true;
            if (chemicalMethod(blocksEngine, Dir.left, reaction, product)) return true;
            if (chemicalMethod(blocksEngine, Dir.right, reaction, product)) return true;
            if (chemicalMethod(blocksEngine, Dir.up, reaction, product)) return true;
            return false;
        }

        bool chemicalMethod(BlocksEngine blocksEngine, int dir, Block reaction, Block product)
        {
            IPoint reactionCoor = getCoor().getDirPoint(dir);
            if (blocksEngine.getBlock(reactionCoor).equalBlock(reaction))
            {
                blocksEngine.createBlock(reactionCoor, product);
                blocksEngine.removeBlock(getCoor());
                return true;
            }
            return false;
        }
    }
}