using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class NaturalGasMushy : DieselMushy
    {
               
        public NaturalGasMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("naturalGasMushy", "null");
            density = 0.855f;
            transmissivity = 2.32f;
            burningPoint = 650.0f;
            calorific = 2100.0f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            NaturalGasMushy block = new NaturalGasMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, -161.5f, "naturalGasLiquid", "naturalGas");
            return block;
        }      

        
    }
}