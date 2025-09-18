using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class ChlorineMushy : MushyBlock
    {

        public ChlorineMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("chlorineMushy", "null");
            thumbnailColor = new Color(0.8313726f, 0.937255f, 0.5686275f);
            density = 3.21f;
            heatCapacity = 3500f;
            transmissivity = 0.35f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            ChlorineMushy block = new ChlorineMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, -34f, "chlorineLiquid", "chlorine");
            return block;
        }
    }
}