using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scraft
{
    public class PitchBar : MonoBehaviour
    {
        public RectTransform barRectTrans;

        UnityAction call;

        Rect rect;

        bool m_isReturnCenten;
        bool m_isClickBar;

        float height;
        float startPos;

        void Start()
        {
            RectTransform rt = transform.GetComponent<RectTransform>();
            rect = new Rect(new Vector2(800 + rt.anchoredPosition.x, rt.anchoredPosition.y), rt.sizeDelta);
            height = rt.sizeDelta.y;
            startPos = rt.anchoredPosition.y;
            m_isReturnCenten = true;

        }

        public void addListener(UnityAction listener)
        {
            call = listener;
        }

        public void setValue(float value)
        {
            barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, value * height);
        }

        public float getValue()
        {
            float value = barRectTrans.anchoredPosition.y / height;
            return value;
        }

        public bool isClickBar()
        {
            return m_isClickBar;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_isClickBar = rect.Contains(IUtils.reviseMousePos(Input.mousePosition));

            }
            if (Input.GetMouseButton(0))
            {
                if (m_isClickBar)
                {
                    float barY = IUtils.reviseMousePos(Input.mousePosition).y - startPos;
                    if (barY < 0)
                    {
                        barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, 0);
                    }
                    else if (barY > height)
                    {
                        barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, height);
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
                if (m_isReturnCenten && m_isClickBar)
                {
                    barRectTrans.anchoredPosition = new Vector2(barRectTrans.anchoredPosition.x, height * 0.5f);
                    call();
                }
                m_isClickBar = false;
            }
        }
    }
}