using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scraft
{
    public class BarHorizontal : MonoBehaviour
    {

        public RectTransform barRectTrans;

        UnityAction call;

        Rect rect;

        bool m_isReturnCenten;
        bool isClickBar;

        float width;
        float startPos;

        void Start()
        {
            RectTransform rt = transform.GetComponent<RectTransform>();
            rect = new Rect(new Vector2(800 + rt.anchoredPosition.x, rt.anchoredPosition.y), rt.sizeDelta);
            width = rt.sizeDelta.x;
            startPos = 800 + rt.anchoredPosition.x;
            m_isReturnCenten = true;

        }

        public void addListener(UnityAction listener)
        {
            call = listener;
        }

        public void setValue(float value)
        {
            barRectTrans.anchoredPosition = new Vector2(value * width, barRectTrans.anchoredPosition.y);
        }

        public float getValue()
        {
            float value = barRectTrans.anchoredPosition.x / width;
            return value;
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
                    float barX = IUtils.reviseMousePos(Input.mousePosition).x - startPos;
                    if (barX < 0)
                    {
                        barRectTrans.anchoredPosition = new Vector2(0, barRectTrans.anchoredPosition.y);
                    }
                    else if (barX > width)
                    {
                        barRectTrans.anchoredPosition = new Vector2(width, barRectTrans.anchoredPosition.y);
                    }
                    else
                    {
                        barRectTrans.anchoredPosition = new Vector2(barX, barRectTrans.anchoredPosition.y);
                    }
                    call();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (m_isReturnCenten && isClickBar)
                {
                    barRectTrans.anchoredPosition = new Vector2(width * 0.5f, barRectTrans.anchoredPosition.y);
                    call();
                }
                isClickBar = false;
            }
        }
    }
}