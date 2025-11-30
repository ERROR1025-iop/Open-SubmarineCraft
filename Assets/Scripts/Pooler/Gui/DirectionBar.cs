using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scraft
{
    public class DirectionBar : MonoBehaviour
    {

        public RectTransform barRectTrans;

        Rect rect;

        UnityAction call;

        bool isClickBar;

        float hight;
        float startPos;

        int value;
        void Start()
        {
            RectTransform rt = transform.GetComponent<RectTransform>();
            rect = new Rect(new Vector2(800 + rt.anchoredPosition.x, rt.anchoredPosition.y), rt.sizeDelta);
            hight = rt.sizeDelta.y;
            startPos = rt.anchoredPosition.y;
            value = 1;
        }

        public void addListener(UnityAction listener)
        {
            call = listener;
        }

        public int getDirectionValue()
        {
            return value;
        }

        public void setDirectionValue(int value)
        {
            this.value = value;
            switch (value)
            {
                case -1:
                    barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, 0);
                    break;
                case 0:
                    barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, hight * 0.5f);
                    break;
                case 1:
                    barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, hight);
                    break;
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isClickBar = rect.Contains(IUtils.reviseMousePos(Input.mousePosition, PoolerUI.canvasW));
            }
            if (Input.GetMouseButton(0))
            {
                if (isClickBar)
                {
                    float barY = IUtils.reviseMousePos(Input.mousePosition, PoolerUI.canvasW).y - startPos;
                    if (barY < 0)
                    {
                        barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, 0);
                    }
                    else if (barY > hight)
                    {
                        barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, hight);
                    }
                    else
                    {
                        barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, barY);
                    }
                    reviseValue();
                    call();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (isClickBar)
                {
                    call();
                }
                reviseBarPos();
                isClickBar = false;
            }
        }

        void reviseBarPos()
        {
            float barY = barRectTrans.anchoredPosition.y;
            if (barY < hight * 0.33f)
            {
                barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, 0);
            }
            else if (barY > hight * 0.66f)
            {
                barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, hight);
            }
            else
            {
                barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, hight * 0.5f);
            }
        }

        void reviseValue()
        {
            float barY = barRectTrans.anchoredPosition.y;
            if (barY < hight * 0.33f)
            {
                value = -1;
            }
            else if (barY > hight * 0.66f)
            {
                value = 1;
            }
            else
            {
                value = 0;
            }
        }
    }
}