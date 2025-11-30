using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class SDColSelectorCell : MonoBehaviour
    {
        public TMPro.TMP_Text titleText;
        public Button button;
        public SDColSelector sdColSelector;
        public string value;

        void Start()
        {
            button.onClick.AddListener(() => sdColSelector.onCellClick(this));
        }
    }
}


