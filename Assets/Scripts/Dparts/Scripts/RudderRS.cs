using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class RudderRS : RunScript
    {
        public Transform rudderTrans;
        public Vector3 rotateAxis;
        public Vector3 forceAxis;

        protected int front, right;
        protected float verticalComponent, horizontalComponent, axisComponent;
        protected Vector3 forceWorldAxis, rotateWorldAxis;
        protected float size;

        private void Awake()
        {
            if (World.GameMode == World.GameMode_Freedom)
            {
                if (Pooler.IS_MainSubmarine_initialized_Finish)
                {
                    enabled = false;
                    return;
                }
            }
        }

        private void OnEnable()
        {
            if (World.GameMode == World.GameMode_Assembler && null != AssemblerRudderCenter.rudders)
            {
                AssemblerRudderCenter.rudders.Add(this);
            }
        }

        private void OnDestroy()
        {
            if (World.GameMode == World.GameMode_Assembler)
            {
                AssemblerRudderCenter.rudders.Remove(this);
            }
        }

        void Start()
        {
            
            if (World.GameMode == World.GameMode_Freedom && enabled)
            {                
                transform.SetParent(World.dpartParentObject.transform);
                front = transform.localPosition.x < MainSubmarine.weightCenter.x ? 1 : -1;
                right = transform.localPosition.z > MainSubmarine.weightCenter.z ? 1 : -1;

                rotateWorldAxis = transform.TransformDirection(rotateAxis);
                verticalComponent = Mathf.Abs(Vector3.Dot(rotateWorldAxis, Vector3.up));
                horizontalComponent = Mathf.Abs(Vector3.Dot(rotateWorldAxis, Vector3.forward));

                forceWorldAxis = transform.TransformDirection(forceAxis);
                axisComponent = Vector3.Dot(forceWorldAxis, Vector3.right);
                if (axisComponent < 0)
                {
                    forceAxis = -forceAxis;
                }

                if (axisComponent * Vector3.Dot(rotateWorldAxis, Vector3.one) < 0)
                {
                    axisComponent = -axisComponent;
                }

                size = IUtils.GetBounds(gameObject).size.magnitude;
            }
        }



        void FixedUpdate()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }

            float forwardTorque = MainSubmarine.speed * 10;
            //垂直分量                   
            float rotateAngle = MainSubmarine.rudderTorque * 30 * verticalComponent * front;

            //水平分量
            rotateAngle += MainSubmarine.pitchTorque * -30 * horizontalComponent;

            //滚转
            rotateAngle += MainSubmarine.rollTorque * 30 * horizontalComponent * right;

            rudderTrans.localEulerAngles = rotateAngle * axisComponent * rotateAxis;


            

            Vector3 forceDir = Quaternion.AngleAxis(180, transform.TransformDirection(forceAxis)) * rudderTrans.TransformDirection(forceAxis) * (MainSubmarine.forward);
            Vector3 force = (forwardTorque * verticalComponent + forwardTorque * horizontalComponent) * forceDir * (Mathf.Abs(rotateAngle) / 30);
            force *= size;
            Debug.DrawLine(transform.position, transform.position + forceDir);
            float water = Buoyancy.getWaterHeight(transform.position);    
            if (transform.position.y > water+1)
            {
                force *= 0.1f;
            }
            MainSubmarine.rigidbody.AddForceAtPosition(force * 3f, transform.position);
        }


    }
}