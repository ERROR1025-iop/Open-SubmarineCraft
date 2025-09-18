using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace{
    public class GasBlock : Block
    {
        static public List<GasBlock> gasFuel;

        int moveTrend;
        bool limitMoved;
        int atChildrensIndex;

        protected float boilingPoint;
        Block liquidBlockStatic;

        const int MAX_COM_CHILDREN = 50;
        Block[] compressChildren;
        int compressChildrenStack;
        bool isInitChildrensIndex;

        float tiPress;

        public GasBlock(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            pState = PState.gas;

            m_isFluid = true;
            canStoreInTag = 2;
            moveTrend = 0;
            limitMoved = false;
            isInitChildrensIndex = false;
            atChildrensIndex = 0;
            compressChildrenStack = 0;
            canStoreInTag = 0;
            tiPress = 0;
            compressChildren = new Block[MAX_COM_CHILDREN];
            penetrationRate = 0.98f;
        }


        public override void onRegister()
        {
            base.onRegister();

            if (gasFuel == null)
            {
                gasFuel = new List<GasBlock>();
            }
            else
            {
                gasFuel.Clear();
            }
        }

        public static GasBlock getGasFuel()
        {
            if (gasFuel.Count > 0)
            {
                return gasFuel[gasFuel.Count - 1];
            }
            return null;
        }

        public override void clear(bool destroy)
        {
            //Debug.Log(compressChildrenStack);
            if (compressChildrenStack > 0)
            {
                //Debug.Log(compressChildrenStack);
            }
            base.clear(destroy);
        }

        public void initGasBlock(BlocksManager blocksManager, float boilingPoint, string liquidBlockName)
        {
            this.boilingPoint = boilingPoint;
            liquidBlockStatic = blocksManager.getBlockByName(liquidBlockName);
        }


        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (gasLiquefyingRule(blocksEngine)) return;
            gasMoveRule();
            gasPressRule(blocksEngine);
            gasCompressChildRealseRule(blocksEngine);
        }

        public override void threadUpdate(BlocksEngine blocksEngine)
        {
            base.threadUpdate(blocksEngine);
            gasPressRule(blocksEngine);
            gasTemperaturePressRule();
        }

        /// <summary>
        ///气体温度与压力之间的规律
        /// </summary>        
        public void gasTemperaturePressRule()
        {
            tiPress =  press + temperature * 0.37f;
        }

        public override float getTiPress()
        {
            return press;
        }

        /// <summary>
        ///气体压力规律
        /// </summary>
        protected void gasPressRule(BlocksEngine blocksEngine)
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
                float dp = (press - block.getPress()) * 0.9f;
                decPress(dp);
                block.addPress(dp);
            }
        }

        /// <summary>
        ///气体压缩释放规律
        /// </summary>
        protected virtual void gasCompressChildRealseRule(BlocksEngine blocksEngine)
        {
            if (compressChildrenStack > 0)
            {
                gasCompressChildRealseMethod(blocksEngine, Dir.down);
                gasCompressChildRealseMethod(blocksEngine, Dir.right);
                gasCompressChildRealseMethod(blocksEngine, Dir.left);
                gasCompressChildRealseMethod(blocksEngine, Dir.up);
            }
        }

        protected virtual void gasCompressChildRealseMethod(BlocksEngine blocksEngine, int neighborDir)
        {
            if (compressChildrenStack > 0)
            {
                Block neighborBlock = getNeighborBlock(neighborDir);
                Block air = blocksEngine.getBlocksManager().air;
                if (neighborBlock.equalBlock(air))
                {
                    Block child = popLastCompressChild();
                    //child.setTemperature(temperature);
                    decPress(100);
                    //Debug.Log("gas-100");                   
                    blocksEngine.placeBlock(neighborBlock.getCoor(), child);
                }
            }
        }

        /// <summary>
        ///添加压缩方块
        /// </summary>
        public bool addCompressChild(Block child)
        {
            if (compressChildrenStack < MAX_COM_CHILDREN)
            {
                compressChildren[compressChildrenStack] = child;
                compressChildrenStack++;               
                //addHeatQuantity((child.getHeatQuantity() - heatCapacity) * 0.5f);
                addPress(100);
                //Debug.Log("gas+100");
                return true;
            }
            return false;
        }

        /// <summary>
        ///拿出一个压缩方块
        /// </summary>
        public Block popLastCompressChild()
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

        protected void realseAllCompressChild()
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

        public override int getCompressChildrenCount()
        {
            return compressChildrenStack;
        }

        /// <summary>
        ///气体冷凝规律
        /// </summary>
        protected bool gasLiquefyingRule(BlocksEngine blocksEngine)
        {
            if (liquidBlockStatic != null && temperature < boilingPoint && isHigherNeighborBlockTemperature())
            {
                if (!isInitChildrensIndex)
                {
                    int totalGasChildCount = (liquidBlockStatic as LiquidBlock).getTotalGasChildCount();
                    atChildrensIndex = (int)(Random.value * totalGasChildCount);
                }

                Block airBlockStatic = blocksEngine.getBlocksManager().air;
                if (atChildrensIndex == 1)
                {
                    LiquidBlock liquidBlock = blocksEngine.createBlock(getCoor(), liquidBlockStatic, temperature, press) as LiquidBlock;
                    liquidBlock.setCalorific(calorific);
                    liquidBlock.setCompressChildren(compressChildren, compressChildrenStack);
                }
                else
                {
                    if (compressChildrenStack > 0)
                    {

                        gasLiquefyingRealseChildrensMethod(blocksEngine);
                        return true;
                    }
                    else
                    {
                        blocksEngine.createBlock(getCoor(), airBlockStatic, temperature, press);
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        ///气体冷凝规律
        /// </summary>
        protected void gasLiquefyingRealseChildrensMethod(BlocksEngine blocksEngine)
        {
            Block child = popLastCompressChild();
            decPress(100);
            //Debug.Log("gas-100");           
            blocksEngine.placeBlock(getCoor(), child);
            if (child.equalPState(PState.liquild) || child.equalPState(PState.mushy))
            {
                LiquidBlock liquidBlock = child as LiquidBlock;
                liquidBlock.setCompressChildren(compressChildren, compressChildrenStack);
            }
            else if (child.equalPState(PState.gas))
            {
                GasBlock gasBlock = child as GasBlock;
                gasBlock.setCompressChildren(compressChildren, compressChildrenStack);
            }
        }

        bool isHigherNeighborBlockTemperature()
        {

            if (getTemperature() > getNeighborBlock(Dir.up).getTemperature())
            {
                return true;
            }
            else if (getTemperature() > getNeighborBlock(Dir.right).getTemperature())
            {
                return true;
            }
            else if (getTemperature() > getNeighborBlock(Dir.left).getTemperature())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       

        /// <summary>
        ///气体移动规律
        /// </summary>
        protected virtual void gasMoveRule()
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

            /*if(up_block.isAir() && up_block.getPress() < press)
            {
                moveTo(up_block.getCoor());
                setLimitMoved(true);
                return;
            }else if(right_block.isAir() && right_block.getPress() < press)
            {
                moveTo(right_block.getCoor());
                return;
            }
            else if (left_block.isAir() && left_block.getPress() < press)
            {
                moveTo(left_block.getCoor());
                return;
            }
            else if (down_block.isAir() && down_block.getPress() < press)
            {
                moveTo(down_block.getCoor());
                return;
            }*/

            
            if (up_block.isAir())
            {
                moveTo(getCoor().getDirPoint(gravityUp));
                setLimitMoved(true);
            }
            else if (down_block.equalPState(PState.gas) && down_block.getCompressChildrenCount() < getCompressChildrenCount())
            {
                moveTo(getCoor().getDirPoint(gravityDown));
            }
            else if (down_block.isFluid() && down_block.getDensity() < density)
            {
                moveTo(getCoor().getDirPoint(gravityDown));
            }
            else if (down_block.equalBlock(this) && temperature < down_block.getTemperature())
            {
                moveTo(getCoor().getDirPoint(gravityDown));
            }
            else if ((left_block.isAir() && right_block.isAir()) ||
                left_block.isFluid() && right_block.isFluid())
            {
                if (isAir())
                {
                    moveTrend = 0;
                }
                else if (moveTrend == 0)
                {


                    if (right_block.getPress() < press)
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
            else if (MainSubmarine.pitch <= 0.2f && MainSubmarine.pitch >= -0.2f)
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
            else if ((left_block.isAir() && MainSubmarine.pitch >= -0.2f) || (left_block.isFluid() && left_block.getDensity() < density))
            {
                moveTo(getCoor().getDirPoint(Dir.left));
                moveTrend = Dir.left;
            }
            else if ((right_block.isAir() && MainSubmarine.pitch <= 0.2f) || (right_block.isFluid() && right_block.getDensity() < density))
            {
                moveTo(getCoor().getDirPoint(Dir.right));
                moveTrend = Dir.right;
            }
            
        }



        public void setAtChildrensIndex(int index)
        {
            atChildrensIndex = index;
            isInitChildrensIndex = true;
        }

        void setLimitMoved(bool limit)
        {
            limitMoved = limit;
        }

        public void setDensity(float d)
        {
            density = d;
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);
            IUtils.keyValue2Writer(writer, "at", atChildrensIndex);
            IUtils.keyValue2Writer(writer, "boilingPoint", boilingPoint);
            IUtils.keyValue2Writer(writer, "liquidBlock", liquidBlockStatic);
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            atChildrensIndex = IUtils.getJsonValue2Int(blockData, "at");
            boilingPoint = IUtils.getJsonValue2Float(blockData, "boilingPoint");
            liquidBlockStatic = IUtils.getJsonValue2Block(blockData, "liquidBlock");            
        }
    }
}
