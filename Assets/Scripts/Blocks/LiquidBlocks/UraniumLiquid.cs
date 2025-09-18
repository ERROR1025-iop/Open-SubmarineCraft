using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class UraniumLiquid : LiquidBlock
    {

        float probability;

        bool isInspire;
        int collideDir;

        public UraniumLiquid(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("uraniumLiquid", "null");

            transmissivity = 9.32f;
            density = 18.95f;
            isInspire = false;
            canStoreInTag = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            UraniumLiquid block = new UraniumLiquid(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 3745, 1132, "uraniumMushy", "uranium", 1);
            return block;
        }

        public override void onParticleCollide(ParticleBlock particleBlock)
        {
            base.onParticleCollide(particleBlock);
            if (particleBlock.equalBlock(BlocksManager.instance.neutron))
            {
                setInspire(true);                
                collideDir = getCoor().relativeDir(particleBlock.getCoor());
            }
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
                Block nblock = getNeighborBlock(Dir.up);
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
            float dt = Uranium.selftAt + temperature * 100;
            if (isInspire)
            {
                dt = Uranium.inspireAt + temperature * 200;
            }
            addHeatQuantity(dt);
            getNeighborBlock(Dir.up).addHeatQuantity(dt);
            getNeighborBlock(Dir.right).addHeatQuantity(dt);
            getNeighborBlock(Dir.down).addHeatQuantity(dt);
            getNeighborBlock(Dir.left).addHeatQuantity(dt);
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
    }
}