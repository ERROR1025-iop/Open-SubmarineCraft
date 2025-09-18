using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TapSDK.Login;
namespace Scraft
{
    public class MainMenu : MonoBehaviour
    {

        public RectTransform buttonsRectTransform;

        public float speed;

        public Button startGameButton;
        public Button tutorialButton;
        public Button settingButton;
        public Button modButton;
        public Button exitGameButton;
        public Button blueprintButton;
        public Button assemblerButton;
        public Button researchButton;
        public Button stationsButton;
        public Button craftsButton;
        public Button shopButton;
        public Button shipsDownloadButton;
        public Button BackButton;

        static Vector2 buttonsRectTarget;

        IToast toast;

        private void Awake()
        {
            Time.timeScale = 1;
            GameSetting.init();
            if (!ILang.isLoaded)
            {
                ILang.loadLangData();
            }
            toast = IToast.instance;
        }

        void Start()
        {
            startGameButton.onClick.AddListener(onStartGameButtonClick);
            tutorialButton.onClick.AddListener(onTutorialButtonClick);
            settingButton.onClick.AddListener(onSettingButtonClick);
            modButton.onClick.AddListener(onModButtonClick);
            exitGameButton.onClick.AddListener(onExitGameButtonClick);
            blueprintButton.onClick.AddListener(onBlueprintButtonClick);
            assemblerButton.onClick.AddListener(onAssemblerButtonClick);
            researchButton.onClick.AddListener(onResearchButtonClick);
            stationsButton.onClick.AddListener(onStationsButtonClick);
            craftsButton.onClick.AddListener(onCraftsButtonClick);
            shopButton.onClick.AddListener(onShopButtonClick);
            shipsDownloadButton.onClick.AddListener(onShipsDownloadButtonClick);
            BackButton.onClick.AddListener(onBackButtonClick);

            craftsButton.gameObject.SetActive(ISecretLoad.isFileExit(ISecretLoad.shipsInfoNamePath));
            stationsButton.gameObject.SetActive(ISecretLoad.isFileExit(ISecretLoad.stationInfoNamePath));
            shopButton.gameObject.SetActive(ISecretLoad.isFileExit(ISecretLoad.stationInfoNamePath));

            onBackButtonClick();

            //initSDPATH("/mnt/sdcard");
        }

        void onStartGameButtonClick()
        {
            if (GameSetting.isViewTutorial)
            {
                buttonsRectTarget = Vector2.zero;
            }
            else
            {
                IConfigBox.instance.show(ILang.get("view tutorial"), onViewTutorialComfirm, onViewTutorialCancel);
            }                    
        }

        void onViewTutorialComfirm()
        {
            onTutorialButtonClick();
        }

        void onViewTutorialCancel()
        {
            buttonsRectTarget = Vector2.zero;
        }

        void onSettingButtonClick()
        {
            toast.show("Loading");
            SceneManager.LoadScene("Setting");
        }

        void onModButtonClick()
        {
            toast.show("Loading");
            SceneManager.LoadScene("Mod");
        }

        void onTutorialButtonClick()
        {
            toast.show("Loading");
            SceneManager.LoadScene("Tutorial");
        }

        void onExitGameButtonClick()
        {
            Application.Quit();
        }

        void onBlueprintButtonClick()
        {
            toast.show("Loading");
            Builder.IS_LOAD_LAST = false;
            SceneManager.LoadScene("Builder");
        }

        void onAssemblerButtonClick()
        {
            toast.show("Loading");
            Assembler.IS_LOAD_LAST = false;
            Assembler.IS_FORM_POOLER = false;
            SceneManager.LoadScene("Assembler");
        }

        void onResearchButtonClick()
        {
            toast.show("Loading");
            SceneManager.LoadScene("Research");
        }

        void onCraftsButtonClick()
        {
            if(ISecretLoad.isFileExit(ISecretLoad.shipsInfoNamePath))
            {
                toast.show("Loading");
                SceneManager.LoadScene("Crafts");
            }
            else
            {
                toast.show("No access", 100);
            }    
        }

        void onStationsButtonClick()
        {          

            if (ISecretLoad.isFileExit(ISecretLoad.stationInfoNamePath))
            {
                toast.show("Loading");
                SceneManager.LoadScene("Stations");
            }
            else
            {
                toast.show("No access", 100);
            }
        }

        void onShopButtonClick()
        {
            if (ISecretLoad.isFileExit(ISecretLoad.stationInfoNamePath))
            {
                toast.show("Loading");
                SceneManager.LoadScene("Shop");
            }
            else
            {
                toast.show("No access", 100);
            }       
        }

        void onShipsDownloadButtonClick()
        {
            UnityAndroidEnter.CallSavesMain();
        }

        void onBackButtonClick()
        {
            buttonsRectTarget = new Vector2(0, 227);            
        }

        void Update()
        {
            buttonsRectTransform.anchoredPosition = Vector2.MoveTowards(buttonsRectTransform.anchoredPosition, buttonsRectTarget, Time.deltaTime * speed);
        }
    }
}