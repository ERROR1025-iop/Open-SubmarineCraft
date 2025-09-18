using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class CraftListCell : IGridScrollViewCell
    {

        Text name;
        CraftInfo craftInfo;

        public CraftListCell(IGridScrollViewInfo gridScrollViewInfo) : base(gridScrollViewInfo)
        {           
            name = rectTransform.GetChild(0).GetComponent<Text>();         
            clearInformation();
        }

        public override void setInformation<T>(T info)
        {
            base.setInformation(info);
            craftInfo = info as CraftInfo;
            name.text = craftInfo.realName;
        }

        public CraftInfo getCraftInfo()
        {
            return craftInfo;
        }        
    }
}
