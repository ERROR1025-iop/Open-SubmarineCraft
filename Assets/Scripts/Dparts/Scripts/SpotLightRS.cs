using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scraft.DpartSpace
{
    public class SpotLightRS : RunScript
    {
        public Light spotLight;
        public MeshRenderer lightBeam;
        public int lightPropertyId;

        void Start()
        {
            
            lightPropertyId = Shader.PropertyToID("_Open");
            if (World.GameMode == World.GameMode_Freedom)
            {
                spotLight.range = 1;
                spotLight.intensity = 1;

                if (transform.localScale.z < 0)
                {
                    spotLight.transform.Rotate(new Vector3(180, 0, 0));
                }
            }
            else if (World.GameMode == World.GameMode_Assembler)
            {
                spotLight.range = 1;
                spotLight.intensity = 1;
                lightBeam.enabled = false;
                spotLight.color = Color.white;
                Shader.SetGlobalFloat(lightPropertyId, 1);
            }
            else if (World.GameMode == World.GameMode_Builder)
            {
                spotLight.enabled = false;
                lightBeam.enabled = false;
                Shader.SetGlobalFloat(lightPropertyId, 1);
            }
        }


        void Update()
        {
            if (World.GameMode == World.GameMode_Freedom)
            {
                spotLight.intensity = MainSubmarine.lightLevel * 0.2f;
                spotLight.range = MainSubmarine.lightLevel * 10;
                spotLight.color = MainSubmarine.lightColor;
                lightBeam.enabled = GameSetting.renderLightbeam && MainSubmarine.lightLevel > 1;
                Shader.SetGlobalFloat(lightPropertyId, MainSubmarine.lightLevel > 1 ? 1 : 0);
            }
        }
    }
}