using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class Wood : SolidBlock
    {

        protected bool isBurning;
        protected float burningPoint;
        protected float unityCalorific;
        protected int m_normalSpriteIndex;
        protected int m_buringSpriteIndex;
        protected float burningAir;

        protected int noOxygenBurningTime;
        protected int carbonizationMaxTime;

        public Wood(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("wood", "structure");
            thumbnailColor = new Color(0.678f, 0.5372f, 0.3176f);
            isCanChangeRedAndCrackTexture = false;
            transmissivity = 0.32f;
            density = 0.75f;

            burningPoint = 270;
            isBurning = false;
            calorific = 395;
            unityCalorific = 4;

            m_normalSpriteIndex = 0;
            m_buringSpriteIndex = 1;
            heatCapacity = 2730f;
            max_storeAir = 0;
            burningAir = 500;

            carbonizationMaxTime = 100;
            noOxygenBurningTime = 0;
            penetrationRate = 0.98f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Wood block = new Wood(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "null", 3300, 530);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.woodCargohold.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override string getBasicInformation()
        {
            return base.getBasicInformation()
                + "," + ILang.get("calorific", "menu") + ":" + calorific;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            if (burnRule(blocksEngine)) return;
            if (equalBlock(BlocksManager.instance.wood))
            {
                if (carbonizationRule(blocksEngine)) return;
            }
        }

        protected virtual bool carbonizationRule(BlocksEngine blocksEngine)
        {
            if(temperature > 400 && !isBurning)
            {
                if(noOxygenBurningTime > carbonizationMaxTime)
                {
                    blocksEngine.createBlock(getCoor(), BlocksManager.instance.charcoal);
                }
                else
                {
                    noOxygenBurningTime++;
                }                
            }
            return false;
        }

        protected virtual bool burnRule(BlocksEngine blocksEngine)
        {
            if (temperature > burningPoint)
            {
                float receive = Pooler.instance.requireAir(burningAir);
                if (receive > burningAir * 0.9f)
                {
                    isBurning = true;
                    if (isBurnChangeTexture())
                        setSpriteRect(m_buringSpriteIndex);
                    if (calorific > 0)
                    {
                        heatNeighborBlock(blocksEngine);
                        createFireMethod(blocksEngine);
                        calorific -= unityCalorific;
                    }
                    else
                    {
                        Block air = blocksEngine.getBlocksManager().air;
                        blocksEngine.createBlock(getCoor(), air, temperature, press);
                        return true;
                    }
                }
                else
                {
                    isBurning = false;
                    if (isBurnChangeTexture())
                        setSpriteRect(m_normalSpriteIndex);
                }
            }
            else if (isBurning == true)
            {
                isBurning = false;
                if (isBurnChangeTexture())
                    setSpriteRect(m_normalSpriteIndex);
                return true;
            }
            return false;
        }

        protected virtual bool isBurnChangeTexture()
        {
            return true;
        }

        protected virtual bool createFireMethod(BlocksEngine blocksEngine)
        {
            Block upBlock = getNeighborBlock(Dir.up);
            if (upBlock.equalBlock(blocksEngine.getBlocksManager().air))
            {

                if (Random.value > 0.5f)
                {
                    Block fireBlockStatic = blocksEngine.getBlocksManager().fire;
                    Fire fire = blocksEngine.createBlock(upBlock.getCoor(), fireBlockStatic, temperature, press) as Fire;
                    fire.initFire(blocksEngine.getBlocksManager(), "null", 130, 13, burningPoint);
                    return true;
                }
            }

            Block leftBlock = getNeighborBlock(Dir.left);
            if (leftBlock.equalBlock(blocksEngine.getBlocksManager().air))
            {

                if (Random.value > 0.9f)
                {
                    Block fireBlockStatic = blocksEngine.getBlocksManager().fire;
                    Fire fire = blocksEngine.createBlock(leftBlock.getCoor(), fireBlockStatic, temperature, press) as Fire;
                    fire.initFire(blocksEngine.getBlocksManager(), "null", 130, 13, burningPoint);
                    return true;
                }
            }

            Block rightBlock = getNeighborBlock(Dir.right);
            if (rightBlock.equalBlock(blocksEngine.getBlocksManager().air))
            {

                if (Random.value > 0.9f)
                {
                    Block fireBlockStatic = blocksEngine.getBlocksManager().fire;
                    Fire fire = blocksEngine.createBlock(rightBlock.getCoor(), fireBlockStatic, temperature, press) as Fire;
                    fire.initFire(blocksEngine.getBlocksManager(), "null", 130, 13, burningPoint);
                    return true;
                }
            }

            return false;
        }     

        protected virtual void heatNeighborBlock(BlocksEngine blocksEngine)
        {
            float dt = unityCalorific * 3000;

            addHeatQuantity(dt);
            getNeighborBlock(Dir.up).addHeatQuantity(dt);
            getNeighborBlock(Dir.right).addHeatQuantity(dt);
            getNeighborBlock(Dir.down).addHeatQuantity(dt);
            getNeighborBlock(Dir.left).addHeatQuantity(dt);
        }

        public override bool isRootUnlock()
        {
            return true;
        }

        public override float getBurningPoint()
        {
            return burningPoint;
        }
    }    
}
