using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Scraft
{
    public class IPressButton : MonoBehaviour
    {

        public Sprite trueSprite;
        public Sprite falseSprite;

        public int pressId = 0;
        public bool value;

        bool lastValue;

        RectTransform rt;
        Canvas parentCanvas;
        Image image;
        UnityAction call;

        void Start()
        {
            rt = transform.GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();
            image = transform.GetComponent<Image>();
        }

        public void setValueChangeListener(UnityAction action)
        {
            call = action;
        }

        public void setValue(bool b)
        {
            value = b;
            setImageValue(value);
        }

        public void setImageValue(bool b)
        {
            if (b)
            {
                image.sprite = trueSprite;
            }
            else
            {
                image.sprite = falseSprite;
            }
        }

        void Update()
        {
            Camera cam = null;
            if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                cam = parentCanvas.worldCamera;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition, cam))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    value = true;
                    setImageValue(value);
                    if (call != null) call();
                }
            }
            if (value == true && Input.GetMouseButtonUp(0))
            {
                value = false;
                setImageValue(value);
                if (call != null) call();
            }
        }
    }
}