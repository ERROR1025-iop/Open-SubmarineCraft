using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft {
    public class LowBuoyancy : MonoBehaviour
    {
        new Rigidbody rigidbody;
        Vector3 floatCenter, l_floatCenter;
        public float tonnage;
        float waterHeight = 0;        

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.drag = 1;
        }

        public void Initialized(float tonnage, Vector3 weightCenter, float shipOffsetY)
        {
            this.tonnage = tonnage;      
            l_floatCenter = new Vector3(weightCenter.x, 0, weightCenter.z);
        }

        private void OnDrawGizmosSelected()
        {
            if (floatCenter != null )
            {
                Gizmos.DrawWireSphere(floatCenter, 0.2f);
            }
        }

        void Update()
        {
            buoyancy();
            stabilization();
        }

        void buoyancy()
        {
            floatCenter = transform.TransformPoint(l_floatCenter);
            float h = 5f + Mathf.Clamp(floatCenter.y, -0.241f, 0) * 20;

            float deep = waterHeight - floatCenter.y;
            deep = deep > 0 ? Mathf.Min(deep, h) : Mathf.Max(deep, -h);
            deep *= 2;
            float G = Physics.gravity.y * rigidbody.mass;
            float floatForce = -G * (deep + 1) + ((floatCenter.y > 0 && tonnage > 0) ? 0 : tonnage);
            floatForce = Mathf.Max(0, floatForce);
            float force = floatForce + G;
            Vector3 forceVector = new Vector3(0, force, 0);
            Debug.DrawLine(floatCenter, floatCenter + forceVector, Color.blue);
            rigidbody.AddForceAtPosition(forceVector, floatCenter);
        }

        void stabilization()
        {
            float lerpSpeed = 5;
            Vector3 roto = transform.rotation.eulerAngles;
            if (roto.x != 0 || roto.z != 0)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, roto.y, 0)), lerpSpeed * Time.deltaTime);
            }
        }

    }
}
