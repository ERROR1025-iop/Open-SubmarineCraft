using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scraft
{
    public class WaveMono : MonoBehaviour
    {

        public Text text;
        RectTransform rectTrans;
        UnityAction call;

        int step;
        float speed = 1000;
        int showStack;

        void Start()
        {
            rectTrans = GetComponent<RectTransform>();
            init();
            if (GameSetting.isCreateAi)
            {
                startShow(1);
            }

        }

        public void startShow(int wave)
        {
            string str = wave.ToString();
            if (wave < 4)
            {
                str = ILang.get("number" + wave, "menu");
            }
            text.text = ILang.get("wave1", "menu") + str + ILang.get("wave2", "menu");
            step = 1;
        }

        public void setShowFinishedListener(UnityAction listener)
        {
            call = listener;
        }

        void init()
        {
            rectTrans.anchoredPosition = new Vector2(1380, rectTrans.anchoredPosition.y);
            step = 0;
            showStack = 0;
        }

        void Update()
        {

            if (step != 0)
            {
                if (step == 1)
                {
                    if (rectTrans.anchoredPosition.x >= 0)
                    {
                        rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x - speed * Time.deltaTime, rectTrans.anchoredPosition.y);
                    }
                    else
                    {
                        step = 2;
                    }
                }
                else if (step == 2)
                {
                    if (showStack < 100)
                    {
                        showStack++;
                    }
                    else
                    {
                        step = 3;
                    }
                }
                else if (step == 3)
                {
                    if (rectTrans.anchoredPosition.x >= -1380)
                    {
                        rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x - speed * Time.deltaTime, rectTrans.anchoredPosition.y);
                    }
                    else
                    {
                        init();
                        step = 0;
                        if (call != null)
                        {
                            call();
                        }
                    }
                }
            }
        }
    }
}