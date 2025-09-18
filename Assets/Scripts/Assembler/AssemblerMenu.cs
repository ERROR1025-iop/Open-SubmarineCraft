using UnityEngine;
using UnityEngine.UI;
using Scraft.DpartSpace;

namespace Scraft
{
    public class AssemblerMenu
    {
        RectTransform rectTrans;

        Assembler assembler;

        public AssemblerMenu()
        {
            assembler = Assembler.instance;

            rectTrans = GameObject.Find("Canvas/menu").GetComponent<RectTransform>();
            GameObject.Find("Canvas/menu/Resume game").GetComponent<Button>().onClick.AddListener(onResumeGameButtonClick);
            GameObject.Find("Canvas/menu/Clear map").GetComponent<Button>().onClick.AddListener(onClearMapButtonClick);
            GameObject.Find("Canvas/menu/Share").GetComponent<Button>().onClick.AddListener(onShareButtonClick);
            GameObject.Find("Canvas/menu/Goto builder").GetComponent<Button>().onClick.AddListener(onGotoBuilderButtonClick);
            GameObject.Find("Canvas/menu/Open wiki").GetComponent<Button>().onClick.AddListener(onOpenWikiButtonClick);
            GameObject.Find("Canvas/menu/Setting").GetComponent<Button>().onClick.AddListener(onSettingButtonClick);
            GameObject.Find("Canvas/menu/Export Obj").GetComponent<Button>().onClick.AddListener(onExportObjButtonClick);
            GameObject.Find("Canvas/menu/Back to menu").GetComponent<Button>().onClick.AddListener(onBackToMenuButtonClick);


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
            Assembler.instance.setPlaneOffset(0);
            Assembler.dpartsEngine.removeAllDpart();
            AssemblerForceArrow.propellers.Clear();
            show(false);
        }

        void onShareButtonClick()
        {
            string shipData = assembler.saveDpart(GamePath.cacheFolder, "upload.cache");
            AssemblerUtils.createGameObjectThumbnailImage(World.dpartParentObject, assembler.shotCamera, GamePath.cacheFolder + "thumbnail.cache", new Rect(0, 0, 800, 480));
            UnityAndroidEnter.CallUploadShip(World.dpartName, true);
        }

        void onGotoBuilderButtonClick()
        {
            IToast.instance.show("Loading");
            Builder.IS_LOAD_LAST = false;
            Application.LoadLevel("builder");
        }

        void onOpenWikiButtonClick()
        {
            UnityAndroidEnter.CallWiki();
        }

        void onSettingButtonClick()
        {
            assembler.assemblerSetting.show(true);
            show(false);
        }

        void onExportObjButtonClick()
        {
            show(false);
            AssExportObj.Start(World.dpartParentObject, World.dpartName);
        }

        void onBackToMenuButtonClick()
        {
            Application.LoadLevel("menu");
        }
    }
}