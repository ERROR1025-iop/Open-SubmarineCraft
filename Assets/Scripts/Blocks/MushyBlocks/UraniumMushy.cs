using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class UraniumMushy : MushyBlock
    {

        public UraniumMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("uraniumMushy", "null");

            density = 18.95f;
            transmissivity = 9.32f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            UraniumMushy block = new UraniumMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 3745, "uraniumLiquid", "uraniumGas");
            return block;
        }
    }
}