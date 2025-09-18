using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.StationSpace;
using Scraft.BlockSpace;

namespace Scraft
{
    public class ShopCell : IGridScrollViewCell
    {

        Image icon;
        Text name;       
        Text price;

        ShopCellInfo shopCellInfo;

        public ShopCell(IGridScrollViewInfo gridScrollViewInfo) : base(gridScrollViewInfo)
        {            
            icon = rectTransform.GetChild(0).GetComponent<Image>();
            name = rectTransform.GetChild(1).GetComponent<Text>();
            price = rectTransform.GetChild(2).GetComponent<Text>();
            clearInformation();
        }

        public override void setInformation<T>(T info)
        {
            base.setInformation(info);
            shopCellInfo = info as ShopCellInfo;
            name.text = shopCellInfo.block.getLangName();
            icon.sprite = shopCellInfo.block.getSyntIconSprite();
            price.text = shopCellInfo.price.ToString("f0");
        }

        public ShopCellInfo getShopCellInfo()
        {
            return shopCellInfo;
        }      
    }

    public class ShopCellInfo
    {
        public Block block;
        public float price;

        public ShopCellInfo(Block block, float price)
        {
            this.block = block;
            this.price = price;
        }
    }
}
