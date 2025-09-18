using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scraft{ public class IChangeImageButton : MonoBehaviour
    {

        public bool value = false;
        public bool manualChangeImagel = false;
        public Sprite trueSprite;
        public Sprite falseSprite;

        Image image;

        UnityAction call;

        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(onClick);
            image = GetComponent<Image>();
        }

        public void setValue(bool v)
        {
            value = v;
            if (value)
            {
                image.sprite = trueSprite;
            }
            else
            {
                image.sprite = falseSprite;
            }
        }

        public void addListener(UnityAction listener)
        {
            call = listener;
        }

        void onClick()
        {
            if (!manualChangeImagel)
            {
                value = !value;
                if (value)
                {
                    image.sprite = trueSprite;
                }
                else
                {
                    image.sprite = falseSprite;
                }
            }
            call();
        }


    }
}
