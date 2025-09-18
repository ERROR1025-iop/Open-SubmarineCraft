using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.DpartSpace;
using Scraft;

namespace Scraft.DpartSpace
{
    public class AirscrewRS : PropellerRS
    {
        void Update()
        {
            if (World.GameMode == World.GameMode_Freedom)
            {
                force = isOpen ? MainSubmarine.forwardForce : 0;
                speed = Mathf.Lerp(speed, force, 0.01f);
                if (backBubble != null)
                {
                    backBubble.emissionRate = speed > 0 ? speed * 10 : 0;
                }
                if (propellerTrans != null)
                {
                    propellerTrans.Rotate(-forwardAxis, speed * Time.deltaTime * (transform.position.y < Buoyancy.waterHeight ? 0.1f : 100));
                }
            }
        }

        void FixedUpdate()
        {
            if (World.GameMode == World.GameMode_Freedom && enabled)
            {
                if (Pooler.mass == 0)
                {
                    return;
                }

                forwardDir = -transform.up;
                Vector3 forwardForce = isOpen ? (MainSubmarine.forwardForce * 3 / propeller_count) * forwardDir : Vector3.zero;

                if (transform.position.y < Buoyancy.waterHeight)
                {
                    forwardForce *= 0.1f;
                }
                //Debug.DrawLine(transform.position, transform.position + forwardDir * 100, Color.blue);
                MainSubmarine.rigidbody.AddForceAtPosition(forwardForce, transform.position);
            }
        }
    }
}
