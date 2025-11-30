using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Scraft {
    public class Tutorial : MonoBehaviour
    {
        public Button textTutorialButton;

        public Button interfaceOperationButton;
        public Button quickLearnButton;
        public Button steamEngineButton;
        public Button miningButton;

        public Button obtainScientificButton;
        public Button oreSmeltingButton;
        public Button electricPowerButton;       
        public Button nuclearFuelButton;
        public Button circuitFoundationButton;
        public Button craneButton;
        public Button construction3DButton;
        public Button useOfTurretsButton;


        void Start()
        {
            textTutorialButton.onClick.AddListener(onTextTutorialButtonClick);
            interfaceOperationButton.onClick.AddListener(onInterfaceOperationButtonClick);
            quickLearnButton.onClick.AddListener(onQuickLearnButtonClick);
            steamEngineButton.onClick.AddListener(onSteamEngineButtonClick);
            miningButton.onClick.AddListener(onMiningButtonClick);

            obtainScientificButton.onClick.AddListener(onObtainScientificButtonClick);
            oreSmeltingButton.onClick.AddListener(onOreSmeltingButtonClick);
            electricPowerButton.onClick.AddListener(onElectricPowerButtonClick);
            nuclearFuelButton.onClick.AddListener(onNuclearFuelButtonClick);
            circuitFoundationButton.onClick.AddListener(onCircuitFoundationButtonClick);
            craneButton.onClick.AddListener(onCraneButtonClick);
            construction3DButton.onClick.AddListener(onConstruction3DButtonClick);
            useOfTurretsButton.onClick.AddListener(onUseOfTurretsButtonClick);

            GameObject.Find("Canvas/Back").GetComponent<Button>().onClick.AddListener(onBackButtonClick);

            if (GameSetting.isViewTutorial == false)
            {
                GameSetting.isViewTutorial = true;
                GameSetting.save();
            }

        }
        
        public static void OpenWikiWeb()
        {
            if(GameSetting.lang == 2)
            {
                Application.OpenURL("https://scraft.aipie.cool");
            }
            else
            {
                Application.OpenURL("https://scraft.aipie.cool?lang=en");
            }
        }

        void onTextTutorialButtonClick()
        {
            OpenWikiWeb();
            //AlertBox.ShowNotImplemented();            
        }

        void onInterfaceOperationButtonClick()
        {
            ITutorial.start("Builder", false, "InterfaceOperation");
        }

        void onQuickLearnButtonClick()
        {
            ITutorial.start("Builder", false, "Quick Learn"); 
        }

        void onSteamEngineButtonClick()
        {
            ITutorial.start("Builder", false, "Steam Engine");
            
        }

        void onMiningButtonClick()
        {
            ITutorial.start("Assembler", false, "Mining");
        }

        void onObtainScientificButtonClick()
        {
            ITutorial.start("Builder", false, "Obtain Scientific");
        }
        void onOreSmeltingButtonClick()
        {
            ITutorial.start("Builder", false, "Ore Smelting");
        }
        void onElectricPowerButtonClick()
        {
            ITutorial.start("Builder", false, "Electric Power");
        }
        
        void onNuclearFuelButtonClick()
        {
            ITutorial.start("Builder", false, "Nuclear Fuel");
        }

        void onCircuitFoundationButtonClick()
        {
            ITutorial.start("Builder", false, "Circuit Foundation");
        }

        void onCraneButtonClick()
        {
            ITutorial.start("Builder", false, "Crane");
        }

        void onConstruction3DButtonClick()
        {
            ITutorial.start("Assembler", false, "Construction 3D");
        }

        void onUseOfTurretsButtonClick()
        {
            ITutorial.start("Assembler", false, "Use Of Turrets");
        }       
     

        void onBackButtonClick()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
