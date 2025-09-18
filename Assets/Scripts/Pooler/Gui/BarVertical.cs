using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scraft
{
    public class BarVertical : MonoBehaviour
    {


        public RectTransform barRectTrans;

        UnityAction call;

        Rect rect;

        bool m_isReturnCenten;
        bool isClickBar;

        float hight;
        float startPos;

        void Start()
        {
            RectTransform rt = transform.GetComponent<RectTransform>();
            rect = new Rect(new Vector2(800 + rt.anchoredPosition.x, rt.anchoredPosition.y), rt.sizeDelta);
            hight = rt.sizeDelta.y;
            startPos = rt.anchoredPosition.y;
            m_isReturnCenten = true;

        }

        public void addListener(UnityAction listener)
        {
            call = listener;
        }

        public float getValue()
        {
            float value = barRectTrans.anchoredPosition.y / hight;
            return value;
        }

        public void addValue(float val)
        {
            setValue(getValue() + val);
        }

        public void setValue(float val)
        {
            if (val < 0)
            {
                val = 0;
            }
            else if (val > 1)
            {
                val = 1;
            }
            barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, val * hight);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isClickBar = rect.Contains(IUtils.reviseMousePos(Input.mousePosition));
            }
            if (Input.GetMouseButton(0))
            {
                if (isClickBar)
                {
                    float barY = IUtils.reviseMousePos(Input.mousePosition).y - startPos;
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
                    call();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                isClickBar = false;
            }
        }
    }
}