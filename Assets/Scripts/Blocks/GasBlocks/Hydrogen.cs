using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Hydrogen : GasBlock
    {
        protected float burningPoint;
        protected float burningAir;

        bool isInit;

        public Hydrogen(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("hydrogen", "material");
            transmissivity = 0.52f;
            heatCapacity = 1430f;
            density = 0.0899f;
            burningPoint = 585;
            burningAir = 400;
            calorific = 2900;
            isInit = false;
            canStoreInTag = 2;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Hydrogen block = new Hydrogen(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, -252.77f, "hydrogenLiquid");
            return block;
        }



        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            gasFuel.Remove(this);
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            onPoolerModeCreate();
            if (burnRule(blocksEngine)) return;
        }

        public void onPoolerModeCreate()
        {
            if (!isInit)
            {
                gasFuel.Add(this);
                isInit = true;
            }

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
                    fire.setFireColor(3);
                    fire.setBurnedBlock(BlocksManager.instance.waterGas);
                    return true;
                }
            }
            return false;
        }

        public override bool isRootUnlock()
        {
            return true;
        }
    }
}