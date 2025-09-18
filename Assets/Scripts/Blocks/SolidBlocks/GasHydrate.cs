using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class GasHydrate : SolidBlock
    {
        float burningPoint;

        public GasHydrate(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("gasHydrate", "material");
            density = 0.9f;
            transmissivity = 1.42f;
            burningPoint = 650.0f;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            GasHydrate block = new GasHydrate(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "naturalGasLiquid", 20, 550);
            return block;
        }

    }
}