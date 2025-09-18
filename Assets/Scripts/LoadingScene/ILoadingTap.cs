using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class ILoadingTap : MonoBehaviour
    {

        Text tapText;

        void Start()
        {

            tapText = GetComponent<Text>();
            int index = (int)(Random.value * 9.0f);
            tapText.text = ILang.get("Tap") + ":" + ILang.get("tap" + index, "tap");
        }
    }
}