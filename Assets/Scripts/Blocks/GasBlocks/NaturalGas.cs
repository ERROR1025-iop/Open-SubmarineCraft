using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class NaturalGas : DieselGas
    {

        public NaturalGas(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("naturalGas", "material");
            density = 0.255f;
            transmissivity = 2.32f;
            burningPoint = 650.0f;
            calorific = 2100.0f;
            canStoreInTag = 2;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            NaturalGas block = new NaturalGas(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, -161.5f, "naturalGasLiquid");
            return block;
        }

        public override bool isRootUnlock()
        {
            return true;
        }

    }
}