using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class ModSence : MonoBehaviour
    {

        public IGridScrollView modsScrollView;
        public Image previewImage;
        public Text infoText;
        public Text describeText;
        public Text offButtonText;
        public Button offButton;

        List<ModCellInfo> modCellInfos;
        ModSenceCell selectedModCell;
        ModCellInfo selectedModInfo;

        bool isChanged;    

        void Start()
        {
            GameObject.Find("Canvas/Back").GetComponent<Button>().onClick.AddListener(onBackButtonClick);

            if(ModLoader.modInfos != null && ModLoader.modInfos.Count > 0)
            {
                modCellInfos = new List<ModCellInfo>();
                List<ModInfo> modInfos = new List<ModInfo>(ModLoader.modInfos.Values);
                List<ModConfig> modConfigs = new List<ModConfig>(ModLoader.modConfigs.Values);
                for (int i=0;i< modInfos.Count; i++)
                {

                    modCellInfos.Add(new ModCellInfo(modInfos[i], modConfigs[i]));
                }
            }

            if (modCellInfos != null)
            {
                modsScrollView.initialized(OnCreateModListCells);
                modsScrollView.setInformation(modCellInfos);
                modsScrollView.OnCellClick += onModListCellClick;

                infoText.text = string.Format("{0}{1}", ILang.get("mod.count"), modCellInfos.Count.ToString());
            }
            

            offButton.onClick.AddListener(onOffButtonClick);          

            isChanged = false;           
        }

        IGridScrollViewCell OnCreateModListCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new ModSenceCell(gridScrollViewInfo);
        }

        void onModListCellClick(IGridScrollViewCell cell)
        {
            
            selectedModCell = cell as ModSenceCell;
            selectedModInfo = selectedModCell.getModCellInfo();            

            string info = string.Format("{0}{1}\n{2}{3}\n{4}{5}",
                ILang.get("mod.author"), selectedModInfo.modInfo.author,
                ILang.get("mod.mod_version"), selectedModInfo.modInfo.version,
                ILang.get("mod.game_version"), selectedModInfo.modInfo.game_version);
            infoText.text = info;

            describeText.text = selectedModInfo.modInfo.describe;

            offButtonText.text = ILang.get(selectedModInfo.modConfig.isActivited ? "mod.Off": "mod.Activite");

            Texture2D texture2D;
            string previewPath = string.Format("{0}{1}/preview.png", GamePath.modFolder, selectedModInfo.modInfo.name);
            try
            {
                texture2D = IUtils.loadTexture2DFromSD(previewPath);
                previewImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            }
            catch
            {

            }
        }
        

        void onOffButtonClick()
        {
            if(selectedModCell != null)
            {
                selectedModInfo.modConfig.isActivited = !selectedModInfo.modConfig.isActivited;
                selectedModInfo.modConfig.saveFile();
                selectedModCell.setActivatedText(selectedModInfo.modConfig.isActivited);
                isChanged = true;
                offButtonText.text = ILang.get(selectedModInfo.modConfig.isActivited ? "mod.Off" : "mod.Activite");
            }           
        }  


        void onBackButtonClick()
        {
            if (isChanged)
            {
                ModLoader.reload();
            }
            Application.LoadLevel("Menu");
        }              

    }
}
