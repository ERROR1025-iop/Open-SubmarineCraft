using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class Propeller : LargeBlock
    {
        protected IPoint receivePoint;
        protected IPoint startOffset;
        protected int startSprite;

        protected int updataFrameDelayPerUnit;
        protected int updataFrameDelayStack;
        protected int updataFrameStack;
        protected int outPutDir;
        protected float getMe;
        protected float speed;
        protected float targetSpeed;
        protected bool canUse;
        protected int startBlockCount;

        public Propeller(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("propeller", "machine");

            thumbnailColor = new Color(0f, 0f, 0f, 0f);
            transmissivity = 7.2f;
            density = 3.5f;
            updataFrameStack = 0;
            updataFrameDelayPerUnit = 30000;
            updataFrameDelayStack = 0;
            getMe = 0;
            outPutDir = 1;
            canUse = false;
            receivePoint = new IPoint(0, 2);
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Propeller block = new Propeller(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            block.initLargeBlock(new IPoint(3, 5));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.steel.getId(), 15 }
                 
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 15);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 60;
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
            Block rightBlock = getNeighborBlock(Dir.right);
            int tag = Pooler.instance.getOutsideAreaTag(rightBlock.getCoor());
            if (rightBlock.equalPState(PState.solid) == false && tag > 1 && tag < 4)
            {
                if (getIsCanUse() == false)
                {
                    setIsCanUse(true);
                }
            }
            MainSubmarine.forwardForceBlocks.Add(this);
            startBlockCount = size.x * size.y;
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            MainSubmarine.forwardForceBlocks.Remove(this);
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            propellerRule(blocksEngine);
            targetSpeed = 0;
        }

        protected void propellerRule(BlocksEngine blocksEngine)
        {
            float lerpSpeed = MainSubmarine.speed < 0.2f ? 0.1f : 0.02f;            
            speed = Mathf.Lerp(speed, targetSpeed, lerpSpeed);
            float AbsSpeed = Mathf.Abs(speed);

            if (AbsSpeed > 300)
            {
                setUpdataFrameDelayPerUnit(0);
            }
            else if (AbsSpeed < 1)
            {
                setUpdataFrameDelayPerUnit(30000);
            }
            else if (AbsSpeed > 1)
            {
                setUpdataFrameDelayPerUnit((int)(300 / AbsSpeed));
            }
        }


        public override bool frameUpdate()
        {
            if (updataFrameDelayStack < updataFrameDelayPerUnit)
            {
                updataFrameDelayStack++;
                return true;
            }
            updataFrameStack = updataFrameStack > 3 ? 0 : updataFrameStack;
            if (outPutDir > 0)
            {
                setSpriteRect(startSprite + updataFrameStack * startBlockCount);
            }
            else if (outPutDir <= 0)
            {
                int temp = 3 - updataFrameStack;
                setSpriteRect(startSprite + temp * startBlockCount);
            }
            updataFrameStack++;
            updataFrameDelayStack = 0;
            return true;
        }

        public override void onReciverMe(float me, int putterDir, Block putter)
        {
            if (!getOffset().equal(receivePoint))
            {
                return;
            }

            canUse = getIsCanUse();
            getMe = me * getBlockCount();

            float absMe = Mathf.Abs(me);
            targetSpeed = me;

            for (int offsetx = 0; offsetx < size.x; offsetx++)
            {
                for (int offsety = 0; offsety < size.y; offsety++)
                {
                    IPoint obOffset = new IPoint(offsetx, offsety);
                    Propeller offsetBlock = BlocksEngine.instance.getBlock(getOriginPoint() + obOffset) as Propeller;
                    if (offsetBlock == null || offsetBlock.equalBlock(this) == false)
                    {
                        continue;
                    }
                    offsetBlock.setTargetSpeed(targetSpeed);
                    offsetBlock.setOutPutDir(me >= 0 ? 1 : -1);
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
                Propeller block = getOrgBlock() as Propeller;
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
                Propeller block = getOrgBlock() as Propeller;
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
            if (getIsCanUse())
            {
                return speed * 0.004f;
            }
            else
            {
                return 0;
            }
        }

        public override Color getThumbnailColor(JsonData blockData)
        {
            if (!m_isBroken)
            {
                if (m_offset.x == 1 || m_offset.y == 2)
                {
                    return new Color(0.4286f, 0.4286f, 0.4286f);
                }
            }

            return base.getThumbnailColor(blockData);
        }

        public override bool isCanReceiveMe()
        {
            return true;
        }
    }

}
