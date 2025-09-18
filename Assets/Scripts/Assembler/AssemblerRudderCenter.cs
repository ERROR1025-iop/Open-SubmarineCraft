using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.DpartSpace;

namespace Scraft
{
    public class AssemblerRudderCenter : MonoBehaviour
    {
        static public AssemblerRudderCenter instance;

        static public List<RudderRS> rudders;

        Vector3 center;

        void Awake()
        {
            instance = this;

            rudders = new List<RudderRS>();
        }


        void Update()
        {
            if (!Assembler.IS_Show_WeighCenter)
            {
                transform.position = new Vector3(0, 9999, 0);
                return;
            }

            center = Vector3.zero;
            int count = 0;
            for (int i = 0; i < rudders.Count; i++)
            {
                RudderRS rudder = rudders[i];
                if (rudder != null && rudder.isActiveAndEnabled)
                {                    
                    center += rudder.transform.position;
                    count++;
                }
            }

            if (count > 0)
            {              
                transform.position = center / count;              
            }
            else
            {
                transform.position = new Vector3(0, 9999, 0);
            }
        }
    }
}