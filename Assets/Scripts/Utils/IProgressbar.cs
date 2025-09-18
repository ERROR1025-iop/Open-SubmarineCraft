using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class IProgressbar : MonoBehaviour
    {

        public float percent;

        public RectTransform bar;
        public Text text;

        private float maxBarWith;

        void Awake()
        {
            maxBarWith = bar.sizeDelta.x;
        }

        public void setValue01(float value)
        {
            value = Mathf.Clamp01(value);
            percent = value;
            bar.sizeDelta = new Vector2(maxBarWith * value, bar.sizeDelta.y);
            text.text = ((int)(value * 100)).ToString() + "%";
        }

        public void setValue(float value, float maxValue)
        {
            percent = value / maxValue;
            percent = Mathf.Clamp01(percent);
            bar.sizeDelta = new Vector2(maxBarWith * percent, bar.sizeDelta.y);
            text.text = string.Format("{0}/{1}", value.ToString("f0"), maxValue.ToString("f0"));
        }

    }
}
