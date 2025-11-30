using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class LiquidBlock : Block
    {
        int moveTrend;
        bool limitMoved;

        protected float boilingPoint;
        protected float dynamicBoilingPoint;
        protected float maxDynamicBoilingPoint;
        protected float freezingPoint;

        protected Block mushyBlockStatic;
        protected Block solidBlockStatic;

        protected int gasChildCount;
        protected int totalGasChildCount;

        const int MAX_COM_CHILDREN = 50;
        public int compressChildrenStack;
        protected Block[] compressChildren;

        public LiquidBlock(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            pState = PState.liquild;
            m_isFluid = true;
            moveTrend = 0;
            limitMoved = false;
            canStoreInTag = 2;

            boilingPoint = 100;
            maxDynamicBoilingPoint = float.MaxValue;
            dynamicBoilingPoint = boilingPoint;
            freezingPoint = 0;
            compressChildrenStack = 0;
            compressChildren = new Block[MAX_COM_CHILDREN];
        }

        public void initLiquidBlock(BlocksManager blocksManager, float boilingPoint, float freezingPoint, string mushyBlockName, string solidBlockName, int gasChildCount)
        {
            this.boilingPoint = boilingPoint;
            this.freezingPoint = freezingPoint;
            mushyBlockStatic = blocksManager.getBlockByName(mushyBlockName);
            solidBlockStatic = blocksManager.getBlockByName(solidBlockName);
            this.gasChildCount = gasChildCount;
            totalGasChildCount = gasChildCount;
        }

        public override string getBasicInformation()
        {
            return base.getBasicInformation()
                + "," + ILang.get("boiling point", "menu") + ":" + boilingPoint
                + "," + ILang.get("freezing point", "menu") + ":" + freezingPoint
                + "," + ILang.get("Mass", "menu") + ":" + getMass();
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (liquildBoilRule(blocksEngine)) return;
            if (liquildFreezingRule(blocksEngine)) return;
            liquildMoveRule();
            liquildCompressChildRealseRule(blocksEngine);

        }

        public override void threadUpdate(BlocksEngine blocksEngine)
        {
            base.threadUpdate(blocksEngine);

            liquildPressRule();
            liquildPressBoilingPointRule();
        }

        /// <summary>
        ///液体压力与沸点之间的规律
        /// </summary>
        protected void liquildPressBoilingPointRule()
        {
            dynamicBoilingPoint = boilingPoint + (press - 100) * 0.1245f;
            if (dynamicBoilingPoint > maxDynamicBoilingPoint)
            {
                dynamicBoilingPoint = maxDynamicBoilingPoint;
            }
        }

        /// <summary>
        ///液体压力规律
        /// </summary>
        protected void liquildPressRule()
        {
            pressCalculateMethod(Dir.up);
            pressCalculateMethod(Dir.down);
            pressCalculateMethod(Dir.left);
            pressCalculateMethod(Dir.right);
        }

        protected void pressCalculateMethod(int dir)
        {
            Block block = getNeighborBlock(dir);
            if ((block.isFluid() || block.isAir()) && press > block.getPress())
            {
                float dp = (press - block.getPress()) * 0.5f;
                decPress(dp);
                block.addPress(dp);
            }
        }

        /// <summary>
        ///液体压缩释放规律
        /// </summary>
        protected virtual void liquildCompressChildRealseRule(BlocksEngine blocksEngine)
        {
            if (compressChildrenStack > 0)
            {
                liquildCompressChildRealseMethod(blocksEngine, Dir.up);
                liquildCompressChildRealseMethod(blocksEngine, Dir.right);
                liquildCompressChildRealseMethod(blocksEngine, Dir.left);
                liquildCompressChildRealseMethod(blocksEngine, Dir.down);
            }
        }

        protected virtual void liquildCompressChildRealseMethod(BlocksEngine blocksEngine, int neighborDir)
        {
            if (compressChildrenStack > 0)
            {
                Block neighborBlock = getNeighborBlock(neighborDir);
                Block air = blocksEngine.getBlocksManager().air;
                if (neighborBlock.equalBlock(air))
                {
                    Block child = popLastCompressChild();
                    decPress(100);            
                    blocksEngine.placeBlock(neighborBlock.getCoor(), child);
                }
            }
        }

        /// <summary>
        ///添加压缩方块
        /// </summary>
        public virtual bool addCompressChild(Block child)
        {
            if (child.isNeedDelete())
            {
                return false;
            }

            if (compressChildrenStack < MAX_COM_CHILDREN)
            {
                compressChildren[compressChildrenStack] = child;
                compressChildrenStack++;               
                //addHeatQuantity((child.getHeatQuantity() - heatCapacity) * 0.5f);
                addPress(100);
                //Debug.Log("liquid+100");
                return true;
            }
            return false;
        }

        public void setCompressChild(Block[] compressChildren, int compressChildrenStack)
        {
            this.compressChildren = compressChildren;
            this.compressChildrenStack = compressChildrenStack;
        }

        /// <summary>
        ///拿出一个压缩方块
        /// </summary>
        protected Block popLastCompressChild()
        {
            if (compressChildrenStack > 0)
            {
                compressChildrenStack--;
                Block child = compressChildren[compressChildrenStack];
                compressChildren[compressChildrenStack] = null;
                child.openHeatMapMode(Pooler.HeatMap_Mode > 0);
                return child;
            }
            return null;
        }

        public void setCompressChildren(Block[] compressChildren, int compressChildrenStack)
        {
            this.compressChildren = compressChildren;
            this.compressChildrenStack = compressChildrenStack;
        }

        public override void clear(bool destroy)
        {
            if (compressChildrenStack > 0)
            {
                //Debug.Log(compressChildrenStack);
            }
            base.clear(destroy);
        }

        public void realseAllCompressChild()
        {
            for (int i = 0; i < MAX_COM_CHILDREN; i++)
            {
                Block block = popLastCompressChild();
                if (block != null)
                {
                    block.clear(true);
                }
            }
        }

        /// <summary>
        ///液体沸腾规律
        /// </summary>
        protected virtual bool liquildBoilRule(BlocksEngine blocksEngine)
        {
            if (mushyBlockStatic != null && temperature > dynamicBoilingPoint)
            {
                //Debug.Log("temperature:" + temperature + ",dynamicBoilingPoint" + dynamicBoilingPoint);
                MushyBlock mushyBlock = blocksEngine.createBlock(getCoor(), mushyBlockStatic, press, true) as MushyBlock;
                if (mushyBlock == null)
                {
                    Debug.Log("mushyBlock == null");
                }
                mushyBlock.setDensity(density - 0.1f);
                mushyBlock.setTransmissivity(transmissivity);
                mushyBlock.setCalorific(calorific);
                mushyBlock.setGasChildCount(gasChildCount);
                mushyBlock.setHeatCapacity(heatCapacity);
                mushyBlock.setHeatQuantity(heatQuantity);
                mushyBlock.setCompressChild(compressChildren, compressChildrenStack);
                return true;
            }
            return false;
        }

        /// <summary>
        ///液体冷却规律
        /// </summary>
        protected virtual bool liquildFreezingRule(BlocksEngine blocksEngine)
        {
            if (solidBlockStatic != null && temperature < freezingPoint)
            {
                blocksEngine.createBlock(getCoor(), solidBlockStatic, press, true);
                return true;
            }
            return false;
        }

        public override int getCompressChildrenCount()
        {
            return compressChildrenStack;
        }

        /// <summary>
        ///液体移动规律
        /// </summary>
        protected virtual void liquildMoveRule()
        {
            if (limitMoved)
            {
                limitMoved = false;
                return;
            }

            int gravityUp = MainSubmarine.inversion >= 0 ? Dir.up : Dir.down;
            int gravityDown = MainSubmarine.inversion >= 0 ? Dir.down : Dir.up;

            Block up_block = getNeighborBlock(gravityUp);
            Block right_block = getNeighborBlock(Dir.right);
            Block down_block = getNeighborBlock(gravityDown);
            Block left_block = getNeighborBlock(Dir.left);

            if (down_block.equalPState(PState.gas))
            {
                //if(equalPState(PState.mushy))Debug.Log("liquid-down for gravity");
                moveTo(getCoor().getDirPoint(gravityDown));
                moveTrend = 0;
            }
            else if (up_block.isLiquidOrMushy() && up_block.getCompressChildrenCount() < getCompressChildrenCount())
            {
                //if(equalPState(PState.mushy))Debug.Log("liquid-up for Compress");
                moveTo(getCoor().getDirPoint(gravityUp));
                moveTrend = 0;
                setLimitMoved(true);
            }
            else if (up_block.equalPState(PState.liquild) && up_block.getDensity() > density)
            {
                //if(equalPState(PState.mushy))Debug.Log("liquid-up for density");
                moveTo(getCoor().getDirPoint(gravityUp));
                moveTrend = 0;
                setLimitMoved(true);
            }
            else if ((left_block.isAir() && right_block.isAir()) ||
                left_block.isFluid() && right_block.isFluid())
            {

                if (moveTrend == 0)
                {
                    if (Random.value > 0.5f)
                    {
                        moveTo(getCoor().getDirPoint(Dir.right));
                        moveTrend = Dir.right;
                    }
                    else
                    {
                        moveTo(getCoor().getDirPoint(Dir.left));
                        moveTrend = Dir.left;
                    }
                }
                else if (moveTrend == Dir.left)
                {
                    moveTo(getCoor().getDirPoint(Dir.left));
                }
                else if (moveTrend == Dir.right)
                {
                    moveTo(getCoor().getDirPoint(Dir.right));
                }

            }
            else if (Random.value > 0.9f)
            {
                moveTrend = 0;
            }
            else if (MainSubmarine.pitch <= 0.2f && MainSubmarine.pitch >= -0.2f && Random.value > 0.5f)
            {
                if (left_block.isAir())
                {
                    moveTo(getCoor().getDirPoint(Dir.left));
                    moveTrend = Dir.left;
                }
                else if (right_block.isAir())
                {
                    moveTo(getCoor().getDirPoint(Dir.right));
                    moveTrend = Dir.right;
                }
            }
            else if ((left_block.equalPState(PState.gas) && MainSubmarine.pitch <= 0.2f) || (left_block.isFluid() && left_block.getDensity() > density))
            {
                moveTo(getCoor().getDirPoint(Dir.left));
                moveTrend = Dir.left;
            }
            else if ((right_block.equalPState(PState.gas) && MainSubmarine.pitch >= -0.2f) || (right_block.isFluid() && right_block.getDensity() > density))
            {
                moveTo(getCoor().getDirPoint(Dir.right));
                moveTrend = Dir.right;
            }
            
            // if (!limitMoved && up_block.isFluid() && temperature > up_block.getTemperature())
            // {
            //     if (up_block.equalBlock(this) || equalPState(PState.mushy))
            //     {
            //         Debug.Log("liquid-up for temperature");
            //         moveTo(getCoor().getDirPoint(gravityUp));
            //         moveTrend = 0;
            //         setLimitMoved(true);
            //     }
            // }
        }

        public bool isHasCompressChildren()
        {
            return compressChildrenStack > 0;
        }

        void setLimitMoved(bool limit)
        {
            limitMoved = limit;
        }

        void setFreezingPoint(float fp)
        {
            freezingPoint = fp;
        }

        public override float getFreezingPoint()
        {
            return freezingPoint;
        }

        public override float getBoilingPoint()
        {
            return boilingPoint;
        }

        public void setGasChildCount(int c)
        {
            gasChildCount = c;
        }

        public int getTotalGasChildCount()
        {
            return totalGasChildCount;
        }
    }
}