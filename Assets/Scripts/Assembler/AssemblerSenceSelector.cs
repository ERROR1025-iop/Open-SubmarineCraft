using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class AssemblerSenceSelector
    {
        RectTransform rectTrans;
        Assembler assembler;

        public AssemblerSenceSelector()
        {
            assembler = Assembler.instance;

            rectTrans = GameObject.Find("Canvas/Sence Selector").GetComponent<RectTransform>();

            rectTrans.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(onSeaButtonClick);
            rectTrans.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(onBuilderButtonClick);
            rectTrans.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(onResumeGameButtonClick);
        }

        public void show(bool show)
        {
            if (show)
            {
                rectTrans.anchoredPosition = new Vector2(-70, 0);
            }
            else
            {
                rectTrans.anchoredPosition = new Vector2(1000, 0);
            }
        }

        public void onSeaButtonClick()
        {
            IToast.instance.show("Loading");
            string backgroundPath = string.Format("Menu/Loading/{0}{1}", GameSetting.isCreateAi ? "n" : "s", (int)(Random.value * 2.9f)); ;
            AsyncLoadScene.sprite = Resources.Load(backgroundPath, typeof(Sprite)) as Sprite;
            UnityAndroidEnter.CallShowInterstitialAD();        
            assembler.saveDpart("Last Ship");
            assembler.createThumbnailFile("Last Ship", GamePath.assemblerThumbnailFolder);
            Pooler.IS_Form_StationCenter = false;
            if (GameSetting.isAndroid)
            {
                AsyncLoadScene.asyncloadScene("Pooler");
            }
            else
            {
                Application.LoadLevel("Pooler");
            }
        }

        public void onBuilderButtonClick()
        {
            if (ITutorial.tutorialStep >= 0)
            {
                ITutorial.tutorialStep++;
            }
            IToast.instance.show("Loading");
            assembler.saveDpart("Last Ship");
            assembler.createThumbnailFile("Last Ship", GamePath.assemblerThumbnailFolder);
            Application.LoadLevel("Builder");
        }

        void onResumeGameButtonClick()
        {
            show(false);
        }
    }
}