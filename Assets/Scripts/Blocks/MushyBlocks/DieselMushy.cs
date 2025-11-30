using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class DieselMushy : MushyBlock
    {

        protected float burningPoint;
        protected float burningAir;

        public DieselMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("dieselMushy", "null");

            burningPoint = 220.0f;
            calorific = 2604.0f;
            burningAir = 300;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            DieselMushy block = new DieselMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 180, "diesel", "dieselGas");
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
                    fire.initFire("dieselGas", calorific, burningPoint);
                    fire.setDensity(density / totalGasChildCount);
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