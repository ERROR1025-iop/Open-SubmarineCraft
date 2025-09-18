using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class PropellerRS : RunScript
    {
        public Transform propellerTrans;
        public ParticleSystem backBubble;
        protected Vector3 forwardAxis;
        protected Vector3 forwardDir, rightDir;
        protected float force;
        static protected int propeller_count;
        static protected int propeller_active_count;

        protected SelectorRS selectorRS;
        protected bool isOpen;
        protected float speed;

        void Awake()
        {
            
            if (World.GameMode == World.GameMode_Freedom)
            {
                if (Pooler.IS_MainSubmarine_initialized_Finish)
                {
                    enabled = false;
                    if (backBubble != null)
                        backBubble.emissionRate = 0;
                    return;
                }
                propeller_count = 0;
                propeller_active_count = 0;
            }
                       
        }

        void Start()
        {                        
            forwardAxis = Vector3.up;
            if (World.GameMode == World.GameMode_Freedom)
            {
                propeller_count++;
                propeller_active_count++;
                selectorRS = GetComponent<SelectorRS>();
                PoolerItemSelector.instance.OnCustom1ButtonClick += OnCustom1ButtonClick;
                PoolerItemSelector.instance.OnCancelButtonClick += OnCancelButtonClick;
                isOpen = true;
                speed = 0;
            }
            else if (backBubble != null)
            {
                backBubble.emissionRate = 0;
            }
        }

        void OnCustom1ButtonClick()
        {
            if (selectorRS.isSelecting)
            {
                isOpen = !isOpen;
                if (isOpen)
                {
                    propeller_active_count++;
                }
                else
                {
                    propeller_active_count--;
                }
                PoolerItemSelector.instance.setCustom1ButtonText(ILang.get(isOpen ? "Stop" : "Open"));
            }
        }

        void OnCancelButtonClick()
        {

        }

        public Vector3 getForceVector()
        {
            forwardAxis = transform.up;
            return forwardAxis;
        }

        private void OnEnable()
        {
            if (World.GameMode == World.GameMode_Assembler)
            {
                AssemblerForceArrow.propellers.Add(this);
            }
        }

        private void OnDestroy()
        {
            if (World.GameMode == World.GameMode_Assembler)
            {
                AssemblerForceArrow.propellers.Remove(this);
            }
            if (World.GameMode == World.GameMode_Freedom)
            {
                PoolerItemSelector.instance.OnCustom1ButtonClick -= OnCustom1ButtonClick;
                PoolerItemSelector.instance.OnCancelButtonClick -= OnCancelButtonClick;
            }
           
        }

        void Update()
        {
            if (World.GameMode == World.GameMode_Freedom)
            {                
                force = isOpen? MainSubmarine.forwardForce : 0;
                speed = Mathf.Lerp(speed, force, 0.02f);
                if (backBubble != null)
                {
                    backBubble.emissionRate = speed > 0 ? speed * 10 : 0;
                }
                if (propellerTrans != null)
                {
                    propellerTrans.Rotate(-forwardAxis, speed * Time.deltaTime * 100);
                }
            }
        }

        void FixedUpdate()
        {
            if (World.GameMode == World.GameMode_Freedom && enabled)
            {
                if (Pooler.mass == 0 || propeller_active_count <= 0)
                {
                    return;
                }

                forwardDir = -transform.up;
                Vector3 forwardForce = isOpen ? (MainSubmarine.forwardForce * 3 / propeller_count) * forwardDir : Vector3.zero;

                if (transform.position.y > Buoyancy.waterHeight)
                {
                    forwardForce *= 0.1f;
                }
                //Debug.DrawLine(transform.position, transform.position + forwardDir * 100, Color.blue);
                MainSubmarine.rigidbody.AddForceAtPosition(forwardForce, transform.position);
            }
        }


    }    
}