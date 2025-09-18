using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scraft.DpartSpace
{
    public class DrillPointRS : MonoBehaviour
    {
        public bool isTouchTerrain;

        string terrainTag = "terrain";

        void Start()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                Destroy(this);
                return;
            }          

            isTouchTerrain = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(terrainTag))
            {
                isTouchTerrain = true;
            }           
        }

        private void OnTriggerExit(Collider other)
        {
            isTouchTerrain = false;
        }
    }
}
