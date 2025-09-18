using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Fire : GasBlock
    {

        protected float unityCalorific;

        protected Block gasBlockStatic;
        protected Block burnedBlockStatic;

        float burningPoint;
        float burningAir;

        public Fire(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("fire", "null");
            max_storeAir = 1000;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Fire block = new Fire(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, 0, "null");
            return block;
        }

        public virtual void initFire(BlocksManager blocksManager, string gasBlockName, float calorific, float unityCalorific, float burningPoint)
        {
            gasBlockStatic = blocksManager.getBlockByName(gasBlockName);
            setCalorific(calorific);
            this.unityCalorific = unityCalorific;
            this.burningPoint = burningPoint;
            burningAir = unityCalorific;
        }

        public void setBurnedBlock(Block burnedBlockStatic)
        {
            this.burnedBlockStatic = burnedBlockStatic;
        }

        public void setFireColor(int spriteIndex)
        {
            setSpriteRect(spriteIndex);
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.gasMoveRule();

            if (burnRule(blocksEngine)) return;
        }

        bool burnRule(BlocksEngine blocksEngine)
        {
            if (temperature > burningPoint)
            {
                float receive = Pooler.instance.requireAir(burningAir);
                if (receive > burningAir * 0.9f)
                {
                    if (calorific > 0)
                    {
                        heatNeighborBlock(blocksEngine);
                        calorific -= unityCalorific;
                    }
                    else
                    {
                        if(burnedBlockStatic == null)
                        {
                            Block air = blocksEngine.getBlocksManager().air;
                            blocksEngine.createBlock(getCoor(), air, temperature, press);
                        }
                        else
                        {
                            blocksEngine.createBlock(getCoor(), burnedBlockStatic, temperature, press);
                        }
                       
                        return true;
                    }
                }
                else
                {
                    coolingMehtod(blocksEngine);
                    return true;
                }
            }
            else
            {
                coolingMehtod(blocksEngine);
                return true;
            }
            return false;
        }

        void coolingMehtod(BlocksEngine blocksEngine)
        {
            if (gasBlockStatic != null)
            {
                blocksEngine.createBlock(getCoor(), gasBlockStatic, temperature, press);
            }
            else
            {
                blocksEngine.removeBlock(getCoor(), true, press);
            }
        }

        void heatNeighborBlock(BlocksEngine blocksEngine)
        {

            float dt = unityCalorific * 2000;

            addHeatQuantity(dt);
            getNeighborBlock(Dir.up).addHeatQuantity(dt);
            getNeighborBlock(Dir.right).addHeatQuantity(dt);
            getNeighborBlock(Dir.down).addHeatQuantity(dt);
            getNeighborBlock(Dir.left).addHeatQuantity(dt);

        }
    }
}