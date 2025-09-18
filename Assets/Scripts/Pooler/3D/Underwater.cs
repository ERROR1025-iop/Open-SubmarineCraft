using UnityEngine;

namespace Scraft
{
    public class Underwater : MonoBehaviour
    {

        public Material downWater;
        public Light directorLight;

        private int underwaterLevel = 0;
        private float fogDensity = 0.005F;
        private Skybox skybox;
        private Camera camera3d;
        private Transform subTrans;

        void Start()
        {

            skybox = GetComponent<Skybox>();
            camera3d = GetComponent<Camera>();
            camera3d.backgroundColor = new Color(0.1450f, 0.2549f, 0.3137f, 1);
            directorLight = GameObject.Find("Directional light").GetComponent<Light>();
            subTrans = MainSubmarine.transform;
        }

        void Update()
        {
            if (transform.position.y <= underwaterLevel + 0.02f)
            {
                float CameraY = transform.position.y;
                float subY = subTrans.localPosition.y;
                float minY = Mathf.Min(CameraY, subY);
                skybox.enabled = false;
                //摄像机及灯光背景颜色
                if (minY > -15)
                {
                    camera3d.backgroundColor = new Color(0.1450f + minY * 0.00967f, 0.2549f + minY * 0.017f, 0.3137f + minY * 0.02091f, 1);
                    directorLight.intensity = minY * 0.067f;
                }
                else
                {
                    camera3d.backgroundColor = Color.black;
                    directorLight.intensity = 0;
                }
                //海底背景颜色变换
                if (minY > -20)
                {
                    float seay = minY + 5;
                    downWater.SetColor("_Color", new Color(0.1450f + seay * 0.00967f, 0.2549f + seay * 0.017f, 0.3137f + seay * 0.02091f, 1));
                }
                else
                {
                    downWater.SetColor("_Color", Color.black);
                }
                float farClipPlaneMax = Pooler.instance.isOpenAdvTerrainSonar() ? 800 : 300;
                camera3d.farClipPlane = Mathf.Clamp(-minY * 50, 100, farClipPlaneMax);
            }
            else
            {
                skybox.enabled = true;
                camera3d.farClipPlane = 1000;
                directorLight.intensity = 1;
            }
        }
    }
}