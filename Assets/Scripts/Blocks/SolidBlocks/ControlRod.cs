using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class ControlRod : RotationBlock
    {
        public ControlRod(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("controlRod", "nuclear");

            thumbnailColor = new Color(0.8862f, 0.8862f, 0.8862f);
            density = 12.35f;
            transmissivity = 7.2f;
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            ControlRod block = new ControlRod(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "null", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.coalPowder.getId(), 10 },
                    { blocksManager.fineSteel.getId(), 4 }                    
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            absordNeutronRule(blocksEngine);
        }

        public override void onParticleCollide(ParticleBlock particleBlock)
        {
            base.onParticleCollide(particleBlock);
            if (particleBlock.equalBlock(BlocksEngine.instance.getBlocksManager().neutron))
            {
                particleBlock.removeSelf();                
            }
        }

        void absordNeutronRule(BlocksEngine blocksEngine)
        {
            absordNeutronMethod(blocksEngine, Dir.up);
            absordNeutronMethod(blocksEngine, Dir.right);
            absordNeutronMethod(blocksEngine, Dir.down);
            absordNeutronMethod(blocksEngine, Dir.left);
        }

        void absordNeutronMethod(BlocksEngine blocksEngine, int dir)
        {
            Block nBlock = getNeighborBlock(dir);
            if (nBlock.equalBlock(blocksEngine.getBlocksManager().neutron))
            {
                blocksEngine.removeBlock(nBlock.getCoor());
            }else if (nBlock.isContainParticleBlock())
            {
                nBlock.clearParticleBlockLayer();
            }           
        }

        public override void clear(bool destroy)
        {
            base.clear(destroy);
        }

    }
}