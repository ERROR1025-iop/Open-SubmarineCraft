using UnityEngine;

namespace Scraft
{
    public class WaterBackGround : MonoBehaviour
    {

        new public static Transform transform;
        public static float waterLevelY;

        void Start()
        {
            transform = GetComponent<Transform>();
        }


        void Update()
        {
            float deep = MainSubmarine.deep;
            if (deep < 50)
            {
                transform.localPosition = new Vector3(0, -50f + Pooler.wbgOffsetY - MainSubmarine.transform.position.y, 10);
            }
            else if (deep > 50)
            {
                transform.localPosition = new Vector3(0, 0, 10);
            }
            waterLevelY = transform.localPosition.y + 50f;

            rotateAround(Vector3.zero, Vector3.forward, -MainSubmarine.transform.localEulerAngles.z);
        }

        void rotateAround(Vector3 center, Vector3 axis, float angle)
        {
            Vector3 pos = transform.position;
            Quaternion rot = Quaternion.AngleAxis(angle, axis);
            Vector3 dir = pos - center; //计算从圆心指向摄像头的朝向向量
            dir = rot * dir;            //旋转此向量
            transform.position = center + dir;
            var myrot = transform.rotation;
            transform.rotation = rot; //设置角度
        }
    }
}