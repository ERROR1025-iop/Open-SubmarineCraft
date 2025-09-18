using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class Rotation3RS : RunScript
    {
        public Transform base1Transform;
        public Transform head1Transform;
        public Transform base2Transform;
        public Transform head2Transform;
        public Transform base3Transform;
        public Transform head3Transform;
        public float base1Speed;
        public float head1Speed;
        public float base2Speed;
        public float head2Speed;
        public float base3Speed;
        public float head3Speed;
        public Vector3 base1Axis;
        public Vector3 head1Axis;
        public Vector3 base2Axis;
        public Vector3 head2Axis;
        public Vector3 base3Axis;
        public Vector3 head3Axis;
        public float[] base1RotateLimit;
        public float[] head1RotateLimit;
        public float[] base2RotateLimit;
        public float[] head2RotateLimit;
        public float[] base3RotateLimit;
        public float[] head3RotateLimit;

        [HideInInspector]
        public SelectorRS selectorRS;

        Vector3 head1EulerAngles;
        Vector3 base1EulerAngles;
        Vector3 head2EulerAngles;
        Vector3 base2EulerAngles;
        Vector3 head3EulerAngles;
        Vector3 base3EulerAngles;

        float startHead1Angles;
        float startBase1Angles;
        float startHead2Angles;
        float startBase2Angles;
        float startHead3Angles;
        float startBase3Angles;

        IJoystick joystick1;
        IJoystick joystick2;
        IJoystick joystick3;

        bool enableHead1Contron;
        bool enableBase1Contron;
        bool enableHead2Contron;
        bool enableBase2Contron;
        bool enableHead3Contron;
        bool enableBase3Contron;

        void Start()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }

            selectorRS = GetComponent<SelectorRS>();

            joystick1 = PoolerItemSelector.instance.GetJoystick1();
            joystick2 = PoolerItemSelector.instance.GetJoystick2();
            joystick3 = PoolerItemSelector.instance.GetJoystick3();

            startHead1Angles = getVector3Component(head1Transform.localEulerAngles, head1Axis);
            startBase1Angles = getVector3Component(base1Transform.localEulerAngles, base1Axis);
            startHead2Angles = getVector3Component(head2Transform.localEulerAngles, head2Axis);
            startBase2Angles = getVector3Component(base2Transform.localEulerAngles, base2Axis);
            startHead3Angles = getVector3Component(head2Transform.localEulerAngles, head2Axis);
            startBase3Angles = getVector3Component(base2Transform.localEulerAngles, base2Axis);

            enableHead1Contron = true;
            enableBase1Contron = true;
            enableHead2Contron = true;
            enableBase2Contron = true;
            enableHead3Contron = true;
            enableBase3Contron = true;
        }

        public void setHead1Angle(float angle)
        {
            setAngle(angle, head1Transform, startHead1Angles, head1RotateLimit, head1Axis, head1Speed * 0.5f);
            enableHead1Contron = false;
        }

        public void setBase1Angle(float angle)
        {
            setAngle(angle, base1Transform, startBase1Angles, base1RotateLimit, base1Axis, base1Speed * 0.5f);
            enableBase1Contron = false;
        }

        public void setHead2Angle(float angle)
        {
            setAngle(angle, head2Transform, startHead2Angles, head2RotateLimit, head2Axis, head2Speed * 0.5f);
            enableHead2Contron = false;
        }

        public void setBase2Angle(float angle)
        {
            setAngle(angle, base2Transform, startBase2Angles, base2RotateLimit, base2Axis, base2Speed * 0.5f);
            enableBase2Contron = false;
        }

        public void setHead3Angle(float angle)
        {
            setAngle(angle, head3Transform, startHead3Angles, head3RotateLimit, head3Axis, head3Speed * 0.5f);
            enableHead3Contron = false;
        }

        public void setBase3Angle(float angle)
        {
            setAngle(angle, base3Transform, startBase3Angles, base3RotateLimit, base3Axis, base3Speed * 0.5f);
            enableBase3Contron = false;
        }

        public void setAngle(float angle, Transform rotateTransform, float startAngle, float[] rotateLimit, Vector3 axis, float speed)
        {
            Vector3 orgEulerAngles = rotateTransform.localEulerAngles;
            float targetAngle = rotatelimit(startAngle + angle, rotateLimit);
            float orgAngles = IUtils.angleRoundIn180(getVector3Component(rotateTransform.localEulerAngles, axis));
            float dAngle = targetAngle - orgAngles;
            float dAngle360 = IUtils.reviseAngleIn360(targetAngle - orgAngles);
            float lerpAngle = orgAngles;
            if (dAngle360 > 0.2f)
            {
                lerpAngle += (dAngle360 > 180 ? -1 : 1) * (dAngle360 > 1 ? speed : 0.2f * speed) * Time.deltaTime;
            }
            else if (dAngle360 > 0.1f)
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

            if (selectorRS.isSelecting)
            {
                if (joystick1.isPointed)
                {
                    head1Rotate();
                    base1Rotate();
                }
                if (joystick2.isPointed)
                {
                    head2Rotate();
                    base2Rotate();
                }
                if (joystick3.isPointed)
                {
                    head3Rotate();
                    base3Rotate();
                }
            }
        }

        void head1Rotate()
        {
            if (enableHead1Contron)
            {
                head1EulerAngles = head1Transform.localEulerAngles;
                float angle = rotatelimit(getVector3Component(head1EulerAngles, head1Axis) - joystick1.y * head1Speed * Time.deltaTime, head1RotateLimit);
                head1Transform.localEulerAngles = IUtils.vector3ComponeMUL(head1EulerAngles, (Vector3.one - head1Axis)) + angle * head1Axis;
            }
            else
            {
                enableHead1Contron = true;
            }
        }

        void base1Rotate()
        {
            if (enableBase1Contron)
            {
                base1EulerAngles = base1Transform.localEulerAngles;
                float angle = rotatelimit(getVector3Component(base1EulerAngles, base1Axis) + joystick1.x * base1Speed * Time.deltaTime, base1RotateLimit);
                base1Transform.localEulerAngles = IUtils.vector3ComponeMUL(base1EulerAngles, (Vector3.one - base1Axis)) + angle * base1Axis;
            }
            else
            {
                enableBase1Contron = true;
            }
        }

        void head2Rotate()
        {
            if (enableHead2Contron)
            {
                head2EulerAngles = head2Transform.localEulerAngles;
                float angle = rotatelimit(getVector3Component(head2EulerAngles, head2Axis) - joystick2.y * head2Speed * Time.deltaTime, head2RotateLimit);
                head2Transform.localEulerAngles = IUtils.vector3ComponeMUL(head2EulerAngles, (Vector3.one - head2Axis)) + angle * head2Axis;
            }
            else
            {
                enableHead2Contron = true;
            }
        }

        void base2Rotate()
        {
            if (enableBase2Contron)
            {
                base2EulerAngles = base2Transform.localEulerAngles;
                float angle = rotatelimit(getVector3Component(base2EulerAngles, base2Axis) + joystick2.x * base2Speed * Time.deltaTime, base2RotateLimit);
                base2Transform.localEulerAngles = IUtils.vector3ComponeMUL(base2EulerAngles, (Vector3.one - base2Axis)) + angle * base2Axis;
            }
            else
            {
                enableBase2Contron = true;
            }
        }

        void head3Rotate()
        {
            if (enableHead3Contron)
            {
                head3EulerAngles = head3Transform.localEulerAngles;
                float angle = rotatelimit(getVector3Component(head3EulerAngles, head3Axis) - joystick3.y * head3Speed * Time.deltaTime, head3RotateLimit);
                head3Transform.localEulerAngles = IUtils.vector3ComponeMUL(head3EulerAngles, (Vector3.one - head3Axis)) + angle * head3Axis;
            }
            else
            {
                enableHead3Contron = true;
            }
        }

        void base3Rotate()
        {
            if (enableBase3Contron)
            {
                base3EulerAngles = base3Transform.localEulerAngles;
                float angle = rotatelimit(getVector3Component(base3EulerAngles, base3Axis) + joystick3.x * base3Speed * Time.deltaTime, base3RotateLimit);
                base3Transform.localEulerAngles = IUtils.vector3ComponeMUL(base3EulerAngles, (Vector3.one - base3Axis)) + angle * base3Axis;
            }
            else
            {
                enableBase3Contron = true;
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