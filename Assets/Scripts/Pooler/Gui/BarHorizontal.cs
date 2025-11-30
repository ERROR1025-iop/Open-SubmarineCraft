

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scraft
{
    public class BarHorizontal : MonoBehaviour
    {

        public RectTransform barRectTrans;
        public UnityAction call;
        public Rect rect;
        public bool m_isReturnCenten;
        public bool isClickBar;

        public float width;
        
        public float startPos;

        public void addListener(UnityAction listener)
        {
            call = listener;
        }

        public virtual void setValue(float value)
        {
            
        }

        public virtual float getValue()
        {
            return 0;
        }
    }
}