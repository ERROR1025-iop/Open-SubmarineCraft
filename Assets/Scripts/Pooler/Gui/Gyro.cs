using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class Gyro
    {

        Text gyroText;

        public Gyro()
        {
            gyroText = GameObject.Find("Canvas/radar rect/radar text rect/gyro text").GetComponent<Text>();
        }

        public void setAngle(float angle)
        {
            gyroText.text = ILang.get("gyro") + ":" + angle.ToString("f0");
        }
    }
}