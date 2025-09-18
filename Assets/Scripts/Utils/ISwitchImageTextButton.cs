using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Scraft
{
    public class ISwitchImageTextButton : MonoBehaviour
    {

        public Sprite imageSprite;
        public Sprite showTextSprite;
        public string showString;

        Image image;
        Text text;
        bool isShowText;

        static List<ISwitchImageTextButton> buttonsList;

        void Awake()
        {
            if (buttonsList == null)
            {
                buttonsList = new List<ISwitchImageTextButton>();
            }            
        }

        private void OnDestroy()
        {
            buttonsList.Remove(this);
        }

        public static void setGlobalShow(bool isShowText)
        {
            int count = buttonsList.Count;
            for (int i = 0; i < count; i++)
            {
                buttonsList[i].setShow(isShowText);
            }
        }

        public static void switchGlobalTextImage()
        {
            int count = buttonsList.Count;
            for (int i = 0; i < count; i++)
            {
                buttonsList[i].switchTextImage();
            }
        }

        void Start()
        {
            image = GetComponent<Image>();
            text = transform.GetChild(0).GetComponent<Text>();

            buttonsList.Add(this);
            setShow(GameSetting.isAssemblerShowText);
        }

        public void switchTextImage()
        {
            isShowText = !isShowText;
            setShow(isShowText);
        }

        public void setShow(bool isShowText)
        {
            this.isShowText = isShowText;
            if (isShowText)
            {
                OnShowText();
            }
            else
            {
                OnShowImage();
            }
        }

        public void change(Sprite imageSprite, string showString)
        {
            this.imageSprite = imageSprite;
            this.showString = showString;
            if (isShowText)
            {
                OnShowText();
            }
            else
            {
                OnShowImage();
            }
        }

        public void OnShowText()
        {            
            text.text = ILang.get(showString);
            image.sprite = showTextSprite;
            isShowText = true;
        }

        public void OnShowImage()
        {            
            text.text = " ";
            image.sprite = imageSprite;
            isShowText = false;
        }

    }
}