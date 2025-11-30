using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEditor;

namespace Scraft.BlockSpace
{
    public class PreBlock
    {

        protected int blockId;
        protected string blockName;
        protected IPoint coor;
        public float density;

        public PreBlock()
        {
            density = 1.0f;
        }

        /// <summary>
        /// 获取相邻方块
        /// </summary>
        public Block getNeighborBlock(int dir)
        {
            return BlocksEngine.instance.getBlock(coor.getDirPoint(dir));
        }

        public virtual float getDensity()
        {
            return density;
        }

        public void setDensity(float d)
        {            
            density = d;
        }
    }
}