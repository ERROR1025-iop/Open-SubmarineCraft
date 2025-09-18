using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.DpartSpace;
using Scraft;

namespace Scraft.DpartSpace
{
    public class WingRS : RudderRS
    {
        Quaternion forceRotate = Quaternion.Euler(new Vector3(0, 0, 90));

        void FixedUpdate()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }

            float forwardTorque = MainSubmarine.speed * 10;
            //垂直分量                   
            float rotateAngle = MainSubmarine.pitchTorque * 5 * horizontalComponent * front;

            //水平分量       
            rotateAngle += MainSubmarine.rudderTorque * 2 * horizontalComponent * right;
            rudderTrans.localEulerAngles = rotateAngle * axisComponent * rotateAxis;

            Vector3 forceDir = Quaternion.AngleAxis(180, transform.TransformDirection(forceAxis)) * rudderTrans.TransformDirection(forceAxis) * (MainSubmarine.forward);
            forceDir = forceRotate * forceDir;
            Vector3 force = (forwardTorque * horizontalComponent) * forceDir * (Mathf.Abs(rotateAngle) / 30);
            
            force *= size;
            Debug.DrawLine(transform.position, transform.position + forceDir);            
            MainSubmarine.rigidbody.AddForceAtPosition(force, transform.position);
        }
    }
}
