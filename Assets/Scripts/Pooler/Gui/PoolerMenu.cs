using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;
using Scraft.StationSpace;
using UnityEngine.SceneManagement;

namespace Scraft
{
    public class PoolerMenu
    {
        static public PoolerMenu instance;

        RectTransform rectTrans;
        Pooler pooler;

        public PoolerMenu(Pooler pooler)
        {
            instance = this;
            this.pooler = pooler;

            rectTrans = GameObject.Find("Canvas/menu").GetComponent<RectTransform>();
            GameObject.Find("Canvas/menu/Resume game").GetComponent<Button>().onClick.AddListener(onResumeGameButtonClick);
            GameObject.Find("Canvas/menu/Revert to launch").GetComponent<Button>().onClick.AddListener(onRevertLaunchButtonClick);
            GameObject.Find("Canvas/menu/Revert to blueprint").GetComponent<Button>().onClick.AddListener(onRevertBlueprintButtonClick);
            GameObject.Find("Canvas/menu/Revert to assembler").GetComponent<Button>().onClick.AddListener(onRevertAssemblerButtonClick);
            GameObject.Find("Canvas/menu/Back to menu").GetComponent<Button>().onClick.AddListener(onBackToMenuButtonClick);
            GameObject.Find("Canvas/menu/Build station").GetComponent<Button>().onClick.AddListener(onBuildStationButtonClick);
            GameObject.Find("Canvas/menu/Render setting").GetComponent<Button>().onClick.AddListener(onRenderSettingButtonClick);

            show(false);
        }

        public void show(bool show)
        {
            if (show)
            {
                World.stopUpdata = true;
                Time.timeScale = 0;
                Pooler.isGlobalShowIcon = false;
                rectTrans.anchoredPosition = new Vector2(0, 0);
            }
            else
            {
                World.stopUpdata = false;
                Time.timeScale = 1;
                Pooler.isGlobalShowIcon = true;
                rectTrans.anchoredPosition = new Vector2(1000, 0);
            }
        }

        void onResumeGameButtonClick()
        {
            show(false);
        }

        void onRevertLaunchButtonClick()
        {
            revertLaunch();
        }

        void revertLaunch()
        {
            Pooler.isRunThread = false;
            IToast.instance.show("Loading");
            string backgroundPath = string.Format("Menu/Loading/{0}{1}", GameSetting.isCreateAi ? "n" : "s", (int)(Random.value * 2.9f));
            AsyncLoadScene.sprite = Resources.Load(backgroundPath, typeof(Sprite)) as Sprite;
            UnityAndroidEnter.CallShowInterstitialAD();           
            SceneManager.LoadScene("Pooler");
        }


        public void onRevertBlueprintButtonClick()
        {
            if (ITutorial.tutorialStep >= 0)
            {
                ITutorial.tutorialStep++;
            }
            Pooler.isRunThread = false;
            IToast.instance.show("Loading");
            Builder.IS_LOAD_LAST = true;
            UnityAndroidEnter.CallShowInterstitialAD();
            SceneManager.LoadScene("Builder");
        }

        void onRevertAssemblerButtonClick()
        {
            Pooler.isRunThread = false;
            IToast.instance.show("Loading");
            Assembler.IS_LOAD_LAST = true;
            Assembler.IS_FORM_POOLER = true;
            UnityAndroidEnter.CallShowInterstitialAD();
            SceneManager.LoadScene("Assembler");
        }      

        void onBackToMenuButtonClick()
        {
            if (GameSetting.isCareer)
            {
                if (Station.mainStation != null)
                {
                    Pooler.isRunThread = false;
                    IToast.instance.show("Loading");
                    Pooler.instance.savePoolerData(false);
                    SceneManager.LoadScene("Menu");
                }
            }
            else
            {
                Pooler.isRunThread = false;
                IToast.instance.show("Loading");
                SceneManager.LoadScene("Menu");
            }
           
        }

        void onBuildStationButtonClick()
        {
            if (MainSubmarine.isRunSurface)
            {
                string title = ILang.get("build station");
                IConfigBox.instance.show(title, onBuildStationConfirmButtonClick, null);     
            }
            else
            {
                IToast.instance.show("Please float to the surface first.", 100);
            }           
            show(false);
        }

        void onBuildStationConfirmButtonClick()
        {
            BlocksManager bm = BlocksManager.instance;
            Block[] blocks = new Block[3] { bm.stone, bm.wood, bm.steel};
            int[] counts = new int[3] { 150, 30 , 50}; //石材x150，木头x30，钢x50
            IPoint[] coors;
            //Pooler.instance.getCargos(blocks, counts, out coors, false);
            //if(true)
            if (Pooler.instance.getCargos(blocks, counts, out coors, false) || !GameSetting.isCareer || Application.isEditor)
            {
                Station station = (Object.Instantiate(Resources.Load("Stations/Prefabs/station 01")) as GameObject).GetComponent<Station>();
                station.setIsNewPlace();
                Place3DItem place3DItem = station.gameObject.AddComponent<Place3DItem>();
                place3DItem.setMaterialBlocksCoor(GameSetting.isCareer ? coors : null);
                place3DItem.setStation(station);
                place3DItem.OnPlaceFinish += station.OnPlaceFinish;
            }
            else
            {
                IToast.instance.show("Inadequate blocks.", 100);
            }
        }

        void onRenderSettingButtonClick()
        {
            RenderSetting.instance.show(true);
            show(false);
        }


    }
}