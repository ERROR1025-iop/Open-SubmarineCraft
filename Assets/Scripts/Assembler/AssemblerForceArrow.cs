using Scraft.DpartSpace;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class AssemblerForceArrow : MonoBehaviour
    {
        static public AssemblerForceArrow instance;

        static public List<PropellerRS> propellers;

        public Vector3 axis;

        Vector3 forceVector;
        Vector3 forceCenter;

        void Awake()
        {
            instance = this;

            propellers = new List<PropellerRS>();
        }


        void Update()
        {
            if (!Assembler.IS_Show_WeighCenter)
            {
                transform.position = new Vector3(0, 9999, 0);
                return;
            }

            forceVector = Vector3.zero;
            forceCenter = Vector3.zero;
            int count = 0;
            for (int i = 0; i < propellers.Count; i++)
            {
                PropellerRS propeller = propellers[i];
                if (propeller != null && propeller.isActiveAndEnabled)
                {
                    forceVector += propeller.getForceVector();
                    forceCenter += propeller.transform.position;
                    count++;
                }
            }

            if (count > 0)
            {
                forceCenter = forceCenter / count;
                forceVector = forceVector.normalized;
                transform.position = forceCenter;
                transform.rotation = Quaternion.FromToRotation(axis, forceVector);
                //Debug.DrawLine(transform.position, transform.position + forceVector * 100, Color.red);
            }
            else
            {
                transform.position = new Vector3(0, 9999, 0);
            }
        }
    }
}