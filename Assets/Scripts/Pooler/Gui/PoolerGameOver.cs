using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scraft
{
    public class PoolerGameOver
    {


        RectTransform rectTrans;

        Pooler pooler;

        Text useTimeText;
        Text maxDeepText;
        Text beatShipText;

        public PoolerGameOver(Pooler pooler)
        {
            this.pooler = pooler;

            rectTrans = GameObject.Find("Canvas/game over").GetComponent<RectTransform>();
            GameObject.Find("Canvas/game over/Revert to launch").GetComponent<Button>().onClick.AddListener(onRevertLaunchButtonClick);
            GameObject.Find("Canvas/game over/Revert to blueprint").GetComponent<Button>().onClick.AddListener(onRevertBlueprintButtonClick);
            GameObject.Find("Canvas/game over/Revert to assembler").GetComponent<Button>().onClick.AddListener(onRevertAssemblerButtonClick);
            GameObject.Find("Canvas/game over/Back to menu").GetComponent<Button>().onClick.AddListener(onBackToMenuButtonClick);
            useTimeText = GameObject.Find("Canvas/game over/use time").GetComponent<Text>();
            maxDeepText = GameObject.Find("Canvas/game over/max deep").GetComponent<Text>();
            beatShipText = GameObject.Find("Canvas/game over/beat ship").GetComponent<Text>();

            setInfo(0, 0, 0, 0);
            show(false);
        }

        public void setInfo(int second, int maxDeep, int wave, int beatShipCount)
        {
            int min = (int)second / 60;
            int sec = second % 60;

            useTimeText.text = ILang.get("use time1", "menu") + ":" + min + ILang.get("use time2", "menu") + sec + ILang.get("use time3", "menu");
            maxDeepText.text = ILang.get("max deep", "menu") + ":" + maxDeep + "m";
            beatShipText.text = ILang.get("beat ship1", "menu") + wave + ILang.get("beat ship2", "menu") + beatShipCount + ILang.get("beat ship3", "menu");
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

        void onRevertLaunchButtonClick()
        {
            Pooler.isRunThread = false;
            IToast.instance.show("Loading");
            SceneManager.LoadScene("Pooler");
        }

        void onRevertBlueprintButtonClick()
        {
            Pooler.isRunThread = false;
            IToast.instance.show("Loading");
            Builder.IS_LOAD_LAST = true;
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
            Pooler.isRunThread = false;
            IToast.instance.show("Loading");
            SceneManager.LoadScene("Menu");
        }
    }
}