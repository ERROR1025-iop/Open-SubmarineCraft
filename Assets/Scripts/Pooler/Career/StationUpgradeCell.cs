using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;

namespace Scraft.StationSpace
{
    public class StationUpgradeCell : IGridScrollViewCell
    {
        public ComponentInfo componentInfo;       
        Image thumbnailImage;
        Text name;
        Text material;
        Text storeSolid;
        Text storeLiquild;
        Text storePower;

        public StationUpgradeCell(IGridScrollViewInfo gridScrollViewInfo) : base(gridScrollViewInfo)
        {          
            thumbnailImage = rectTransform.GetChild(0).GetComponent<Image>();
            name = rectTransform.GetChild(1).GetComponent<Text>();
            material = rectTransform.GetChild(3).GetComponent<Text>();
            storeSolid = rectTransform.GetChild(4).GetComponent<Text>();
            storeLiquild = rectTransform.GetChild(5).GetComponent<Text>();
            storePower = rectTransform.GetChild(6).GetComponent<Text>();

            clearInformation();
        }

        public override void setInformation<T>(T info)
        {
            base.setInformation(info);
            componentInfo = info as ComponentInfo;
            thumbnailImage.enabled = true;
            thumbnailImage.sprite = componentInfo.thumbnailSprite;
            name.text = componentInfo.name;
            storeSolid.text = componentInfo.canStoreSoild.ToString();
            storeLiquild.text = componentInfo.canStoreLiquid.ToString();
            storePower.text = componentInfo.canStorePower.ToString();
            string materialStr = "";
            for(int i=0;i< componentInfo.counts.Length; i++)
            {
                materialStr += string.Format("{0} x {1}\n", componentInfo.blocks[i].getLangName(), componentInfo.counts[i]);
            }
            material.text = materialStr;
        }              
    }


}
