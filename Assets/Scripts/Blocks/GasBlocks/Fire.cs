using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Fire : GasBlock
    {
        protected Block gasBlockStatic;
        protected Block burnedBlockStatic;
        float unityCalorific;
        float burningPoint;
        float burningAir;

        public Fire(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("fire", "null");
            max_storeAir = 1000;
            canStoreInTag = 0;
            density = 0.0129f;
            heatCapacity = 1012f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Fire block = new Fire(blockId, parentObject, blockObject);
            block.initGasBlock(blocksManager, 0, "null");
            return block;
        }

        public virtual void initFire(string gasBlockName, float calorific, float burningPoint=0)
        {
            gasBlockStatic = BlocksManager.instance.getBlockByName(gasBlockName);
            if(gasBlockStatic != null)
            {
                setDensity(gasBlockStatic.density);
                setHeatCapacity(gasBlockStatic.heatCapacity);
            }
            setCalorific(calorific);
            unityCalorific = calorific / 20;
            burningAir = unityCalorific;
            this.burningPoint = burningPoint;
            calorific -= unityCalorific * 5;
            setHeatQuantity(C2HQ(unityCalorific * 5));
        }

        public void setBurnedBlock(Block burnedBlockStatic)
        {
            this.burnedBlockStatic = burnedBlockStatic;
            setDensity(burnedBlockStatic.getDensity());
            setHeatCapacity(burnedBlockStatic.heatCapacity);
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
                            blocksEngine.createBlock(getCoor(), air, press);
                        }
                        else
                        {
                            blocksEngine.createBlock(getCoor(), burnedBlockStatic, press);
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
            // if (gasBlockStatic != null)
            // {
            //     Block newBlock = blocksEngine.createBlock(getCoor(), gasBlockStatic, press); 
            //     newBlock.setCalorific(calorific);
            // }
            // else
            // {
            //     blocksEngine.removeBlock(getCoor(), true, press);
            // }
            blocksEngine.removeBlock(getCoor(), true, press);
        }

        void heatNeighborBlock(BlocksEngine blocksEngine)
        {

            float dt = C2HQ(unityCalorific);

            addHeatQuantity(dt);

        }

        static public float C2HQ(float calorific)
        {
            if (calorific > 0)
            {
                return calorific * 1000 * 12;
            }
            return 0;
        }
        static public float HQ2C(float hq)
        {
            if (hq > 0)
            {
                return hq / 1000 / 12; 
            }
            return 0;
        }
    }
}