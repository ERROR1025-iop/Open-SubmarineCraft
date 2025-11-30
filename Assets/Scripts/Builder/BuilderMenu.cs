using LitJson;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class BuilderMenu
    {

        RectTransform rectTrans;

        Builder builder;

        Text SettingText;

        public BuilderMenu()
        {
            builder = Builder.instance;

            rectTrans = GameObject.Find("Canvas/menu").GetComponent<RectTransform>();
            GameObject.Find("Canvas/menu/Resume game").GetComponent<Button>().onClick.AddListener(onResumeGameButtonClick);
            GameObject.Find("Canvas/menu/Clear map").GetComponent<Button>().onClick.AddListener(onClearMapButtonClick);
            GameObject.Find("Canvas/menu/Share").GetComponent<Button>().onClick.AddListener(onShareButtonClick);
            GameObject.Find("Canvas/menu/Assembler").GetComponent<Button>().onClick.AddListener(onAssemblerButtonClick);
            GameObject.Find("Canvas/menu/Open wiki").GetComponent<Button>().onClick.AddListener(onOpenWikiButtonClick);
            GameObject.Find("Canvas/menu/Setting").GetComponent<Button>().onClick.AddListener(onSettingButtonClick);
            GameObject.Find("Canvas/menu/Back to menu").GetComponent<Button>().onClick.AddListener(onBackToMenuButtonClick);

            SettingText = GameObject.Find("Canvas/menu/Setting/Text").GetComponent<Text>();
            SettingText.text = ILang.get("Setting", "menu");

            show(false);
        }

        public void show(bool show)
        {
            if (show)
            {
                rectTrans.anchoredPosition = new Vector2(0, 0);
            }
            else
            {
                rectTrans.anchoredPosition = new Vector2(1000, 0);
            }
        }

        void onResumeGameButtonClick()
        {
            show(false);
        }

        void onClearMapButtonClick()
        {
            builder.clearMap();
            show(false);
        }

        void onShareButtonClick()
        {
            string shipData = builder.saveBlocks(GamePath.cacheFolder, "upload.cache");
            Texture2D texture2D = builder.createThumbnailTexture2D(JsonMapper.ToObject(shipData));
            byte[] png = texture2D.EncodeToPNG();
            File.WriteAllBytes(GamePath.cacheFolder + "thumbnail.cache", png);
            ShipUploader shipUploader = ShipUploader.instance;
            shipUploader.isAssembler = false;
            shipUploader.titleInput.text = World.mapName;
            shipUploader.SetCover(GamePath.cacheFolder + "thumbnail.cache");
            shipUploader.Show();
            show(false);
            //UnityAndroidEnter.CallUploadShip(World.mapName, false);
            //AlertBox.ShowNotImplemented();


        }

        void onAssemblerButtonClick()
        {
            IToast.instance.show("Loading");
            Assembler.IS_FORM_POOLER = false;
            Assembler.IS_LOAD_LAST = false;
            Application.LoadLevel("Assembler");
        }

        void onOpenWikiButtonClick()
        {
            //AlertBox.ShowNotImplemented();
            Tutorial.OpenWikiWeb();
            show(false);
        }

        void onSettingButtonClick()
        {
            builder.builderSetting.show(true);
            show(false);
        }

        void onBackToMenuButtonClick()
        {
            IToast.instance.show("Loading");
            Application.LoadLevel("menu");
        }
    }
}