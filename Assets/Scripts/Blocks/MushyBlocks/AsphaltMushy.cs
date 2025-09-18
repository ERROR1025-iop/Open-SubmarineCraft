using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AsphaltMushy : MushyBlock
    {

        public AsphaltMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("asphaltMushy", "null");
            density = 1.15f;
            transmissivity = 1.22f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AsphaltMushy block = new AsphaltMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 998, "asphaltLiquid", "asphaltGas");
            return block;
        }
    }
}