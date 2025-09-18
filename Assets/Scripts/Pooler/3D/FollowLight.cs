using UnityEngine;

namespace Scraft
{
    public class FollowLight : MonoBehaviour
    {
        Light pointLight;

        void Start()
        {
            pointLight = GetComponent<Light>();
        }


        void Update()
        {
            bool open = MainSubmarine.lightLevel > 1;
            pointLight.enabled = open;
            pointLight.color = MainSubmarine.lightColor;
        }
    }
}