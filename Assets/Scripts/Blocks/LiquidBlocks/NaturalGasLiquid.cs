using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class NaturalGasLiquid : Diesel
    {
        

        public NaturalGasLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("naturalGasLiquid", "null");
            density = 0.855f;
            transmissivity = 2.32f;
            canStoreInTag = 0;           
            calorific = 2100.0f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            NaturalGasLiquid block = new NaturalGasLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, -161.5f, -182.5f, "naturalGasMushy", "null", 10);
            return block;
        }
    }
}