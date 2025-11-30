using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulfurMushy : MushyBlock
    {

        float burningPoint;
        float burningAir;

        public SulfurMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulfurMushy", "null");

            burningPoint = 168f;
            calorific = 1932.0f;
            burningAir = 300;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulfurMushy block = new SulfurMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 444.6f, "sulfurLiquid", "sulfurGas");
            return block;
        }

        protected override GasBlock createGasMethod(BlocksEngine blocksEngine, IPoint coor, bool compressSelf)
        {
            if (temperature > burningPoint)
            {
                float receive = Pooler.instance.requireAir(burningAir);
                if (receive > burningAir * 0.9f)
                {
                    Block fireBlockStatic = blocksEngine.getBlocksManager().fire;
                    Fire fire;
                    if (compressSelf)
                    {
                        fire = blocksEngine.createCompressBlock(fireBlockStatic, this) as Fire;
                    }
                    else
                    {
                        fire = blocksEngine.createBlock(coor, fireBlockStatic, press) as Fire;
                    }
                    fire.initFire("sulfurLiquid", calorific, burningPoint);
                    fire.setDensity(density / totalGasChildCount);
                    fire.setFireColor(1); 
                    fire.setBurnedBlock(BlocksManager.instance.sulfurDioxide);
                    SeparationTemperatureCalculation(this, fire);
                    gasChildCount--;
                    return fire;
                }
                else
                {
                    return base.createGasMethod(blocksEngine, coor, compressSelf);
                }                   
            }
            else
            {
                return base.createGasMethod(blocksEngine, coor, compressSelf);
            }

        }
    }
}