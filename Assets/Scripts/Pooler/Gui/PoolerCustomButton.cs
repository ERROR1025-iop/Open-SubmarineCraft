using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Scraft {
    public class PoolerCustomButton : MonoBehaviour
    {
        static public PoolerCustomButton instance;
               
        public Button button1;
        public Text button1Text;
        public Button deleteButton;
        public Button cancelButton;

        RectTransform mainTrans;
        bool isShow;

        public UnityAction button1ClickCall;
        public UnityAction deleteClickCall;
        public UnityAction cancelClickCall;

        void Awake()
        {
            instance = this;
            button1.onClick.AddListener(onButton1Click);
            deleteButton.onClick.AddListener(onDeleteClick);
            cancelButton.onClick.AddListener(onCancelClick);

            mainTrans = GetComponent<RectTransform>();

            show(false);
        }
        

        public void setButton1Text(string LangString)
        {
            button1Text.text = ILang.get(LangString);
        }

        void onButton1Click()
        {
            if(button1ClickCall != null)
            {
                button1ClickCall();
            }
        }

        void onDeleteClick()
        {
            if (deleteClickCall != null)
            {
                deleteClickCall();
            }
        }

        void onCancelClick()
        {
            show(false);
            if (cancelClickCall != null)
            {
                cancelClickCall();
            }
        }

        public void initialized(string text1, UnityAction button1Click, UnityAction deleteClick, UnityAction cancelClick)
        {
            if (cancelClickCall != null)
            {
                cancelClickCall();
            }

            setButton1Text(text1);
            button1ClickCall = button1Click;
            deleteClickCall = deleteClick;
            cancelClickCall = cancelClick;
        }

        public void show(bool show)
        {
            isShow = show;
            if (show)
            {
                mainTrans.anchoredPosition = new Vector2(-44f, -96.6f);
            }
            else
            {
                mainTrans.anchoredPosition = new Vector2(9999f, 0);

            }
        }

        public void setClickCallNull()
        {
            button1ClickCall = null;
            deleteClickCall = null;
            cancelClickCall = null;
        }
    }
}