using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Scraft
{
    public class PoolBlockParams : MonoBehaviour
    {

        static public PoolBlockParams instance;
        public float temperature_diff = 5;

        void Awake()
        {
            instance = this;
        }
    }
}
