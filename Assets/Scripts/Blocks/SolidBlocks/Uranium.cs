using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class Uranium : SolidBlock
    {
        public static float selftAt = 70000f;
        public static float inspireAt = 160000f;

        float probability;
        bool isInspire;
        int collideDir;

        public Uranium(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("uranium", "nuclear");

            thumbnailColor = new Color(0.0588f, 0.329f, 0f);
            transmissivity = 9.32f;
            density = 18.95f;
            isInspire = false;
            max_storeAir = 0;
            isCanbeCorrosion = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Uranium block = new Uranium(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "uraniumLiquid", 1132, 1210);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.uraniumPowder.getId(), 3 }
                  
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onParticleCollide(ParticleBlock particleBlock)
        {
            base.onParticleCollide(particleBlock);
            if (particleBlock.equalBlock(BlocksManager.instance.neutron))
            {
                setInspire(true);
                setSpriteRect(4);
                collideDir = getCoor().relativeDir(particleBlock.getCoor());
            }
        }     

        public bool checkControlRodMethod(BlocksEngine blocksEngine, int dir)
        {           
            return getNeighborBlock(dir).equalBlock(blocksEngine.getBlocksManager().controlRod);            
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            uraniumRule(blocksEngine);
            uraniumSelfHeatRule(blocksEngine);
            uraniumInspireRule(blocksEngine);
        }

        void uraniumRule(BlocksEngine blocksEngine)
        {
            probabilityMethod();
            if (Random.value < probability)
            {
                int sendDir = (int)(Random.value * 4);
                Block nBlock = getNeighborBlock(sendDir);
                if (nBlock.isAir())
                {
                    ParticleBlock particleBlock = blocksEngine.createBlock(nBlock.getCoor(), blocksEngine.getBlocksManager().neutron) as ParticleBlock;
                    heatNeighborBlock(blocksEngine);
                    particleBlock.setMoveDir(sendDir);
                }
                else
                {
                    ParticleBlock particleBlock = blocksEngine.createBlockWithNotPlace(blocksEngine.getBlocksManager().neutron, this) as ParticleBlock;
                    particleBlock.setMoveDir(sendDir);
                    particleMoveIn(particleBlock, true);
                }
            }


        }

        void uraniumSelfHeatRule(BlocksEngine blocksEngine)
        {
            int nblockCount = 0;
            for (int i = 0; i < 4; i++)
            {
                Block nblock = getNeighborBlock(i);
                if (nblock.equalBlock(this) || nblock.equalBlock(BlocksManager.instance.uraniumLiquid))
                {
                    nblockCount++;
                }
            }
            if (Random.value * 4 > (8 - nblockCount))
            {
                setInspire(true);                
                heatNeighborBlock(blocksEngine);
            }
        }

        void uraniumInspireRule(BlocksEngine blocksEngine)
        {
            if (isInspire)
            {
                int sDir = Dir.addDir(collideDir, 2);
                uraniumInspireMethod(blocksEngine, sDir);
                setInspire(false);
                setSpriteRect(0);
            }
        }

        void uraniumInspireMethod(BlocksEngine blocksEngine, int dir)
        {
            Block nBlock = getNeighborBlock(dir);
            if (nBlock.isAir())
            {
                Neutron neutron = blocksEngine.createBlock(nBlock.getCoor(), blocksEngine.getBlocksManager().neutron) as Neutron;
                if (neutron != null)
                {
                    neutron.setMoveDir(dir);
                }
            }
            heatNeighborBlock(blocksEngine);
        }

        void heatNeighborBlock(BlocksEngine blocksEngine)
        {
            float dt = selftAt;
            if (isInspire)
            {
                dt = inspireAt + temperature * 500;
            }
            addHeatQuantity(dt);
        }

        void probabilityMethod()
        {
            float t = getTemperature();
            float p = t / 4132.0f;
            probability = p;
        }

        public void setInspire(bool b)
        {
            isInspire = b;            
        }

        public override void clear(bool destroy)
        {
            base.clear(destroy);
        }
    }
}
