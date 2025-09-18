using UnityEngine;

namespace Scraft
{
    public class UnderwaterEffect : PostEffectsBase
    {

        public Shader briSatConShader;
        public Texture2D distortionMap;
        public Color colorMask;

        private Material briSatConMaterial;
        public Material material
        {
            get
            {
                briSatConMaterial = CheckShaderAndCreateMaterial(briSatConShader, briSatConMaterial);
                return briSatConMaterial;
            }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (GameSetting.renderUnderwaterEffect && material != null)
            {
                material.SetTexture("_DistortionMap", distortionMap);
                material.SetColor("_ColorMask", colorMask);
                Graphics.Blit(src, dest, material);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    }
}