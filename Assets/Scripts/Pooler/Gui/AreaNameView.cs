using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scraft
{
    public class AreaNameView : MonoBehaviour
    {
        public static AreaNameView instance;
        public Text text;
        public GameObject icon;

        public string areaString;
        public string sciString;

        void Awake()
        {
            instance = this;
            icon.SetActive(false);
        }

        public void SetAreaString(string areaString)
        {
            this.areaString = areaString;
            if (sciString == null || sciString == "")
            {                
                if(areaString != null && areaString != "")
                {
                    text.text = areaString;
                }
                else
                {
                    text.text = "";
                }
            }
        }

        public void SetSciString(string sciString)
        {
            this.sciString = sciString;
            if (sciString == null || sciString == "")
            {
                icon.SetActive(false);
                if(areaString != null && areaString != "")
                {
                    text.text = areaString;
                }
                else
                {
                    text.text = "";
                }
            }
            else
            {
                icon.SetActive(true);
                text.text = sciString;
            }
        }
    }
}