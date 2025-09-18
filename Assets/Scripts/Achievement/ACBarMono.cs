using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft.AchievementSpace
{
    public class ACBarMono : MonoBehaviour
    {

        Text text;
        RectTransform rectTrans;
        bool startShow;
        float speed = 500;
        float stay = 500;
        int showStack;
        int step = 0;


        void Start()
        {
            text = transform.GetChild(0).GetComponent<Text>();
            rectTrans = transform.GetComponent<RectTransform>();
            rectTrans.anchoredPosition = new Vector2(5, rectTrans.anchoredPosition.y);
            startShow = false;
        }

        public void show(string t)
        {
            text.text = t;
            rectTrans.anchoredPosition = new Vector2(5, rectTrans.anchoredPosition.y);
            startShow = true;
            step = 1;
            showStack = 0;
        }

        void Update()
        {
            if (step == 1)
            {
                step1();
            }
            else if (step == 2)
            {
                step2();
            }
            else if (step == 3)
            {
                step3();
            }
        }

        void step1()
        {
            if (rectTrans.anchoredPosition.x > -165)
            {
                rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x - speed * Time.deltaTime, rectTrans.anchoredPosition.y);
            }
            else
            {
                step = 2;
            }
        }

        void step2()
        {
            if (showStack < stay)
            {
                showStack++;
            }
            else
            {
                step = 3;
            }
        }

        void step3()
        {
            if (rectTrans.anchoredPosition.x < 5)
            {
                rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x + speed * Time.deltaTime, rectTrans.anchoredPosition.y);
            }
            else
            {
                step = 0;
                startShow = false;
                showStack = 0;
            }
        }
    }
}