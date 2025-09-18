using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class AuxiliaryGenerator : LargeBlock
    {
        IPoint receivePoint;
        IPoint startOffset;
        int startSprite;

        int updataFrameDelayPerUnit;
        int updataFrameDelayStack;
        int updataFrameStack;
        int outPutDir;
        float outputMe;
        float speed;
        float targetSpeed;
        bool canUse;

        public AuxiliaryGenerator(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("auxiliaryGenerator", "power");

            thumbnailColor = IUtils.HexToColor("6F6F6F");
            transmissivity = 7.2f;
            density = 3.5f;
            updataFrameStack = 0;
            updataFrameDelayPerUnit = 30000;
            updataFrameDelayStack = 0;
            outputMe = 0;
            outPutDir = 1;
            canUse = false;
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AuxiliaryGenerator block = new AuxiliaryGenerator(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            block.initLargeBlock(new IPoint(3, 1));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.steel.getId(), 3 },
                     { blocksManager.winding.getId(), 1 },
                    { blocksManager.shaft.getId(), 3 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 3);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 1;
        }

        protected override void onLargeBlockInitFinish()
        {
            startOffset = getOffset();
            startSprite = ((size.y - 1) - startOffset.y) * size.x + startOffset.x;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            int tag = getOutsideTag();
            if (tag == 5)
            {
                if (getIsCanUse() == false)
                {
                    setIsCanUse(true);
                }
            }
            MainSubmarine.forwardForceBlocks.Add(this);
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            MainSubmarine.forwardForceBlocks.Remove(this);
            setSpeed(0);
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            speedGenerationRule();
            propellerRule(blocksEngine);
        }

        protected void propellerRule(BlocksEngine blocksEngine)
        {
            speed = Mathf.Lerp(speed, targetSpeed, 0.1f);
            float AbsSpeed = Mathf.Abs(speed);

            if (AbsSpeed > 10)
            {
                setUpdataFrameDelayPerUnit(0);
            }
            else if (AbsSpeed < 1)
            {
                setUpdataFrameDelayPerUnit(30000);
            }
            else if (AbsSpeed > 1)
            {
                setUpdataFrameDelayPerUnit((int)(10 / AbsSpeed));
            }
        }


        public override bool frameUpdate()
        {
            if (updataFrameDelayStack < updataFrameDelayPerUnit)
            {
                updataFrameDelayStack++;
                return true;
            }
            updataFrameStack = updataFrameStack > 7 ? 0 : updataFrameStack;
            if (outPutDir > 0)
            {
                setSpriteRect(startSprite + updataFrameStack * 3);
            }
            else if (outPutDir <= 0)
            {
                int temp = 7 - updataFrameStack;
                setSpriteRect(startSprite + temp * 3);
            }
            updataFrameStack++;
            updataFrameDelayStack = 0;
            return true;
        }


        public void speedGenerationRule()
        {
            if (!isOrigin() || m_isNeedDelete)
            {
                return;
            }

            outputMe = MainSubmarine.speed * MainSubmarine.forward;
            bool isUpWater = (Pooler.blocksMapQuaternion * blockObject.transform.position).y > WaterBackGround.waterLevelY;
            if (isUpWater)
            {
                outputMe *= 0.2f;
            }



            float absMe = Mathf.Abs(outputMe * 0.5f);
            Pooler.instance.chargeElectric(this, absMe);
            targetSpeed = outputMe;
            setSpeed(targetSpeed);
        }

        public void setSpeed(float speed)
        {
            for (int offsetx = 0; offsetx < size.x; offsetx++)
            {
                for (int offsety = 0; offsety < size.y; offsety++)
                {
                    IPoint obOffset = new IPoint(offsetx, offsety);
                    AuxiliaryGenerator offsetBlock = BlocksEngine.instance.getBlock(getOriginPoint() + obOffset) as AuxiliaryGenerator;
                    if (offsetBlock == null || offsetBlock.equalBlock(this) == false)
                    {
                        continue;
                    }
                    offsetBlock.setTargetSpeed(speed);
                    offsetBlock.setOutPutDir(outputMe >= 0 ? 1 : -1);
                }
            }
        }

        public bool getIsCanUse()
        {
            if (isOrigin())
            {
                return canUse;
            }
            else
            {
                AuxiliaryGenerator block = getOrgBlock() as AuxiliaryGenerator;
                if (block != null)
                {
                    return block.getIsCanUse();
                }
            }
            return false;
        }


        public void setIsCanUse(bool b)
        {
            if (isOrigin())
            {
                canUse = b;
            }
            else
            {
                AuxiliaryGenerator block = getOrgBlock() as AuxiliaryGenerator;
                if (block != null)
                {
                    block.setIsCanUse(b);
                }
            }
        }

        public void setTargetSpeed(float s)
        {
            targetSpeed = s;
        }

        public void setUpdataFrameDelayPerUnit(int unit)
        {
            updataFrameDelayPerUnit = unit;
        }

        public void setOutPutDir(int outPutDir)
        {
            this.outPutDir = outPutDir;
        }

        public override float getForce()
        {
            if (isOrigin() && getIsCanUse())
            {
                return -speed * 5;
            }
            else
            {
                return 0;
            }

        }

        public override Color getThumbnailColor(JsonData blockData)
        {
            return base.getThumbnailColor(blockData);
        }
    }
}