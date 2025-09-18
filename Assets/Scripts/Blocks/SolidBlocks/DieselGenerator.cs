using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class DieselGenerator : DieselEngine
    {
        public DieselGenerator(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("dieselGenerator", "power");

            thumbnailColor = new Color(1.0f, 0.9490f, 0.0f);
            transmissivity = 3.2f;
            density = 10.3f;
            storeFuel = 0;
            outputMe = 0;
            maxMeOutput = 1200;
            outPutAcc = 0;
            lastOutputMe = 0;
            powerBarValue = 0;
            powerDirection = 1;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            DieselGenerator block = new DieselGenerator(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            block.initLargeBlock(new IPoint(2, 2));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.steel.getId(), 4 },
                    { blocksManager.fineSteel.getId(), 1 },
                    { blocksManager.shaft.getId(), 2 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 4);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 5;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            if (getCurrentBindId() == 6)
            {
                powerBarValue = 1;
            }
        }

        public override void onPreesButtonClick(bool isClick)
        {
            base.onPreesButtonClick(isClick);
            if (isClick)
            {
                onWorldModeClick();
            }
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (putMeRule(blocksEngine)) return;
            if (absorbRule(blocksEngine)) return;
        }

        protected override bool putMeRule(BlocksEngine blocksEngine)
        {
            if (isOrigin() && storeFuel > 0)
            {
                float real_comsumeAir = comsumeAir * powerBarValue;
                float receive = Pooler.instance.requireAir(real_comsumeAir);
                if (receive > real_comsumeAir * 0.9f)
                {
                    outputMe = outPutAcc + lastOutputMe;
                    if (Mathf.Abs(outputMe) < 1)
                    {
                        outputMe = 0;
                    }

                    Pooler.instance.chargeElectric(this, outputMe * getEfficiency() * 0.009f);
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
                }
                else
                {
                    outputMe = 0;
                    lastOutputMe = outputMe;
                    setDiseselTexture(false);
                }
                    return true;
                
            }
            else if (isOrigin() && storeFuel <= 0)
            {
                outputMe = 0;
                lastOutputMe = outputMe;
                setDiseselTexture(false);
            }
            return false;
        }

        protected override void setDiseselTexture(bool isLight)
        {
            DieselGenerator lightBlock = BlocksEngine.instance.getBlock(getReviseBlockCoor(new IPoint(1, 1))) as DieselGenerator;
            if (lightBlock != null)
            {
                if (isLight)
                {
                    lightBlock.setSpriteRect(4);
                }
                else
                {
                    lightBlock.setSpriteRect(1);
                }
            }
        }

        public override Color getThumbnailColor(JsonData blockData)
        {
            if (IUtils.getJsonValue2Bool(blockData, "ib") == false)
            {
                IPoint toffset = new IPoint(IUtils.getJsonValue2Int(blockData, "ox"), IUtils.getJsonValue2Int(blockData, "oy"));
                if (toffset.x == 0 && toffset.y == 0)
                {
                    return new Color(0.647f, 0.647f, 0.647f);
                }
            }

            return base.getThumbnailColor(blockData);
        }

        public override int[] getBindArr()
        {
            return new int[4] { 3, 4, 5, 6 };
        }
    }
}