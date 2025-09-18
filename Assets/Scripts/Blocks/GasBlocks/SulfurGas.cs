using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulfurGas : GasBlock
    {

        protected float burningPoint;
        protected float burningAir;

        public SulfurGas(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulfurGas", "null");

            density = 0.236f;
            transmissivity = 0.72f;
            burningPoint = 168f;
            calorific = 1932.0f;
            burningAir = 500;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulfurGas block = new SulfurGas(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, 444.6f, "sulfurLiquid");
            return block;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (burnRule(blocksEngine)) return;
        }

        bool burnRule(BlocksEngine blocksEngine)
        {
            if (temperature > burningPoint)
            {
                float receive = Pooler.instance.requireAir(burningAir);
                if (receive > burningAir * 0.9f)
                {
                    Block fireBlockStatic = blocksEngine.getBlocksManager().fire;
                    Fire fire = blocksEngine.createBlock(getCoor(), fireBlockStatic, temperature, press) as Fire;
                    fire.initFire(blocksEngine.getBlocksManager(), this.getName(), calorific, 35f, burningPoint);
                    fire.setFireColor(1);
                    fire.setBurnedBlock(BlocksManager.instance.sulfurDioxide);
                    return true;
                }
            }
            return false;
        }

        public void setBurningPoint(float bp)
        {
            boilingPoint = bp;
        }
    }
}