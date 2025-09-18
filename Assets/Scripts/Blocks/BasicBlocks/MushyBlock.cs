using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class MushyBlock : LiquidBlock
    {
        protected Block gasBlockStatic;
        protected Block liquidBlockStatic;

        public MushyBlock(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            pState = PState.mushy;
            canStoreInTag = 0;
        }

        public void initMushyBlock(BlocksManager blocksManager, float boilingPoint, string liquidBlockName, string gasBlockName)
        {
            this.boilingPoint = boilingPoint;
            liquidBlockStatic = blocksManager.getBlockByName(liquidBlockName);
            gasBlockStatic = blocksManager.getBlockByName(gasBlockName);
        }

        public override void update(BlocksEngine blocksEngine)
        {
            heatMapRule();
            temperatureRule();
            liquildMoveRule();
            liquildPressRule();
            liquildCompressChildRealseRule(blocksEngine);
            if (mushyLiquefyingRule(blocksEngine)) return;
            if (mushyCreateGasRule(blocksEngine)) return;
        }

        protected virtual bool mushyLiquefyingRule(BlocksEngine blocksEngine)
        {
            if (liquidBlockStatic != null && temperature < boilingPoint)
            {
                LiquidBlock liquidBlock = (LiquidBlock)blocksEngine.createBlock(getCoor(), liquidBlockStatic, temperature, press);
                liquidBlock.setCalorific(calorific / totalGasChildCount);
                liquidBlock.setGasChildCount(gasChildCount);
                liquidBlock.setDensity(density + 0.1f - 0.001f);
                liquidBlock.setCompressChildren(compressChildren, compressChildrenStack);
                return true;
            }
            return false;
        }

        protected virtual bool mushyCreateGasRule(BlocksEngine blocksEngine)
        {
            if (gasBlockStatic != null)
            {

                Block airBlockStatic = blocksEngine.getBlocksManager().air;
                Block up_block = getNeighborBlock(Dir.up);

                //Debug.Log("gasChildCount:" + gasChildCount);            
                if (gasChildCount > 0)
                {
                    if (getNeighborBlock(Dir.up).equalBlock(airBlockStatic))
                    {
                        createGasMethod(blocksEngine, getCoor().getDirPoint(Dir.up), false);
                    }
                    else if (!(up_block.equalPState(PState.liquild) || up_block.equalPState(PState.mushy)))
                    {
                        if (gasChildCount == 1)
                        {
                            GasBlock gasBlock = createGasMethod(blocksEngine, getCoor(), false);
                            gasBlock.setCompressChildren(compressChildren, compressChildrenStack);
                            return true;
                        }
                        else
                        {
                            createGasMethod(blocksEngine, getCoor(), true);
                        }
                    }
                }
                else
                {
                    mushyBolingRealseChildrensMethod(blocksEngine);
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        ///气体冷凝规律
        /// </summary>
        protected void mushyBolingRealseChildrensMethod(BlocksEngine blocksEngine)
        {
            if (compressChildrenStack > 0)
            {
                Block child = popLastCompressChild();
                decPress(100);
                //Debug.Log("gas-100");
                setDensity(density - 0.001f);
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
            else
            {
                blocksEngine.createBlock(getCoor(), blocksEngine.getBlocksManager().air, temperature, press);
            }

        }

        protected virtual GasBlock createGasMethod(BlocksEngine blocksEngine, IPoint coor, bool compressSelf)
        {
            GasBlock gasBlock;
            if (compressSelf)
            {
                gasBlock = blocksEngine.createCompressBlock(gasBlockStatic, this) as GasBlock;
            }
            else
            {
                gasBlock = blocksEngine.createBlock(coor, gasBlockStatic, blocksEngine.getBlock(coor).getTemperature(), press) as GasBlock;
            }
            gasBlock.setCalorific(calorific);
            gasBlock.setAtChildrensIndex(gasChildCount);
            gasBlock.setDensity(density / totalGasChildCount);
            //decHeatQuantity(gasBlock.getHeatQuantity());蒸发吸热
            gasChildCount--;
            return gasBlock;
        }

        public void setGasChildCount(int c)
        {
            gasChildCount = c;
            totalGasChildCount = c;
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);
            IUtils.keyValue2Writer(writer, "gasChildCount", gasChildCount);
            IUtils.keyValue2Writer(writer, "totalGasChildCount", totalGasChildCount);
            IUtils.keyValue2Writer(writer, "boilingPoint", boilingPoint);
            IUtils.keyValue2Writer(writer, "gasBlockStatic", gasBlockStatic);
            IUtils.keyValue2Writer(writer, "liquidBlock", liquidBlockStatic);
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            gasChildCount = IUtils.getJsonValue2Int(blockData, "gasChildCount");
            totalGasChildCount = IUtils.getJsonValue2Int(blockData, "totalGasChildCount");
            boilingPoint = IUtils.getJsonValue2Float(blockData, "boilingPoint");
            gasBlockStatic = IUtils.getJsonValue2Block(blockData, "gasBlockStatic");
            liquidBlockStatic = IUtils.getJsonValue2Block(blockData, "liquidBlock");
        }
    }
}
