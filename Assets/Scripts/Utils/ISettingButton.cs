using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scraft
{
    public class ISettingButton : MonoBehaviour
    {

        public Text buttonText;
        public Button button;
        public string valueTure;
        public string valueFalse;

        bool value;

        void Start()
        {

        }

        public void init()
        {
            valueTure = ILang.get(valueTure, "menu");
            valueFalse = ILang.get(valueFalse, "menu");
        }

        public void setButtonText(string text)
        {
            buttonText.text = text;
        }

        public void setValue(bool value)
        {
            this.value = value;
            if (value)
            {
                buttonText.text = valueTure;
            }
            else
            {
                buttonText.text = valueFalse;
            }
        }

        public bool getValue()
        {
            return value;
        }

        public void setClickListener(UnityAction call)
        {
            button.onClick.AddListener(call);
        }

    }
}