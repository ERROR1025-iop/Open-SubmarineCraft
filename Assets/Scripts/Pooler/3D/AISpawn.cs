using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft {
    public class AISpawn : AreaDetector
    {
        static public List<AISpawn> aISpawns;

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
    }
}
