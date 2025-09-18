using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace {
    public class SmallElectorEngine :LargeFlipBlock
    {
        protected float outPutAcc;
        protected float powerBarValue;
        protected float lastOutputMe;
        protected float maxMeOutput;
        protected float outputMe;
        protected float storeFuel;
        protected int powerDirection;
        protected float comsume;

        bool isWork;
        IPoint startOffset;
        int startSprite;
        protected int startBlockCount;

        public SmallElectorEngine(int id, GameObject parentObject, GameObject blockObject)
               : base(id, parentObject, blockObject)
        {
            initBlock("smallElectorEngine", "engine");

            thumbnailColor = new Color(0.2509f, 0.7294f, 0.8509f);
            transmissivity = 3.2f;
            density = 10.3f;
            currentSettingValue = 5600;

            outputMe = 0;
            outPutAcc = 0;
            lastOutputMe = 0;
            powerBarValue = 0;
            powerDirection = 1;
            isWork = false;

            comsume = 56;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SmallElectorEngine block = new SmallElectorEngine(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            block.initLargeBlock(new IPoint(3, 1));
            block.initFlipLargeBlock(6);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[4, 2] {
                    { blocksManager.winding.getId(), 2 },
                    { blocksManager.steel.getId(), 3 },
                    { blocksManager.copper.getId(), 1 },
                    { blocksManager.shaft.getId(), 3 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 3);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 12;
        }

        protected override void onLargeBlockInitFinish()
        {
            startBlockCount = size.x * size.y;
            startOffset = getOffset();
            startSprite = ((size.y - 1) - startOffset.y) * size.x + startOffset.x;
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
                SmallElectorEngine block = getOrgBlock() as SmallElectorEngine;
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

        IPoint getPutMeCoor()
        {
            return getOriginPoint() + new IPoint(isFlip ? -1 : 3, 0);
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (getIsBroken())
            {
                blocksEngine.putMe(this, getPutMeCoor(), 0);
                PoolerEngineSound.dieselEngineVolume = 0;
                return;
            }

            putMeRule(blocksEngine);
            updateSpriteRule();
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            PoolerEngineSound.dieselEngineVolume = 0;
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
                    setIsWork(true);
                    return true;
                }
                else
                {
                    outputMe = 0;
                    blocksEngine.putMe(this, getPutMeCoor(), outputMe);
                    lastOutputMe = outputMe;
                }
                setIsWork(false);
            }         
            return false;
        }

        void updateSpriteRule()
        {
            isWork = getIsWork();
            int index = startSprite + (isWork ? startBlockCount : 0);
            setSpriteRect(index);
        }

        public void setIsWork(bool isWork)
        {
            if (isOrigin())
            {
                this.isWork = isWork;
            }
            else
            {
                SmallElectorEngine orgBlock = getOrgBlock() as SmallElectorEngine;
                if (orgBlock != null && orgBlock.equalBlock(this))
                {
                    orgBlock.setIsWork(isWork);
                }
            }
        }

        public bool getIsWork()
        {
            if (isOrigin())
            {
                return isWork;           
            }
            else
            {
                SmallElectorEngine orgBlock = getOrgBlock() as SmallElectorEngine;
                if (orgBlock != null && orgBlock.equalBlock(this))
                {
                    return orgBlock.getIsWork();
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
            return new int[1] { 3 };
        }

        public override int isCanSettingValue()
        {
            return 3;
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 5600 };
        }

        public override string getSettingValueName()
        {
            return "max power";
        }
    }
}
