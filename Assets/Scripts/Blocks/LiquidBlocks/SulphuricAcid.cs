using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulphuricAcid : LiquidBlock
    {
        public SulphuricAcid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulphuricAcid", "material");

            thumbnailColor = new Color(0.8784314f, 0.8392158f, 0.4784314f);
            canStoreInTag = 2;
            density = 1.8305f;
            transmissivity = 0.35f;
            maxDynamicBoilingPoint = 634;
            heatCapacity = 3500f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulphuricAcid block = new SulphuricAcid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 337, 10.4f, "sulphuricAcidMushy", "sulphuricAcidSolid", 10);
            return block;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (corrosionRule()) return;
        }

        protected bool corrosionRule()
        {
            if (corrosionMethod(Dir.down)) return true;
            if (corrosionMethod(Dir.left)) return true;
            if (corrosionMethod(Dir.right)) return true;
            if (corrosionMethod(Dir.up)) return true;
            return false;
        }

        protected bool corrosionMethod(int dir)
        {
            SolidBlock solidBlock = getNeighborBlock(dir) as SolidBlock;
            if(solidBlock != null)
            {
                return solidBlock.onCorrosion(this, BlocksManager.instance.sulfurDioxide, 0.5f);
            }
            return false;
        }
    }
}
