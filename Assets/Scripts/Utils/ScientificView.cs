using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{    
    public class ScientificView : MonoBehaviour
    {

        public static ScientificView instance;
        public Text countText;

        async void Start()
        {
            instance = this;
            UpdateScientificCount();
        }

        public void UpdateScientificCount()
        {
            var s = ISecretLoad.getScientific();
            countText.text = s.ToString("f1");
        }
    }
}
