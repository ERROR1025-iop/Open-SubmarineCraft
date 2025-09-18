using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulphuricAcidGas : GasBlock
    {

        public SulphuricAcidGas(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulphuricAcidGas", "null");
            thumbnailColor = new Color(0.8784314f, 0.8392158f, 0.4784314f);
            density = 0.18305f;
            heatCapacity = 3500f;
            transmissivity = 0.35f;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulphuricAcidGas block = new SulphuricAcidGas(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, 337, "sulphuricAcid");
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
            if (solidBlock != null)
            {
                return solidBlock.onCorrosion(this, BlocksManager.instance.sulfurDioxide, 0.05f);
            }
            return false;
        }

    }
}