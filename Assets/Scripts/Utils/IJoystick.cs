using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Scraft
{
    public class IJoystick : MonoBehaviour
    {
        public float x;
        public float y;     
        public bool isPointed;
        public bool changing;

        public virtual void SetValue(float x, float y)
        {
            
        }
    }
}