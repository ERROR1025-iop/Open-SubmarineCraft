using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Scraft
{
    public class LightZeroArea : AreaDetectorActive 
    {            
        protected override void OnEnterArea() { 
            Underwater.maxLight = 0;
        }

        protected override void OnExitArea() {
            Underwater.maxLight = 1;
        }
    }
}