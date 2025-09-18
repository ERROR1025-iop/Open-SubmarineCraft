using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class AimRS : RunScript
    {
        public Transform cameraPoint;
        public bool showAimMask;

        SelectorRS selectorRS;

        void Start()
        {            
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }            

            PoolerItemSelector.instance.OnCustom1ButtonClick += OnCustom1ButtonClick;
            PoolerItemSelector.instance.OnCancelButtonClick += OnCancelButtonClick;

            selectorRS = GetComponent<SelectorRS>();

            if (transform.localScale.z < 0)
            {
                cameraPoint.Rotate(new Vector3(180, 0, 180));
            }           
        }

        void OnCustom1ButtonClick()
        {
            if (selectorRS.isMainSelecting)
            {
                SubCamera.instance.setAimMode(!SubCamera.isAimMode, cameraPoint, Vector3.zero, showAimMask);
                PoolerItemSelector.instance.setCustom1ButtonText(ILang.get(SubCamera.isAimMode ? "Cancel" : "Watch"));
            }
        }

        void OnCancelButtonClick()
        {
            SubCamera.instance.setAimMode(false, cameraPoint, Vector3.zero, false);
        }

        private void OnDestroy()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                return;
            }
            PoolerItemSelector.instance.OnCustom1ButtonClick -= OnCustom1ButtonClick;
            PoolerItemSelector.instance.OnCancelButtonClick -= OnCancelButtonClick;
        }
    }
}