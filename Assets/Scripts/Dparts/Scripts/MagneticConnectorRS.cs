using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class MagneticConnectorRS : RunScript
    {       
        public MagneticConnector magneticConnector;      
        void Start()
        {
            if (World.GameMode == World.GameMode_Freedom && enabled)
            {
                magneticConnector.gameObject.layer = 17; // MagneticConnector layer
                PoolerItemSelector.instance.OnCustom1ButtonClick += OnCustom1ButtonClick;
            }
        }

        void OnCustom1ButtonClick()
        {
            magneticConnector.Detach();
        }
    }
}