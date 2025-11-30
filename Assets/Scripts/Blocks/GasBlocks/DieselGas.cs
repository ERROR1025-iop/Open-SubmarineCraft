using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class DieselGas : GasBlock
    {

        protected float burningPoint;
        protected float burningAir;

        public DieselGas(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("dieselGas", "null");

            density = 0.255f;
            transmissivity = 1.62f;
            burningPoint = 220.0f;
            calorific = 2604.0f;
            burningAir = 500;
            canStoreInTag = 0;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            gasFuel.Add(this);
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            gasFuel.Remove(this);
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            DieselGas block = new DieselGas(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, 180, "diesel");
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
                    Fire fire = blocksEngine.createBlock(getCoor(), fireBlockStatic, press) as Fire;
                    fire.initFire(getName(), calorific, burningPoint); 
                    return true;
                }
            }
            return false;
        }        
    }
}