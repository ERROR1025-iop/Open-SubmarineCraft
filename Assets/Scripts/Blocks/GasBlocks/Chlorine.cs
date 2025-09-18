using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Chlorine : GasBlock
    {

        public Chlorine(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("chlorine", "material");
            thumbnailColor = new Color(0.8313726f, 0.937255f, 0.5686275f);
            density = 0.321f;
            heatCapacity = 3500f;
            transmissivity = 0.35f;
            canStoreInTag = 2;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Chlorine block = new Chlorine(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, -34, "chlorineLiquid");
            return block;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (corrosionRule()) return;
        }

        protected bool corrosionRule()
        {
            if(temperature > 200)
            {
                if (corrosionMethod(Dir.down)) return true;
                if (corrosionMethod(Dir.left)) return true;
                if (corrosionMethod(Dir.right)) return true;
                if (corrosionMethod(Dir.up)) return true;
            }

            return false;
        }

        protected bool corrosionMethod(int dir)
        {
            SolidBlock solidBlock = getNeighborBlock(dir) as SolidBlock;
            if (solidBlock != null)
            {
                return solidBlock.onCorrosion(this, BlocksManager.instance.sulfurDioxide, temperature * 0.00001f);
            }
            return false;
        }

    }
}