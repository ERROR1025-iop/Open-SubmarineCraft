using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.StationSpace;
using Scraft.BlockSpace;

namespace Scraft
{
    public class StationsListCell : IGridScrollViewCell
    {

        Text name;
        StationInfo stationInfo;
        Image icon;

        public StationsListCell(IGridScrollViewInfo gridScrollViewInfo) : base(gridScrollViewInfo)
        {
            name = rectTransform.GetChild(0).GetComponent<Text>();
            icon = rectTransform.GetChild(1).GetComponent<Image>();
            clearInformation();
        }

        public override void setInformation<T>(T info)
        {
            base.setInformation(info);
            stationInfo = info as StationInfo;
            name.text = stationInfo.name;
            icon.gameObject.SetActive(stationInfo.isMainStation());
        }

        public StationInfo getStationInfo()
        {
            return stationInfo;
        }
    }
}
