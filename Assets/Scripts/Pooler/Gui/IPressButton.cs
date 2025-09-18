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

        Rect rect;
        Image image;
        UnityAction call;

        void Start()
        {
            RectTransform rt = transform.GetComponent<RectTransform>();
            rect = new Rect(new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y), rt.sizeDelta);
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
            if (rect.Contains(IUtils.reviseMousePos(Input.mousePosition)))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    value = true;
                    setImageValue(value);
                    call();
                }
            }
            if (value == true && Input.GetMouseButtonUp(0))
            {
                value = false;
                setImageValue(value);
                call();
            }
        }
    }
}