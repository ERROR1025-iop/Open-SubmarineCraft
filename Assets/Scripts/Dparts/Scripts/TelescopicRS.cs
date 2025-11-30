using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class TelescopicRS : RunScript
    {
        public Transform base1Transform;
        public Transform base2Transform;
        [HideInInspector]
        public SelectorRS selectorRS;
        public float speed = 10f;
        IJoystick joystick1;
        public float[] base1MoveLimit;
        public float[] base2MoveLimit;
        void Start()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }            

            selectorRS = GetComponent<SelectorRS>();

            joystick1 = PoolerItemSelector.instance.GetJoystick1();            
        }

        void Update()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }

            if (selectorRS.isSelecting && joystick1.isPointed)
            {
                // Use joystick1.x for both bases. Move base1 first; if it hits its limit
                // and there is leftover movement, apply that leftover to base2.
                float joy = -joystick1.y;
                float delta = joy * speed * Time.deltaTime;
                float leftover = MoveApply(base1Transform, delta, base1MoveLimit);
                if (Mathf.Abs(leftover) > 0f)
                {
                    MoveApply(base2Transform, leftover, base2MoveLimit);
                }
            }   
        }

        // Apply a movement delta (in world units) to transform's local z position.
        // Returns leftover delta that couldn't be applied because the target hit its limit
        // and the movement direction matches the leftover sign. Otherwise returns 0.
        float MoveApply(Transform t, float delta, float[] limit)
        {
            var p = t.localPosition;
            float target = p.z + delta;
            float clamped = target;
            if (limit.Length == 2)
            {
                clamped = Mathf.Clamp(target, limit[0], limit[1]);
            }
            t.localPosition = new Vector3(0, 0, clamped);

            float applied = clamped - p.z; // actual applied movement
            float leftover = delta - applied; // remaining movement

            if (Mathf.Abs(leftover) <= 0f)
                return 0f;

            // If clamped to a bound and leftover has same sign as attempted movement,
            // return leftover so it can be applied to the next segment.
            if (limit.Length == 2)
            {
                if (delta > 0f && Mathf.Approximately(clamped, limit[1]))
                    return leftover;
                if (delta < 0f && Mathf.Approximately(clamped, limit[0]))
                    return leftover;
            }

            return 0f;
        }
    }
}