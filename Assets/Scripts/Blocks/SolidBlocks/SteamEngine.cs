using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class SteamEngine : LargeFlipBlock
    {

        protected float outPutAcc;
        protected float powerBarValue;
        protected float lastOutputMe;
        protected float maxMeOutput;
        protected float outputMe;
        protected float storeFuel;
        protected int powerDirection;
        protected int storeSteam;
        protected int storeWater;
        protected float waterTemperature;
        protected float genGasTemperature;
        protected int childOneStack;
        protected float fuelTotalCalorific;
        protected float comsumeAir;
        protected float workHq;
        protected float lastOutput;
        protected float currentOutput;

        public SteamEngine(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("steamEngine", "engine", true);

            thumbnailColor = new Color(0.580f, 0.580f, 0.580f);
            transmissivity = 3.2f;
            density = 10.3f;
            storeFuel = 0;
            storeSteam = 0;
            outputMe = 0;
            lastOutputMe = 0;
            powerBarValue = 0;
            powerDirection = 1;
            outPutAcc = 0;
            storeWater = 0;
            childOneStack = 0;
            comsumeAir = 100;
            currentOutput = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SteamEngine block = new SteamEngine(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            block.initLargeBlock(new IPoint(3, 3));
            block.initFlipLargeBlock(10);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.steel.getId(), 9 },
                    { blocksManager.gearSet.getId(), 2 },
                    { blocksManager.shaft.getId(), 3 }                 
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 9);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 20;
        }

        public override void onWorldModeClick()
        {
            if (isOrigin())
            {
                if (powerBarValue > 0)
                    powerBarValue = 0;
                else
                    powerBarValue = 1;
            }
            else
            {
                SteamEngine block = getOrgBlock() as SteamEngine;
                if (block != null)
                {
                    block.onWorldModeClick();
                }
            }

        }

        public override void onPowerBarPush(float value)
        {
            powerBarValue = value;
            outPutAcc = (maxMeOutput * value - outputMe) * 0.1f;
        }

        public override void onDirectionBarPush(int value)
        {
            powerDirection = value;
        }

        
        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            if (getIsBroken())
            {
                blocksEngine.putMe(this, getPutMeCoor(), 0);
                return;
            }

            if (isOrigin())
            {
                //Debug.Log(steam_temperature);
                steamEngineRule(blocksEngine);
            }
            
            absorbWaterAndCompressSteamRule(blocksEngine);
        }

        protected void steamEngineRule(BlocksEngine blocksEngine)
        {
            furnaceGetFuelWaterMethod(blocksEngine);
            bool canHeat = false;

            if (storeFuel > 0 && powerBarValue > 0.05f)
            {
                float receive = Pooler.instance.requireAir(comsumeAir * powerBarValue);
                if (receive > comsumeAir * 0.9f)
                {
                    canHeat = true;
                }
            }

            if (canHeat)
            {
                float unityFuel = fuelTotalCalorific * powerBarValue / 300;
                storeFuel -= unityFuel;
                waterTemperature += Fire.C2HQ(unityFuel) * 0.6f / 4180f;
                setSteamTexture(true);
            }
            else
            {
                if (waterTemperature > getTemperature())
                {
                    waterTemperature -= 1;
                }
                setSteamTexture(false);
            }

            if (waterTemperature > 120)
            {
                if (storeWater > 0 && storeSteam < 20)
                {
                    genGasTemperature = waterTemperature;
                    storeSteam++;
                    storeWater--;
                    workHq = (waterTemperature - 25) * 4180f * 0.2f;
                    waterTemperature -= workHq * 1.13f / 4180f;
                }
            }

            if (waterTemperature > meltingPoint * 1.1f)
            {
                setTemperature(meltingPoint * 1.1f);
            }
            
            float output = currentOutput;
            float realOutPut = Mathf.Abs(Mathf.Lerp(lastOutput, output, 0.01f));
            blocksEngine.putMe(this, getPutMeCoor(), realOutPut);
            Debug.Log("Steam Eng output:" + output + ",realOutPut:" + realOutPut);
            lastOutput = realOutPut;
            currentOutput = 0;

            Debug.Log("waterTemperature:" + waterTemperature);
        }

        protected virtual void furnaceGetFuelWaterMethod(BlocksEngine blocksEngine)
        {
            if (storeFuel <= 0)
            {
                Coal coal = Coal.getCoalBlock();
                if (coal != null)
                {
                    fuelTotalCalorific = coal.getCalorific();
                    storeFuel = fuelTotalCalorific;
                    blocksEngine.removeBlock(coal.getCoor());
                }
            }
        }

        protected virtual void absorbWaterAndCompressSteamRule(BlocksEngine blocksEngine)
        {
            Block block = blocksEngine.getBlock(getOriginPoint());
            if (block.equalBlock(this))
            {
                SteamEngine originBlock = block as SteamEngine;
                compressSteamMethod(blocksEngine, Dir.up, originBlock);
                compressSteamMethod(blocksEngine, Dir.right, originBlock);
                compressSteamMethod(blocksEngine, Dir.left, originBlock);
                compressSteamMethod(blocksEngine, Dir.down, originBlock);               


                absorbWater:
                if (absorbWaterMethod(blocksEngine, Dir.up, originBlock)) return;
                if (absorbWaterMethod(blocksEngine, Dir.right, originBlock)) return;
                if (absorbWaterMethod(blocksEngine, Dir.left, originBlock)) return;
                if (absorbWaterMethod(blocksEngine, Dir.down, originBlock)) return;
            }
        }

        protected virtual void compressSteamMethod(BlocksEngine blocksEngine, int dir, SteamEngine originBlock)
        {
            Block block = getNeighborBlock(dir);
            if (block.isAir())
            {
                if (originBlock.storeSteam > 0)
                {      
                    GasBlock steamBlock = blocksEngine.createBlock(block.getCoor(), blocksEngine.getBlocksManager().waterGas) as GasBlock;    
                    steamBlock.addTemperature(originBlock.genGasTemperature);
                    if (originBlock.childOneStack == 20)
                    {
                        originBlock.childOneStack = 0;
                        steamBlock.setAtChildrensIndex(1);
                    }
                    else
                    {
                        originBlock.childOneStack++;
                        steamBlock.setAtChildrensIndex(3);
                    }
                    originBlock.storeSteam--;
                    float output = originBlock.workHq * 0.13f * originBlock.getEfficiency() * originBlock.powerDirection;
                    originBlock.currentOutput += output;
                }
            }          
        }

        IPoint getPutMeCoor()
        {
            return getOriginPoint() + new IPoint(isFlip ? -1 : 3, 1);
        }

        protected virtual bool absorbWaterMethod(BlocksEngine blocksEngine, int dir, SteamEngine originBlock)
        {
            Block block = getNeighborBlock(dir);
            if (block.equalBlock(blocksEngine.getBlocksManager().distilledWater) || block.isWater())
            {
                if (originBlock.storeWater <= 0)
                {
                    originBlock.storeWater += 20;
                    originBlock.waterTemperature = block.getTemperature();
                    blocksEngine.removeBlock(block.getCoor());
                }
                return true;
            }
            return false;
        }

        protected void setSteamTexture(bool isLight)
        {
            SteamEngine lightBlock = BlocksEngine.instance.getBlock(getReviseBlockCoor(new IPoint(1, 1))) as SteamEngine;
            if (lightBlock != null)
            {
                if (isLight)
                {
                    lightBlock.setSpriteRect(9);
                }
                else
                {
                    lightBlock.setSpriteRect(4);
                }
            }
        }

        public override bool isCanBind()
        {
            return true;
        }

        public override int[] getBindArr()
        {
            return new int[1] { 3 };
        }

        public override bool isRootUnlock()
        {
            return true;
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);
            if (m_isOrigin)
            {

                IUtils.keyValue2Writer(writer, "storeWater", storeFuel);
                IUtils.keyValue2Writer(writer, "storeWater", storeWater);
                IUtils.keyValue2Writer(writer, "storeSteam", storeSteam);
                IUtils.keyValue2Writer(writer, "childOneStack", childOneStack);
            }
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            if (m_isOrigin)
            {
                storeFuel = IUtils.getJsonValue2Float(blockData, "storeFuel");
                storeWater = IUtils.getJsonValue2Int(blockData, "storeWater");
                storeSteam = IUtils.getJsonValue2Int(blockData, "storeSteam");
                childOneStack = IUtils.getJsonValue2Int(blockData, "childOneStack");
            }
        }
    }
}
