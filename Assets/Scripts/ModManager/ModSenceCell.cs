using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class ModSenceCell : IGridScrollViewCell
    {

        Text nameText;
        Text authorText;
        Text activatedText;
        ModCellInfo modInfo;

        public ModSenceCell(IGridScrollViewInfo gridScrollViewInfo) : base(gridScrollViewInfo)
        {
            nameText = rectTransform.GetChild(0).GetComponent<Text>();
            authorText = rectTransform.GetChild(1).GetComponent<Text>();
            activatedText = rectTransform.GetChild(2).GetComponent<Text>();
            clearInformation();
        }

        public override void setInformation<T>(T info)
        {
            base.setInformation(info);
            modInfo = info as ModCellInfo;
            nameText.text = modInfo.modInfo.name;
            authorText.text = modInfo.modInfo.author;
            setActivatedText(modInfo.modConfig.isActivited);
        }

        public void setActivatedText(bool isActivited)
        {
            activatedText.text = ILang.get(isActivited ? "Activited" : "mod.Off");
            activatedText.color = isActivited ? Color.green : Color.gray;
        }

        public ModCellInfo getModCellInfo()
        {
            return modInfo;
        }
    }

    public class ModCellInfo
    {
        public ModInfo modInfo;
        public ModConfig modConfig;

        public ModCellInfo(ModInfo modInfo, ModConfig modConfig)
        {
            this.modInfo = modInfo;
            this.modConfig = modConfig;
        }
    }
}
