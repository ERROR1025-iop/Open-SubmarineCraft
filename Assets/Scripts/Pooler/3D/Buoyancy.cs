using UnityEngine;
using System.Collections;
using System;

namespace Scraft
{
    public class Buoyancy : MonoBehaviour
    {
        Rigidbody rigidbody;

        public static Vector3 floatCenter;
        Bounds bounds;
        Vector3[] waterFloatPoint;
        Vector3[] floatPoint;
        public float dyMax = 1.5f;
        public float gFloatRate = 1;
        public int lookPoint;
        public float force, deep;
        float floatForce, m_force, h1, h2;
        bool isInit = false;

        public static float waterHeight = 0;
        public float m_tonnage = 0;

        float floatCenterY1, floatCenterY2;

        float lastInversion;

        public Material WaterMaterial1;
        public Material WaterMaterial2;
        static LuxWaterUtils.GersterWavesDescription Description;
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            floatPoint = new Vector3[4];
            waterFloatPoint = new Vector3[4];

            if (GameSetting.waveMode == 1)
            {
                LuxWaterUtils.GetGersterWavesDescription(ref Description, WaterMaterial1);               
            }
            if (GameSetting.waveMode == 2)
            {
                LuxWaterUtils.GetGersterWavesDescription(ref Description, WaterMaterial2);
            }
        }

        private void OnDrawGizmosSelected()
        {
            var fp = floatPoint;
            if (fp != null && fp.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(transform.TransformPoint(fp[i]), 0.2f);

                }
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.TransformPoint(MainSubmarine.weightCenter), 0.5f);
            }
        }

        void initialized()
        {
            floatCenterY1 = Pooler.shipOffsetY;
            floatCenterY2 = MainSubmarine.weightCenter.y;

            float fy = MainSubmarine.inversion >= 0 ? floatCenterY1 : floatCenterY2;
            floatCenter = new Vector3(MainSubmarine.weightCenter.x, fy, MainSubmarine.weightCenter.z);
            bounds = MainSubmarine.bounds;
            float bottomY = MainSubmarine.bounds.min.y;

            float floatCy = MainSubmarine.weightCenter.y;
            floatPoint[0] = new Vector3(floatCenter.x + bounds.extents.x * 0.9f, floatCy, floatCenter.z);
            floatPoint[1] = new Vector3(floatCenter.x - bounds.extents.x * 0.9f, floatCy, floatCenter.z);
            floatPoint[2] = new Vector3(floatCenter.x, floatCy, floatCenter.z + bounds.extents.y);
            floatPoint[3] = new Vector3(floatCenter.x, floatCy, floatCenter.z - bounds.extents.y);

            float floatCy1 = floatCenter.y;
            waterFloatPoint[0] = new Vector3(floatCenter.x + bounds.extents.x * 0.9f, floatCy1, floatCenter.z);
            waterFloatPoint[1] = new Vector3(floatCenter.x - bounds.extents.x * 0.9f, floatCy1, floatCenter.z);
            waterFloatPoint[2] = new Vector3(floatCenter.x, floatCy1, floatCenter.z + bounds.extents.y);
            waterFloatPoint[3] = new Vector3(floatCenter.x, floatCy1, floatCenter.z - bounds.extents.y);

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

            
            float dy = Math.Abs(transform.TransformPoint(floatCenter).y - MainSubmarine.seaLevel);
            for (int i = 0; i < 4; i++)
            {                
                Vector3 worldFloatPoint = transform.TransformPoint(floatPoint[i]);
                Vector3 worldWaterFloatPoint = transform.TransformPoint(waterFloatPoint[i]);
                float water = getWaterHeight(worldFloatPoint);
                float G = Physics.gravity.y * rigidbody.mass;
                float m_deep = water - worldWaterFloatPoint.y;
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
                if (MainSubmarine.isRunSurface && (m_force < G))
                {
                    m_force = G;
                }
                if (i == lookPoint)
                {
                    force = m_force;
                    deep = m_deep;
                }
                Vector3 forceVector = new Vector3(0, m_force * gFloatRate, 0);
                Debug.DrawLine(worldFloatPoint, worldFloatPoint + forceVector, Color.blue);
                rigidbody.AddForceAtPosition(forceVector, worldFloatPoint);
            }

            if (lastInversion * MainSubmarine.inversion < 0)
            {
                isInit = false;
            }
            lastInversion = MainSubmarine.inversion;
        }

        static public float getWaterHeight(Vector3 vector3)
        {
            if (GameSetting.waveMode > 0)
            {
                Vector3 Offset = LuxWaterUtils.GetGestnerDisplacement(vector3, Description, 0);
                return Offset.y;
            }
            else
            {
                return 0;
            }
        }
    }
}