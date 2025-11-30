using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;

namespace Scraft.StationSpace
{
    public class SyntCell : IGridScrollViewCell
    {
        public Block block;
        public SyntInfo[] syntInfos;

        Image icon;
        Text name;
        

        public SyntCell(IGridScrollViewInfo gridScrollViewInfo) : base(gridScrollViewInfo)
        {
            icon = rectTransform.GetChild(0).GetComponent<Image>();
            name = rectTransform.GetChild(1).GetComponent<Text>();

            clearInformation();
        }

        public override void setInformation<T>(T info)
        {
            base.setInformation(info);
            block = info as Block;

            icon.sprite = block.getSyntIconSprite();
            name.text = block.getLangName();

            syntInfos = block.getSyntInfo(BlocksManager.instance);
        }       
    }   
}
