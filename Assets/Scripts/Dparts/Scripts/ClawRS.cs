using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class ClawRS : RunScript
    {
        public Transform baseTransform;
        public Transform headTransform;
        public Transform head2Transform;
        public float baseSpeed;
        public float headSpeed;
        public Vector3 baseAxis;
        public Vector3 headAxis;
        public float[] baseRotateLimit;
        public float[] headRotateLimit;

        [HideInInspector]
        public SelectorRS selectorRS;

        Vector3 headEulerAngles;
        Vector3 baseEulerAngles;

        float startHeadAngles;
        float startBaseAngles;

        IJoystick joystick1;

        bool enableHeadContron;
        bool enableBaseContron;

        void Start()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }            

            selectorRS = GetComponent<SelectorRS>();

            joystick1 = PoolerItemSelector.instance.GetJoystick1();

            if (headTransform != null)
            {
                startHeadAngles = getVector3Component(headTransform.localEulerAngles, headAxis);
                enableHeadContron = true;
            }
            else
            {
                startHeadAngles = 0f;
                enableHeadContron = false;
            }

            if (baseTransform != null)
            {
                startBaseAngles = getVector3Component(baseTransform.localEulerAngles, baseAxis);
                enableBaseContron = true;
            }
            else
            {
                startBaseAngles = 0f;
                enableBaseContron = false;
            }
        }

        public void setHeadAngle(float angle)
        {
            if (headTransform == null)
            {
                return;
            }
            setAngle(angle, headTransform, startHeadAngles, headRotateLimit, headAxis, headSpeed * 0.5f);
            enableHeadContron = false;
        }

        public void setBaseAngle(float angle)
        {
            setAngle(angle, baseTransform, startBaseAngles, baseRotateLimit, baseAxis, baseSpeed * 0.5f);
            enableBaseContron = false;
        }

        public void setAngle(float angle, Transform rotateTransform, float startAngle, float[] rotateLimit, Vector3 axis, float speed)
        {
            if (rotateTransform == null)
            {
                return;
            }
            Vector3 orgEulerAngles = rotateTransform.localEulerAngles;
            float targetAngle = rotatelimit(startAngle + angle, rotateLimit);
            float orgAngles = IUtils.angleRoundIn180(getVector3Component(rotateTransform.localEulerAngles, axis));           
            float dAngle = targetAngle - orgAngles;
            float dAngle360 = IUtils.reviseAngleIn360(targetAngle - orgAngles);
            float lerpAngle = orgAngles;
            if (dAngle360 > 0.2f)
            {
                lerpAngle += (dAngle360 > 180 ? -1 : 1) * (dAngle360 > 1 ? speed : 0.2f * speed) * Time.deltaTime;
            }else if(dAngle360 > 0.1f)
            {
                lerpAngle = targetAngle;
            }
            lerpAngle = rotatelimit(lerpAngle, rotateLimit);
            rotateTransform.localEulerAngles = IUtils.vector3ComponeMUL(orgEulerAngles, (Vector3.one - axis)) + lerpAngle * axis;          
        }

        void Update()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }

            if (selectorRS.isSelecting && joystick1.isPointed)
            {
                headRotate();
                baseRotate();
            }
        }

        void headRotate()
        {            
            if (headTransform == null)
            {
                return;
            }

            if (enableHeadContron)
            {
                headEulerAngles = headTransform.localEulerAngles;
                float angle = rotatelimit(getVector3Component(headEulerAngles, headAxis) - joystick1.y * headSpeed * Time.deltaTime, headRotateLimit);
                headTransform.localEulerAngles = IUtils.vector3ComponeMUL(headEulerAngles, (Vector3.one - headAxis)) + angle * headAxis;
                head2Transform.localEulerAngles = IUtils.vector3ComponeMUL(headEulerAngles, (Vector3.one - headAxis)) - angle * headAxis;
            }
            else
            {
                enableHeadContron = true;
            }            
        }

        void baseRotate()
        {
            if (enableBaseContron)
            {
                baseEulerAngles = baseTransform.localEulerAngles;
                float angle = rotatelimit(getVector3Component(baseEulerAngles, baseAxis) + joystick1.x * baseSpeed * Time.deltaTime, baseRotateLimit);
                baseTransform.localEulerAngles = IUtils.vector3ComponeMUL(baseEulerAngles, (Vector3.one - baseAxis)) + angle * baseAxis;
            }
            else
            {
                enableBaseContron = true;
            }           
        }

        float rotatelimit(float angle, float[] limit)
        {
            if (limit != null && limit.Length == 2)
            {
                angle = IUtils.angleRoundIn180(angle);
                angle = Mathf.Clamp(angle, limit[0], limit[1]);
            }
            return angle;
        }

        float getVector3Component(Vector3 vector3, Vector3 axis)
        {
            if (axis.x == 1)
            {
                return vector3.x;
            }
            if (axis.y == 1)
            {
                return vector3.y;
            }
            if (axis.z == 1)
            {
                return vector3.z;
            }
            return 0;
        }
    }
}