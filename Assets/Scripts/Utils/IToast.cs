using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class IToast : MonoBehaviour
    {
        static public IToast instance;
        public bool update = false;

        Text toastText;

        int showTime;
        int timeStack;      


        void Awake()
        {
            instance = this;
            toastText = transform.GetChild(0).GetComponent<Text>();
            hide();
            timeStack = 0;
            enabled = update;
            // 防止重复创建
            // if (instance != null && instance != this)
            // {
            //     Destroy(gameObject);
            //     return;
            // }
            // DontDestroyOnLoad(gameObject);
        }

        public void showWithoutILang(string str, int time)
        {
            if (toastText != null)
            {
                showTime = time;
                timeStack = time;
                toastText.text = str;
                gameObject.SetActive(true);
            }
        }

        public void show(string str, int time)
        {
            if(toastText != null)
            {
                showTime = time;
                timeStack = time;
                toastText.text = ILang.get(str);
                gameObject.SetActive(true);
            }
        }

        public void showWithoutILang(string str)
        {
            if (toastText != null)
            {
                showTime = 0;
                timeStack = 0;
                toastText.text = str;
                gameObject.SetActive(true);
            }
        }

        public void show(string str)
        {
            if (toastText != null)
            {
                showTime = 0;
                timeStack = 0;
                toastText.text = ILang.get(str);
                gameObject.SetActive(true);
            }
        }

        public void hide()
        {
            if(timeStack  <= 0)
            {
                showTime = 0;
                timeStack = 0;
                gameObject.SetActive(false);
            }           
        }

        void Update()
        {
            if (showTime > 0)
            {
                timeStack--;
                if (timeStack < 0)
                {
                    hide();
                }
            }
        }
    }
}