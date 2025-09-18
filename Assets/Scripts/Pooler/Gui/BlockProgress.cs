using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft {
    public class BlockProgress : MonoBehaviour
    {
        static public BlockProgress instance;

        public RectTransform bar;
        private float maxBarWith;

        void Start()
        {
            instance = this;
            maxBarWith = bar.sizeDelta.x;
            setValue(0);
        }

        public void setValue(float percent)
        {
            percent = Mathf.Clamp01(percent);
            bar.sizeDelta = new Vector2(maxBarWith * percent, bar.sizeDelta.y);
        }
    }
}
