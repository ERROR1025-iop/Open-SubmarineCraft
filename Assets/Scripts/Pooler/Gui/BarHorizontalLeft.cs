using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scraft
{
    public class BarHorizontalLeft : BarHorizontal
    {
        void Start()
        {
            RectTransform rt = transform.GetComponent<RectTransform>();
            rect = new Rect(new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y), rt.sizeDelta);
            width = rt.sizeDelta.x;
            startPos = rt.anchoredPosition.x;
            m_isReturnCenten = true;

        }

        public override void setValue(float value)
        {
            barRectTrans.anchoredPosition = new Vector2(value * width, barRectTrans.anchoredPosition.y);
        }

        public override float getValue()
        {
            float value = barRectTrans.anchoredPosition.x / width;
            return value;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var p = IUtils.reviseMousePos(Input.mousePosition, PoolerUI.canvasW);
                isClickBar = rect.Contains(p);                
            }
            if (Input.GetMouseButton(0))
            {
                if (isClickBar)
                {
                    float barX = IUtils.reviseMousePos(Input.mousePosition, PoolerUI.canvasW).x - startPos;
                    Debug.Log("barX:" + barX + ", rect:" + rect + ", isClickBar:" + isClickBar);
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