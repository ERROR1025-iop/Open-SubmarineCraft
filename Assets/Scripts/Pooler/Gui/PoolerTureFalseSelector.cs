using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Scraft
{
    public class PoolerTureFalseSelector : MonoBehaviour
    {
        static public PoolerTureFalseSelector instance;

        UnityAction trueCall;
        UnityAction falseCall;
        UnityAction rotateCall;
        UnityAction upCall;
        UnityAction downCall;
        Button rotateButton;
        Button upButton;
        Button downButton;
        bool autoHide;

        void Start()
        {
            instance = this;
            transform.GetChild(0).GetComponent<Button>().onClick.AddListener(onTrueButtonClick);
            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(onFalseButtonClick);
            rotateButton = transform.GetChild(2).GetComponent<Button>();
            rotateButton.onClick.AddListener(onRotateButtonClick);
            upButton = transform.GetChild(3).GetComponent<Button>();
            upButton.onClick.AddListener(onUpButtonClick);
            downButton = transform.GetChild(4).GetComponent<Button>();
            downButton.onClick.AddListener(onDownButtonClick);
            autoHide = true;
            show(false);
        }

        public void show(bool isShow, UnityAction trueListener, UnityAction falseListener, UnityAction rotateListener, UnityAction upListener, UnityAction downListener, bool autoHide = true)
        {
            trueCall = trueListener;
            falseCall = falseListener;
            rotateCall = rotateListener;
            upCall = upListener;
            downCall = downListener;
            gameObject.SetActive(isShow);
            this.autoHide = autoHide;
            rotateButton.gameObject.SetActive(rotateListener != null);
            upButton.gameObject.SetActive(upListener != null);
            downButton.gameObject.SetActive(downListener != null);
        }

        public void show(bool isShow)
        {          
            gameObject.SetActive(isShow);
        }

        void onTrueButtonClick()
        {
            if(trueCall != null)
            {
                trueCall();
            }
            if (autoHide)
            {
                show(false);
            }
        }

        void onFalseButtonClick()
        {
            if (falseCall != null)
            {
                falseCall();
            }
            if (autoHide)
            {
                show(false);
            }
        }

        void onRotateButtonClick()
        {
            if (rotateCall != null)
            {
                rotateCall();
            }
        }

        void onUpButtonClick()
        {
            if (upCall != null)
            {
                upCall();
            }
        }

        void onDownButtonClick()
        {
            if (downCall != null)
            {
                downCall();
            }
        }
    }
}
