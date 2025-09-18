using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class DeepGauge
    {

        Text deepText1;
        Text vspeedText;
        RectTransform deepArrTrans;

        public DeepGauge()
        {
            deepText1 = GameObject.Find("Canvas/radar rect/radar text rect/deep text").GetComponent<Text>();
            vspeedText = GameObject.Find("Canvas/radar rect/radar text rect/vspeed").GetComponent<Text>();
            deepArrTrans = GameObject.Find("Canvas/deep gauge/deep arr").GetComponent<RectTransform>();
        }

        public void setDeep(float deep)
        {
            if (deep < 0)
            {
                deep = 0;
            }
            if (deep < 3000)
            {
                deepArrTrans.anchoredPosition = new Vector2(deepArrTrans.anchoredPosition.x, -1 - 0.06f * deep);
            }

            deepText1.text = ILang.get("deep", "menu") + ":" + deep.ToString("f0") + "m";
        }

        public void setVerticalSpeed(float vSpeed)
        {
            if (Mathf.Abs(vSpeed) < 0.1f)
            {
                vSpeed = 0;
            }
            if (vSpeed > 0)
            {
                vspeedText.text = "↑ " + Mathf.Abs(vSpeed).ToString("f1") + "m/s";
            }
            else if (vSpeed < 0)
            {
                vspeedText.text = "↓ " + Mathf.Abs(vSpeed).ToString("f1") + "m/s";
            }
            else
            {
                vspeedText.text = "  " + Mathf.Abs(vSpeed).ToString("f1") + "m/s";
            }
        }


    }
}