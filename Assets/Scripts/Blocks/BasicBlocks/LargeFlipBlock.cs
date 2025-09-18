using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{

    public class LargeFlipBlock : LargeBlock
    {
        int flipStartSprite;
        int m_startSprite;
        int showSpriteIndex;
        
        protected bool isFlip;

        public LargeFlipBlock(int id, GameObject parentObject, GameObject blockObject)
           : base(id, parentObject, blockObject)
        {
            isFlip = false;
        }

        public void initFlipLargeBlock(int flipStartSprite)
        {
            this.flipStartSprite = flipStartSprite;          
        }

        public override void onRotateButtonClick()
        {
            if (isOrigin())
            {
                for (int offsetx = 0; offsetx < size.x; offsetx++)
                {
                    for (int offsety = 0; offsety < size.y; offsety++)
                    {
                        IPoint obOffset = new IPoint(offsetx, offsety);
                        LargeFlipBlock offsetBlock = BlocksEngine.instance.getBlock(getOriginPoint() + obOffset) as LargeFlipBlock;
                        if (offsetBlock != null && offsetBlock.equalBlock(this) && offsetBlock.getOffset().equal(obOffset))
                        {
                            offsetBlock.setFlip(!offsetBlock.isFlip);
                        }
                    }
                }
            }
            else
            {
                LargeBlock orgBlock = getOrgBlock() as LargeBlock;
                if (orgBlock != null && orgBlock.equalBlock(this))
                {
                    orgBlock.onRotateButtonClick();
                }
            }
               
        }

        public void setFlip(bool isFlip)
        {
            this.isFlip = isFlip;
            m_startSprite = isFlip ? flipStartSprite : 0;
            updateSprites();
        }

        public void updateSprites()
        {
            base.setSpriteRect(m_startSprite + showSpriteIndex);
        }

        protected override void setSpriteRect(int index)
        {
            showSpriteIndex = index;
            base.setSpriteRect(m_startSprite + index);
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);
            IUtils.keyValue2Writer(writer, "isFlip", isFlip);
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            setFlip(IUtils.getJsonValue2Bool(blockData, "isFlip"));
        }


        public override bool isCanRotate()
        {
            return true;
        }
    }
}
