using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class ParticleBlock : Block
    {

        int life;

        bool limitMoved;
        int moveDir;
        bool isInsideOtherBlock;
        Block parasiticBlock;

        public ParticleBlock(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            pState = PState.particle;
            life = 50;
            canStoreInTag = 0;
            max_storeAir = 1000;
            transmissivity = 0.01f;
        }

        public void initParticleBlock(BlocksManager blocksManager)
        {

        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (particleDisappearRule(blocksEngine)) return;
            particleMoveRule(blocksEngine);
        }        

        public void insideUpdate(BlocksEngine blocksEngine, Block insideBlock)
        {
            particleInsideMoveRule(blocksEngine, insideBlock);
            particleTemperatureRule();

        }        

        protected virtual bool particleDisappearRule(BlocksEngine blocksEngine)
        {
            if (life <= 0)
            {
                blocksEngine.removeBlock(getCoor());
                return true;
            }
            return false;
        }

        protected virtual void particleMoveRule(BlocksEngine blocksEngine)
        {
            if (limitMoved)
            {
                limitMoved = false;
                return;
            }
            Block forwardBlock = getNeighborBlock(moveDir);
            if (!forwardBlock.isAir())
            {
                if(Random.value > forwardBlock.getPenetrationRate())
                {                    
                    moveDir = (int)(Random.value * 4);
                }
                else
                {
                    forwardBlock.particleMoveIn(this, false);
                    forwardBlock.onParticleCollide(this);
                }
           
            }
            else
            {
                moveTo(forwardBlock.getCoor());                                    
            }
            setTemperature(forwardBlock.getTemperature());
            setPress(forwardBlock.getPress());
            setLimitMoved(true);
            life--;
        }

        public void particleInsideMoveRule(BlocksEngine blocksEngine, Block insideBlock)
        {
            if (limitMoved)
            {
                limitMoved = false;
                return;
            }
            Block forwardBlock = insideBlock.getNeighborBlock(moveDir);
            if (!forwardBlock.isAir())
            {
                if (Random.value > forwardBlock.getPenetrationRate())
                {
                    moveDir = Dir.addDir(moveDir, 2);
                    Block backBlock = insideBlock.getNeighborBlock(moveDir);
                    jumpToOtherBlock(insideBlock, backBlock);
                    forwardBlock.onParticleCollide(this);
                }
                else
                {
                    jumpToOtherBlock(insideBlock, forwardBlock);
                    forwardBlock.onParticleCollide(this);
                }

            }
            else if (forwardBlock.isBorder())
            {
                removeSelf();
            }
            else
            {
                insideBlock.particleMoveOut(forwardBlock);
            }
            setLimitMoved(true);
            life--;
        }

        public void particleTemperatureRule()
        {
            if (isInsideOtherBlock)
            {
                //setTemperature(parasiticBlock.getTemperature());
                //setPress(parasiticBlock.getPress());
            }
        }

        public void collideRule()
        {
            if (isInsideOtherBlock)
            {
                parasiticBlock.onParticleCollide(this);
            }
        }

        public void jumpToOtherBlock(Block nowBlock ,Block block)
        {
            nowBlock.particlePushOut();
            block.particleMoveIn(this, true);
        }

        public void setMoveDir(int dir)
        {
            moveDir = dir;
        }

        void setLimitMoved(bool limit)
        {
            limitMoved = limit;
        }        

        public void onInsideOtherBlock(Block parentBlock)
        {
            parasiticBlock = parentBlock;
            isInsideOtherBlock = true;
        }

        public void onOutOtherBlock()
        {
            parasiticBlock = null;
            isInsideOtherBlock = false;
        }

        public bool getIsInsideOtherBlock()
        {
            return isInsideOtherBlock;
        }

        public Block getParasiticBlock()
        {
            return parasiticBlock;
        }

        public void removeSelf()
        {
            if (isInsideOtherBlock)
            {
                parasiticBlock.clearParticleBlockLayer();
            }
            else
            {
                BlocksEngine.instance.removeBlock(getCoor());
            }
        }
    }
}
