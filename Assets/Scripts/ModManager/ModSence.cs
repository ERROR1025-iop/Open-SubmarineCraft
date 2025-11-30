using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scraft;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public Button moreButton;
        public Button uploadButton;

        List<ModCellInfo> modCellInfos;
        ModSenceCell selectedModCell;
        ModCellInfo selectedModInfo;

        bool isChanged;

        void Start()
        {
            GamePath.init();
            GameObject.Find("Canvas/Back").GetComponent<Button>().onClick.AddListener(onBackButtonClick);

            if (ModLoader.modInfos != null && ModLoader.modInfos.Count > 0)
            {
                modCellInfos = new List<ModCellInfo>();
                List<ModInfo> modInfos = new List<ModInfo>(ModLoader.modInfos.Values);
                List<ModConfig> modConfigs = new List<ModConfig>(ModLoader.modConfigs.Values);
                for (int i = 0; i < modInfos.Count; i++)
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
            moreButton.onClick.AddListener(onMoreButtonClick);
            uploadButton.onClick.AddListener(onUploadButtonClickAsync);

            isChanged = false;

            if (GameSetting.isAndroid && GameSetting.gameBackend == "il2cpp")
            {
                AlertBox.instance.Show("该版本不支持MOD，加Q群获取支持MOD的版本");
            }
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

            offButtonText.text = ILang.get(selectedModInfo.modConfig.isActivited ? "mod.Off" : "mod.Activite");

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
            if (selectedModCell != null)
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
            SceneManager.LoadScene("Menu");
        }

        void onMoreButtonClick()
        {

        }

        async void onUploadButtonClickAsync()
        {
            if (LoginHandle.userData == null || LoginHandle.userData.token1 == null)
            {
                AlertBox.instance.Show("请先登录\nPlease log in first");
                return;
            }
            string zipPath = selectedModInfo.modConfig.GetZipPath();
            IUtils.ZipFolder(selectedModInfo.modConfig.GetFolderPath(), zipPath);
            if (ISecretLoad.isFileExit(zipPath))
            {
                string zipData = Base64Helper.FileToBase64String(zipPath);
                string version = selectedModInfo.modInfo.version;
                var request = new HttpRequest(NetworkFactory.SCRAFT_HOST + "/ship/download");
                request.addFormData("token1", LoginHandle.userData.token1);
                IToast.instance.show("Downloading...");
                HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
                IToast.instance.hide();
                if (response.code == 200)
                {
                    
                }
            }
        }

    }
}
