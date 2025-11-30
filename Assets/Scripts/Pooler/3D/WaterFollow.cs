using UnityEngine;

namespace Scraft
{
    public class WaterFollow : MonoBehaviour
    {
        Transform selfSubTrans;

        void Start()
        {
            selfSubTrans = MainSubmarine.transform;
        }


        void Update()
        {
            transform.localPosition = new Vector3(selfSubTrans.position.x, -0.02f, selfSubTrans.position.z);
        }
    }
}