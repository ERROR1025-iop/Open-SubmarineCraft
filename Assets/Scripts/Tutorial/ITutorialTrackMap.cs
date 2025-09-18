using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft {
    public class ITutorialTrackMap : MonoBehaviour
    {

        public Vector3 worldPos;

        void Start()
        {
            if(worldPos == Vector3.zero)
            {
                worldPos = Camera.main.ScreenToWorldPoint(transform.position);
            }            
        }

        void Update()
        {
            transform.position = Camera.main.WorldToScreenPoint(worldPos);
        }
    }
}
