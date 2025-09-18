using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class PointLightRS : RunScript
    {
        public Light pointLight;
        public int lightPropertyId;

        void Start()
        {


            lightPropertyId = Shader.PropertyToID("_Open");
            if (World.GameMode == World.GameMode_Freedom)
            {

            }
            else if (World.GameMode == World.GameMode_Assembler)
            {
                bool open = true;
                pointLight.enabled = open;
                pointLight.color = Color.white;
                Shader.SetGlobalFloat(lightPropertyId, 1);
            }
            else if (World.GameMode == World.GameMode_Builder)
            {
                bool open = false;
                pointLight.enabled = open;
                Shader.SetGlobalFloat(lightPropertyId, 1);
            }            
        }


        void Update()
        {
            if (World.GameMode == World.GameMode_Freedom)
            {
                bool open = MainSubmarine.lightLevel > 1;
                pointLight.enabled = open;
                pointLight.color = MainSubmarine.lightColor;
                Shader.SetGlobalFloat(lightPropertyId, open ? 1 : 0);
            }
        }
    }
}