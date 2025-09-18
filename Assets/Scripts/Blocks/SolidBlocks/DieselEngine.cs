using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class DieselEngine : LargeFlipBlock
    {

        protected float outPutAcc;
        protected float powerBarValue;
        protected float lastOutputMe;
        protected float maxMeOutput;
        protected float outputMe;
        protected float storeFuel;
        protected int powerDirection;
        protected float comsumeAir;

        public DieselEngine(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("dieselEngine", "engine", true);

            thumbnailColor = new Color(0.647f, 0.647f, 0.647f);
            transmissivity = 3.2f;
            density = 10.3f;
            storeFuel = 0;
            outputMe = 0;
            maxMeOutput = 5200;
            outPutAcc = 0;
            lastOutputMe = 0;
            powerBarValue = 0;
            powerDirection = 1;
            comsumeAir = 3000;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            DieselEngine block = new DieselEngine(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            block.initLargeBlock(new IPoint(5, 3));
            block.initFlipLargeBlock(16);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.steel.getId(), 15 },
                    { blocksManager.fineSteel.getId(), 5 },
                    { blocksManager.shaft.getId(), 5 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 15);

            return syntInfos;
        }

        IPoint getPutMeCoor()
        {
            return getOriginPoint() + new IPoint(isFlip ? -1 : 5, 1);
        }

        public override int getSyntIconSpriteIndex()
        {
            return 32;
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
                DieselEngine block = getOrgBlock() as DieselEngine;
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
                PoolerEngineSound.electorEngineVolume = powerDirection;
                return;
            }

            if (putMeRule(blocksEngine)) return;
            if (absorbRule(blocksEngine)) return;
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            PoolerEngineSound.electorEngineVolume = 0;
        }

        protected virtual bool putMeRule(BlocksEngine blocksEngine)
        {
            if (isOrigin() && storeFuel > 0)
            {
                float real_comsumeAir = comsumeAir * powerBarValue;
                float receive = Pooler.instance.requireAir(real_comsumeAir);
                if(receive > real_comsumeAir * 0.9f)
                {
                    outputMe = outPutAcc + lastOutputMe;
                    if (Mathf.Abs(outputMe) < 1)
                    {
                        outputMe = 0;
                    }

                    blocksEngine.putMe(this, getPutMeCoor(), outputMe * getEfficiency());
                    decFuel(outputMe * 0.005f);
                    if (Mathf.Abs(outputMe) < 300)
                    {
                        outPutAcc = (maxMeOutput * powerDirection * powerBarValue - outputMe) * 0.3f;
                    }
                    else
                    {
                        outPutAcc = (maxMeOutput * powerDirection * powerBarValue - outputMe) * 0.1f;
                    }

                    lastOutputMe = outputMe;

                    setDiseselTexture((powerDirection != 0 && powerBarValue != 0));
                    PoolerEngineSound.dieselEngineVolume += powerBarValue;
                }
                else
                {
                    outputMe = 0;
                    blocksEngine.putMe(this, getPutMeCoor(), outputMe);
                    lastOutputMe = outputMe;
                    setDiseselTexture(false);
                }               
                return true;
            }
            else if (isOrigin() && storeFuel <= 0)
            {
                outputMe = 0;
                blocksEngine.putMe(this, getPutMeCoor(), outputMe);
                lastOutputMe = outputMe;
                setDiseselTexture(false);
            }
            return false;
        }

        protected virtual bool absorbRule(BlocksEngine blocksEngine)
        {
            Block block = blocksEngine.getBlock(getOriginPoint());
            if (block.equalBlock(this))
            {
                DieselEngine originBlock = block as DieselEngine;
                if (originBlock.isFuelEmpty())
                {
                    if (absorbMethod(blocksEngine, Dir.up, originBlock)) return true;
                    if (absorbMethod(blocksEngine, Dir.right, originBlock)) return true;
                    if (absorbMethod(blocksEngine, Dir.left, originBlock)) return true;
                    if (absorbMethod(blocksEngine, Dir.down, originBlock)) return true;
                }
            }
            return false;
        }

        protected bool absorbMethod(BlocksEngine blocksEngine, int dir, DieselEngine originBlock)
        {
            Block block = getNeighborBlock(dir);
            if (block.getCalorific() > 0 && block.equalPState(PState.solid) == false)
            {
                originBlock.addFuel(block.getCalorific());
                blocksEngine.removeBlock(block.getCoor());
                return true;
            }
            return false;
        }

        public void addFuel(float fuel)
        {
            storeFuel += fuel;
        }

        public bool decFuel(float fuel)
        {
            storeFuel -= fuel;
            if (storeFuel <= 0)
            {
                return true;
            }
            return false;
        }

        public bool isFuelEmpty()
        {
            return storeFuel <= 0;
        }

        public float getStoreFuel()
        {
            return storeFuel;
        }

        public override bool isCanBind()
        {
            return true;
        }

        public override int[] getBindArr()
        {
            return new int[1] { 3 };
        }


        protected virtual void setDiseselTexture(bool isLight)
        {
            DieselEngine lightBlock = BlocksEngine.instance.getBlock(getReviseBlockCoor(new IPoint(isFlip ? 4 : 0,  1))) as DieselEngine;
            if (lightBlock != null)
            {
                if (isLight)
                {
                    lightBlock.setSpriteRect(15);
                }
                else
                {
                    lightBlock.setSpriteRect(isFlip? 9 : 5);
                }
            }
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);
            IUtils.keyValue2Writer(writer, "lastOutputMe", lastOutputMe);
            IUtils.keyValue2Writer(writer, "storeFuel", storeFuel);
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            lastOutputMe = IUtils.getJsonValue2Float(blockData, "lastOutputMe");
            storeFuel = IUtils.getJsonValue2Float(blockData, "storeFuel");
        }

        public override Color getThumbnailColor(JsonData blockData)
        {
            if (IUtils.getJsonValue2Bool(blockData, "ib") == false)
            {
                IPoint toffset = new IPoint(IUtils.getJsonValue2Int(blockData, "ox"), IUtils.getJsonValue2Int(blockData, "oy"));
                if (toffset.y == 2)
                {
                    return new Color(1.0f, 0.9490f, 0.0f);
                }
            }

            return base.getThumbnailColor(blockData);
        }

    }
}