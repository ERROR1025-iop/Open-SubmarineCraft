using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Scraft
{
    public class IJoystick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform buttonRectTrans;

        bool isInRect;

        public bool isPointed;
        public float x;
        public float y;

        RectTransform JoystickRectTrans;

        void Start()
        {
            isInRect = false;
            isPointed = false;

            JoystickRectTrans = GetComponent<RectTransform>();
        }


        void Update()
        {
            if (Input.GetMouseButtonDown(0) && isInRect)
            {
                isPointed = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isPointed = false;
                buttonRectTrans.anchoredPosition = Vector2.zero;
                x = 0;
                y = 0;
            }

            if (isPointed)
            {
                Vector2 mousePoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(JoystickRectTrans, Input.mousePosition, null, out mousePoint))
                {
                    if (mousePoint.magnitude > 50)
                    {
                        buttonRectTrans.anchoredPosition = mousePoint.normalized * 50;
                    }
                    else
                    {
                        buttonRectTrans.anchoredPosition = mousePoint;
                    }

                    x = buttonRectTrans.anchoredPosition.x * 0.01f;
                    y = buttonRectTrans.anchoredPosition.y * 0.01f;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isInRect = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isInRect = false;
        }
    }
}