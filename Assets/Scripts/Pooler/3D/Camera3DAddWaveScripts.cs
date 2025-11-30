using System.Collections;
using System.Collections.Generic;
using LuxWater;
using UnityEngine;

namespace Scraft
{
    public class Camera3DAddWaveScripts : MonoBehaviour
    {
        void Start()
        {
            LuxWater_WaterVolumeTrigger luxWater_WaterVolumeTrigger = gameObject.AddComponent<LuxWater_WaterVolumeTrigger>();
            luxWater_WaterVolumeTrigger.cam = GetComponent<Camera>();
            gameObject.AddComponent<LuxWater_UnderWaterRendering>();
            gameObject.AddComponent<LuxWater_UnderWaterBlur>();
            gameObject.AddComponent<LuxWater_ProjectorRenderer>();

        }
    }
}
