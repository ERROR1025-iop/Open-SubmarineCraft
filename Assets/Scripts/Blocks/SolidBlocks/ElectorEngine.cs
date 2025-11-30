using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class ElectorEngine : LargeFlipBlock
    {
        protected float outPutAcc;
        protected float powerBarValue;
        protected float lastOutputMe;
        protected float maxMeOutput;
        protected float outputMe;
        protected float storeFuel;
        protected int powerDirection;
        protected float comsume;

        public ElectorEngine(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("electorEngine", "engine", true);

            thumbnailColor = new Color(0.2509f, 0.7294f, 0.8509f);
            transmissivity = 3.2f;
            density = 10.3f;
            currentSettingValue = 11000;

            outputMe = 0;
            outPutAcc = 0;
            lastOutputMe = 0;
            powerBarValue = 0;
            powerDirection = 1;

            comsume = 78;

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            ElectorEngine block = new ElectorEngine(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            block.initLargeBlock(new IPoint(5, 3));
            block.initFlipLargeBlock(16);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[4, 2] {
                    { blocksManager.winding.getId(), 4 },
                    { blocksManager.steel.getId(), 15 },
                    { blocksManager.copper.getId(), 10 },
                    { blocksManager.shaft.getId(), 5 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 15);

            return syntInfos;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            maxMeOutput = currentSettingValue;
            comsume = maxMeOutput * 0.01f;
        }

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();
            maxMeOutput = currentSettingValue;
            comsume = maxMeOutput * 0.01f;
            outPutAcc = (maxMeOutput * powerBarValue - outputMe) * 0.1f;
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
                ElectorEngine block = getOrgBlock() as ElectorEngine;
                if (block != null)
                {
                    block.onWorldModeClick();
                }
            }
        }

        public override void onPreesButtonClick(bool isClick)
        {
            if (isClick)
            {                
                onWorldModeClick();
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
                //PoolerEngineSound.dieselEngineVolume = 0;
                return;
            }

            putMeRule(blocksEngine);
        }

        protected virtual void setTexture(bool isLight)
        {
            ElectorEngine lightBlock = BlocksEngine.instance.getBlock(getReviseBlockCoor(new IPoint(isFlip ? 4 : 0, 1))) as ElectorEngine;
            if (lightBlock != null)
            {
                if (isLight)
                {
                    lightBlock.setSpriteRect(15);
                }
                else
                {
                    lightBlock.setSpriteRect(isFlip ? 9 : 5);
                }
            }
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            //PoolerEngineSound.dieselEngineVolume = 0;            
        }

        protected virtual bool putMeRule(BlocksEngine blocksEngine)
        {
            if (isOrigin())
            {
                float totalComsume = Mathf.Abs(comsume * powerBarValue * powerDirection);
                float receive = Pooler.instance.requireElectric(this, totalComsume);
                float dr = receive / totalComsume;
                if (receive > 0)
                {
                    outputMe = outPutAcc + lastOutputMe;
                    if (Mathf.Abs(outputMe) < 1)
                    {
                        outputMe = 0;
                    }

                    blocksEngine.putMe(this, getPutMeCoor(), outputMe * dr * getEfficiency());
                    if (Mathf.Abs(outputMe) < 300)
                    {
                        outPutAcc = (maxMeOutput * powerDirection * powerBarValue - outputMe) * 0.3f;
                    }
                    else
                    {
                        outPutAcc = (maxMeOutput * powerDirection * powerBarValue - outputMe) * 0.1f;
                    }

                    lastOutputMe = outputMe;
                    PoolerEngineSound.electorEngineVolume += powerBarValue;
                    setTexture(true);
                    return true;
                }
                else
                {
                    outputMe = 0;
                    blocksEngine.putMe(this, getPutMeCoor(), outputMe);
                    lastOutputMe = outputMe;
                    setTexture(false);
                }
            }
            return false;
        }

        public override bool isCanBind()
        {
            return true;
        }

        public override int[] getBindArr()
        {
            return new int[3] { 3, 4, 5 };
        }

        public override int isCanSettingValue()
        {
            return 3;
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 50000 };
        }

        public override string getSettingValueName()
        {
            return "max power";
        }
    }
}