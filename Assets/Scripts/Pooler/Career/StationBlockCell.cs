using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;

namespace Scraft.StationSpace
{
    public class StationBlockCell : IGridScrollViewCell
    {
        BlockCellInfo blockCellInfo;
        Image iconImage;
        Text countText;

        public StationBlockCell(IGridScrollViewInfo gridScrollViewInfo) : base(gridScrollViewInfo)
        {
            iconImage = rectTransform.GetChild(0).GetComponent<Image>();
            countText = rectTransform.GetChild(1).GetComponent<Text>();            

            clearInformation();
        }

        public override void setInformation<T>(T info)
        {
            base.setInformation(info);
            blockCellInfo = info as BlockCellInfo;
            iconImage.sprite = blockCellInfo.block.getSyntIconSprite();
            countText.text = blockCellInfo.count.ToString();
        }

        public Block getBlock()
        {
            return blockCellInfo.block;
        }
    }

    public class BlockCellInfo
    {
        public Block block;
        public int count;

        public BlockCellInfo(Block block, int count)
        {
            this.block = block;
            this.count = count;
        }
    }
}
