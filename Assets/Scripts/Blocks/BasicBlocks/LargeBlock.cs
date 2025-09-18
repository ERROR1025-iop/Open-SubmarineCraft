using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace{ public class LargeBlock : SolidBlock
    {
        protected bool m_isOrigin;
        protected bool m_isBroken;

        protected IPoint m_offset;
        protected IPoint size;
        protected IPoint m_orgPoint;
        protected int blockCount;

        bool isRollBackRemove;
        bool removeTag;

        public LargeBlock(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            m_isOrigin = false;
            m_isLargeBlock = true;
            m_isBroken = true;
            isRollBackRemove = false;
            removeTag = false;

            size = new IPoint(1, 1);
            m_offset = new IPoint(0, 0);
            m_orgPoint = getCoor();
            blockCount = 1;
            isCanChangeRedAndCrackTexture = false;
        }

        public void initLargeBlock(IPoint blockSize)
        {
            size = blockSize;
            blockCount = size.x * size.y;
        }

        public override void onBuilderModeCreated()
        {
            base.onBuilderModeCreated();
            IRect rect = new IRect(getCoor().x, getCoor().y, size.x, size.y);

            if (isContainOtherBlocks(rect, getCoor()))
            {

                isRollBackRemove = true;
                Builder.instance.delBlock(getCoor());
                return;
            }


            m_isOrigin = true;
            setIsBroken(false);
            setOffset(new IPoint(0, 0));
            for (int offsetx = 0; offsetx < size.x; offsetx++)
            {
                for (int offsety = 0; offsety < size.y; offsety++)
                {
                    IPoint obOffset = new IPoint(offsetx, offsety);
                    if (!obOffset.isZeroPoint())
                    {
                        Block oldBlock = BlocksEngine.instance.getBlock(getCoor() + obOffset);
                        if (oldBlock != null)
                        {
                            Builder.instance.delBlock(oldBlock.getCoor());
                        }
                    }

                    LargeBlock offsetBlock = BlocksEngine.instance.createBlock(getCoor() + obOffset, this) as LargeBlock;
                    if (offsetBlock != null)
                    {
                        offsetBlock.setOffset(obOffset);
                        offsetBlock.onLargeBlockInitFinish();
                        offsetBlock.setIsBroken(false);

                        if (!obOffset.isZeroPoint())
                        {
                            BlocksManager.instance.addConsumeCargoCount(this, 1);
                        }                            
                        CardManager.instance.updatecargoCount();
                    }
                }
            }
        }

        public override void onBuilderModeDeleted()
        {
            base.onBuilderModeDeleted();
            if (removeTag || isRollBackRemove)
            {
                //release();
                return;
            }

            IPoint origin = getOriginPoint();
            for (int offsetx = 0; offsetx < size.x; offsetx++)
            {
                for (int offsety = 0; offsety < size.y; offsety++)
                {
                    IPoint obOffset = new IPoint(offsetx, offsety);
                    if (obOffset.equal(m_offset))
                    {
                        continue;
                    }
                    LargeBlock offsetBlock = BlocksEngine.instance.getBlock(origin + obOffset) as LargeBlock;
                    if (offsetBlock != null && offsetBlock.equalBlock(this))
                    {
                        offsetBlock.setRemoveTag(true);

                        Builder.instance.delBlock(origin + obOffset);
                    }
                }
            }

            //release();
        }

        public bool isContainOtherBlocks(IRect rect, IPoint exceptPoint)
        {
            for (int x = rect.x; x < rect.getMaxX(); x++)
            {
                for (int y = rect.y; y < rect.getMaxY(); y++)
                {
                    if (new IPoint(x, y).equal(exceptPoint))
                    {
                        continue;
                    }
                    Block block = BlocksEngine.instance.getBlock(x, y);
                    if (Builder.IS_Can_Cover)
                    {
                        return false;
                    }
                    else
                    {
                        if (block != null)
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        protected virtual void onLargeBlockInitFinish()
        {

        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            reCalculateBlocksCount();
        }

        /// <summary>
        ///输入偏移值，返回实际世界坐标
        /// </summary>
        public IPoint getReviseBlockCoor(IPoint normal_offset)
        {
            return getOriginPoint() + normal_offset;
        }

        /// <summary>
        ///获取原点方块，如果原点方块已经不存在，则返回null
        /// </summary>
        public LargeBlock getOrgBlock()
        {
            if (getIsBroken())
            {
                return null;
            }
            else
            {
                Block block = BlocksEngine.instance.getBlock(getOriginPoint());
                if (block != null && block.equalBlock(this))
                {
                    return block as LargeBlock;
                }
            }
            return null;
        }

        void setRemoveTag(bool tag)
        {
            removeTag = tag;
        }

        public IPoint getOriginPoint()
        {
            return getCoor() - getOffset();
        }

        public IPoint getOffset()
        {
            return m_offset;
        }

        public void setOffset(IPoint offset)
        {
            this.m_offset = offset;
            setTextureRect(offset);
            m_orgPoint = getCoor() - offset;
        }

        protected void setTextureRect(IPoint offset)
        {
            setSpriteRect(((size.y - 1) - offset.y) * size.x + offset.x);
        }

        public void setIsBroken(bool isBroken)
        {
            if (isOrigin())
            {
                m_isBroken = isBroken;
            }
            else
            {
                LargeBlock orgBlock = BlocksEngine.instance.getBlock(getOriginPoint()) as LargeBlock;
                if (orgBlock != null && orgBlock.equalBlock(this))
                {
                    orgBlock.setIsBroken(isBroken);
                }
            }

        }

        public bool getIsBroken()
        {
            if (isOrigin())
            {
                return m_isBroken;
            }
            else
            {
                LargeBlock orgBlock = BlocksEngine.instance.getBlock(getOriginPoint()) as LargeBlock;
                if (orgBlock != null && orgBlock.equalBlock(this))
                {
                    return orgBlock.getIsBroken();
                }
            }
            return true;
        }

        public void reCalculateBlocksCount()
        {
            if (isOrigin())
            {
                blockCount = 0;
                IPoint origin = getCoor();
                for (int offsetx = 0; offsetx < size.x; offsetx++)
                {
                    for (int offsety = 0; offsety < size.y; offsety++)
                    {
                        IPoint obOffset = new IPoint(offsetx, offsety);
                        if (obOffset.equal(m_offset))
                        {
                            blockCount++;
                            continue;
                        }
                        LargeBlock offsetBlock = BlocksEngine.instance.getBlock(origin + obOffset) as LargeBlock;
                        if (offsetBlock != null && offsetBlock.equalBlock(this) && offsetBlock.getOffset().equal(obOffset))
                        {
                            blockCount++;
                        }
                    }
                }
            }
        }

        public bool isOrigin()
        {
            return m_offset.isZeroPoint();
        }


        public override int getCurrentBindId()
        {
            if (isOrigin())
            {
                return currentBindId;
            }
            else
            {
                LargeBlock orgBlock = getOrgBlock() as LargeBlock;
                if (orgBlock != null && orgBlock.equalBlock(this))
                {
                    return orgBlock.getCurrentBindId();
                }
            }
            return 0;

        }

        public override void setBindId(int id)
        {
            if (isOrigin())
            {
                currentBindId = id;
            }
            else
            {
                Block block = BlocksEngine.instance.getBlock(getOriginPoint());
                if (block != null && block.equalBlock(this))
                {
                    LargeBlock orgBlock = block as LargeBlock;
                    orgBlock.currentBindId = id;
                }
            }
        }

        public override void setSettingValue(int v)
        {
            if (isOrigin())
            {
                base.setSettingValue(v);
            }
            else
            {
                Block block = BlocksEngine.instance.getBlock(getOriginPoint());
                if (block != null && block.equalBlock(this))
                {
                    LargeBlock orgBlock = block as LargeBlock;
                    orgBlock.setSettingValue(v);
                }
            }
        }

        public override int getCurrentSettingValue()
        {
            if (isOrigin())
            {
                return currentSettingValue;
            }
            else
            {
                LargeBlock orgBlock = getOrgBlock() as LargeBlock;
                if (orgBlock != null && orgBlock.equalBlock(this))
                {
                    return orgBlock.getCurrentSettingValue();
                }
            }
            return 0;
        }

        public override void onWorldModeDestroy()
        {
            LargeBlock orgBlock = getOrgBlock();
            if (orgBlock != null)
            {
                orgBlock.decBlockCount();
                orgBlock.setIsBroken(true);
            }

        }

        public int getBlockCount()
        {
            if (isOrigin())
            {
                return blockCount;
            }
            else
            {
                LargeBlock block = getOrgBlock();
                if (block != null)
                {
                    return block.getBlockCount();
                }
            }
            return 0;
        }

        public void decBlockCount()
        {
            if (isOrigin())
            {
                blockCount--;
            }
            else
            {
                LargeBlock block = getOrgBlock();
                if (block != null)
                {
                    block.decBlockCount();
                }
            }
        }

        protected float getEfficiency()
        {
            return (float)getBlockCount() / (float)(size.x * size.y);
        }

        public override string getBasicInformation()
        {
            return base.getBasicInformation()
                + "," + ILang.get("Mass", "menu") + ":" + getMass() * blockCount;
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);

            IUtils.keyValue2Writer(writer, "ib", getIsBroken());
            IUtils.keyValue2Writer(writer, "io", isOrigin());
            IUtils.keyValue2Writer(writer, "ox", m_offset.x);
            IUtils.keyValue2Writer(writer, "oy", m_offset.y);
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);

            m_isOrigin = IUtils.getJsonValue2Bool(blockData, "io");
            setIsBroken(IUtils.getJsonValue2Bool(blockData, "ib"));

            if (getIsBroken() == false)
            {
                setOffset(new IPoint(IUtils.getJsonValue2Int(blockData, "ox"), IUtils.getJsonValue2Int(blockData, "oy")));
                onLargeBlockInitFinish();
                blockCount = size.x * size.y;
            }
        }

        public IPoint getSize()
        {
            return size;
        }

        public override int getTotalPrice()
        {
            return price * blockCount;
        }    
    }
    

}
