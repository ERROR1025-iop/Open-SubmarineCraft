using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Scraft
{
    public class IJoystickV1 : IJoystick, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform buttonRectTrans;        
        bool isInRect;
        RectTransform JoystickRectTrans;

        void Start()
        {
            isInRect = false;
            isPointed = false;
            changing = false;

            JoystickRectTrans = GetComponent<RectTransform>();
        }


        void Update()
        {
            if (isPointed == false)
            {
                changing = false;
            }

            if (Input.GetMouseButtonDown(0) && isInRect)
            {
                isPointed = true;
                changing = true;
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

        override public void SetValue(float x, float y)
        {
            // Convert normalized x,y (stored as -1..1) to anchored position (scaled by 100)
            Vector2 anchored = new Vector2(x * 100f, y * 100f);

            // Clamp to joystick range (radius 50)
            if (anchored.magnitude > 50f)
            {
                anchored = anchored.normalized * 50f;
            }

            // Apply to button if available
            if (buttonRectTrans != null)
            {
                buttonRectTrans.anchoredPosition = anchored;
            }

            // Store values on this instance (fields inherited from IJoystick)
            this.x = anchored.x * 0.01f;
            this.y = anchored.y * 0.01f;
        }
    }
}