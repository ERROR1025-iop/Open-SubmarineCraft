using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class TorpedpTube : LargeBlock
    {

        int readyCycle;
        bool m_isReady;
        int readyingTick;

        float predictorData;
        float targetAngle;

        static float targetDeep = 0;

        public TorpedpTube(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("torpedpTube", "weapon");

            thumbnailColor = new Color(0.3686f, 0.3686f, 0.3686f);
            transmissivity = 3.2f;
            density = 12.3f;

            m_isReady = true;
            readyingTick = 0;
            readyCycle = 20;
            targetAngle = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            TorpedpTube block = new TorpedpTube(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1430, 1320);
            block.initLargeBlock(new IPoint(7, 3));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.fineSteel.getId(), 21 }                  
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 21);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            torpedpTubeReadyCycleRule(blocksEngine);
        }

        protected void torpedpTubeReadyCycleRule(BlocksEngine blocksEngine)
        {
            if (isReady() == false)
            {
                if (readyingTick < readyCycle)
                {
                    readyingTick++;
                }
                else
                {
                    setIsReady(true, blocksEngine);
                    readyingTick = 0;
                }
            }
        }

        public override bool onFireButtonClicked()
        {
            if (isOrigin() == false)
            {
                return false;
            }

            if (Pooler.instance.fireWeapon != 0)
            {
                return false;
            }

            if (isReady())
            {
                if (Pooler.instance.takeOneTorpedp(this))
                {
                    var firePos = getBlockObject().transform.localPosition + new Vector3(-0.32f, 0.16f, 0);
                    float deep = targetDeep;
                    if (deep < 0)
                    {
                        deep = MainSubmarine.deep;
                    }
                    Pooler.instance.fireOneTorpedp(this, targetAngle, firePos, -deep / 10);
                    setIsReady(false, BlocksEngine.instance);
                    targetAngle = 0;
                    return true;
                }
            }

            return false;
        }

        public bool isReady()
        {
            return m_isReady;
        }

        public void setIsReady(bool b, BlocksEngine blocksEngine)
        {
            m_isReady = b;
            if (m_isReady)
            {
                Block block = blocksEngine.getBlock(getCoor() + new IPoint(5, 1));
                if (block.equalBlock(this))
                {
                    TorpedpTube ttBlock = block as TorpedpTube;
                    ttBlock.setSpriteRect(12);
                }
            }
            else
            {
                Block block = blocksEngine.getBlock(getCoor() + new IPoint(5, 1));
                if (block.equalBlock(this))
                {
                    TorpedpTube ttBlock = block as TorpedpTube;
                    ttBlock.setSpriteRect(21);
                }
            }
        }

        public override void onReciverWe(float voltage, int putterDir, Block putter)
        {
            base.onReciverWe(voltage, putterDir, putter);

            if (isOrigin())
            {
                predictorData = (int)voltage;
                targetAngle = (int)(predictorData / 100) + 0.5f;
            }
            else
            {
                TorpedpTube block = getOrgBlock() as TorpedpTube;
                if (block != null)
                {
                    block.onReciverWe(voltage, putterDir, putter);
                }
            }
        }

        public override int getSyntIconSpriteIndex()
        {
            return 22;
        }

        public override int isCanSettingValue()
        {
            return 2;
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { -100, 1000 };
        }

        public override void onSettingValueChange()
        {
            targetDeep = currentSettingValue;
        }

        public override int getCurrentSettingValue()
        {
            return (int)targetDeep;
        }

        public override string getSettingValueName()
        {
            return "Deep";
        }
    }
}
