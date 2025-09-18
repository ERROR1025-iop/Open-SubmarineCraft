using UnityEngine;
using System.Collections;

namespace Scraft
{
    public class Buoyancy : MonoBehaviour
    {
        Rigidbody rigidbody;

        public static Vector3 floatCenter;
        Bounds bounds;
        Vector3[] floatPoint;
        public int lookPoint;
        public float force, deep;
        float floatForce, m_force, h1, h2;
        bool isInit = false;

        public static float waterHeight = 0;
        public float m_tonnage = 0;

        float floatCenterY1, floatCenterY2;

        float lastInversion;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            floatPoint = new Vector3[4];           
        }

        private void OnDrawGizmosSelected()
        {
            if (floatPoint != null && floatPoint.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    Gizmos.DrawWireSphere(transform.TransformPoint(floatPoint[i]), 0.2f);
                }
                Gizmos.DrawWireSphere(transform.TransformPoint(MainSubmarine.weightCenter), 0.5f);
            }
        }        

        void initialized()
        {
            floatCenterY1 = Pooler.shipOffsetY;
            floatCenterY2 = MainSubmarine.weightCenter.y;

            float fy = MainSubmarine.inversion >= 0 ? floatCenterY1 : floatCenterY2;
            floatCenter = new Vector3( MainSubmarine.weightCenter.x, fy, MainSubmarine.weightCenter.z);
            bounds = MainSubmarine.bounds;
            float bottomY = MainSubmarine.bounds.min.y;

            floatPoint[0] = new Vector3(floatCenter.x + bounds.extents.x * 0.8f, floatCenter.y, floatCenter.z);
            floatPoint[1] = new Vector3(floatCenter.x - bounds.extents.x * 0.8f, floatCenter.y, floatCenter.z);
            floatPoint[2] = new Vector3(floatCenter.x, floatCenter.y, floatCenter.z + bounds.extents.y);
            floatPoint[3] = new Vector3(floatCenter.x, floatCenter.y, floatCenter.z - bounds.extents.y);
            isInit = true;
            lastInversion = MainSubmarine.inversion;
        }

        void FixedUpdate()
        {
            if (!isInit)
            {
                initialized();
            }

            float worldFloatCenterY = transform.TransformPoint(floatCenter).y;
            h1 = 5f + Mathf.Clamp(worldFloatCenterY, -0.241f, 0) * 20;
            h2 = h1;


            for (int i = 0; i < 4; i++)
            {
                Vector3 worldFloatPoint = transform.TransformPoint(floatPoint[i]);
                float water = 0;
                float G = Physics.gravity.y * rigidbody.mass;
                float m_deep = water - worldFloatPoint.y;
                m_tonnage = MainSubmarine.tonnage;
                m_deep = m_deep > 0 ? Mathf.Min(m_deep, h1) : Mathf.Max(m_deep, -h2);
                m_deep *= 2;
                float baseFloatForce = -G * (m_deep + 1);
                if (worldFloatCenterY > -1 && m_tonnage > 0)
                {
                    m_tonnage = MainSubmarine.tonnage * -worldFloatCenterY;
                }
                floatForce = baseFloatForce + m_tonnage;
                floatForce = Mathf.Max(0, floatForce);                
                m_force = floatForce + G;
                float forceRate = 1;    
                if (MainSubmarine.isRunSurface && i < 2)
                {
                    forceRate = Mathf.Abs(worldFloatPoint.y) + 1;
                }
                if (!MainSubmarine.isRunSurface)
                {
                    forceRate = 0.5f;
                }
                m_force = m_force * forceRate;  
                if(MainSubmarine.isRunSurface && (m_force < G))
                {
                    m_force = G;
                }
                if(i == lookPoint)
                {
                    force = m_force;
                    deep = m_deep;
                }
                Vector3 forceVector = new Vector3(0, m_force, 0);              
                Debug.DrawLine(worldFloatPoint, worldFloatPoint + forceVector, Color.blue);
                rigidbody.AddForceAtPosition(forceVector, worldFloatPoint);           
            }

            if(lastInversion * MainSubmarine.inversion < 0)
            {
                isInit = false;                
            }
            lastInversion = MainSubmarine.inversion;
        }
    }
}