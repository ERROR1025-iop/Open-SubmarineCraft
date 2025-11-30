using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEditor;

namespace Scraft.BlockSpace
{
    public class PreBlockParticle : PreBlockTemperature
    {
        protected ParticleBlock particleBlockLayer;
        protected float penetrationRate;

        /// <summary>
        /// 主线程更新
        /// </summary>
        public virtual void update(BlocksEngine blocksEngine)
        {
            if (particleBlockLayer != null)
            {
                particleBlockLayer.insideUpdate(blocksEngine, (Block)this);
            }
        }

        /// <summary>
        ///粒子方块进入
        /// </summary>
        public void particleMoveIn(ParticleBlock block, bool isPlaceIn)
        {
            particleBlockLayer = block;
            block.onInsideOtherBlock((Block)this);
            if (!isPlaceIn)
            {
                BlocksEngine.instance.removeBlock(block.getCoor(), false, block.getPress());
            }
        }

        public void particleMoveOut(Block outBlock)
        {
            particleBlockLayer.onOutOtherBlock();
            BlocksEngine.instance.placeBlock(outBlock.getCoor(), particleBlockLayer);
            particleBlockLayer = null;
        }

        public ParticleBlock particlePushOut()
        {
            particleBlockLayer.onOutOtherBlock();
            ParticleBlock particleBlock = particleBlockLayer;
            particleBlockLayer = null;
            return particleBlock;
        }

        public bool isContainParticleBlock()
        {
            return particleBlockLayer != null;
        }

        /// <summary>
        /// 当被粒子碰撞的时候
        /// </summary>
        public virtual void onParticleCollide(ParticleBlock particleBlock)
        {

        }
        public void clearParticleBlockLayer()
        {
            if (particleBlockLayer != null)
            {
                particleBlockLayer.clear(true);
                particleBlockLayer = null;
            }
        }
    }
}