using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft {
    public class AISpawn : MonoBehaviour
    {
        static public List<AISpawn> aISpawns;

        public float radius;

        void Awake()
        {
            if(aISpawns == null)
            {
                aISpawns = new List<AISpawn>();
            }

            aISpawns.Add(this);
        }

        private void OnDestroy()
        {
            aISpawns.Remove(this);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
