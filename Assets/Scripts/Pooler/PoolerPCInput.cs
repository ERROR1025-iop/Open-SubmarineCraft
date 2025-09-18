using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InsaneSystems.InputManager;

namespace Scraft
{
    public class PoolerPCInput : MonoBehaviour
    {

        static public bool isPitchBarClick;

        void Start()
        {
            isPitchBarClick = false;
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PoolerUI.instance.onMenuButtonClick();
            }

            if (InputController.GetKeyActionIsDown("Turn left"))
            {
                PoolerUI.instance.rudder.setValue(0);
                PoolerUI.instance.onRudderPush();
            }
            else if (InputController.GetKeyActionIsDown("Turn right"))
            {
                PoolerUI.instance.rudder.setValue(1);
                PoolerUI.instance.onRudderPush();
            }

            if (InputController.GetKeyActionIsUp("Turn left") || InputController.GetKeyActionIsUp("Turn right"))
            {
                PoolerUI.instance.rudder.setValue(0.5f);
                PoolerUI.instance.onRudderPush();
            }

            if (InputController.GetKeyActionIsDown("Pitch up"))
            {
                PoolerUI.instance.pitchBar.setValue(0);
                PoolerUI.instance.onPitchBarPush();
                isPitchBarClick = true;
            }
            else if (InputController.GetKeyActionIsDown("Pitch down"))
            {
                PoolerUI.instance.pitchBar.setValue(1);
                PoolerUI.instance.onPitchBarPush();
                isPitchBarClick = true;
            }

            if (InputController.GetKeyActionIsUp("Pitch up") || InputController.GetKeyActionIsUp("Pitch down"))
            {
                PoolerUI.instance.pitchBar.setValue(0.5f);
                PoolerUI.instance.onPitchBarPush();
                isPitchBarClick = false;
            }

            if (InputController.GetKeyActionIsDown("Fire"))
            {
                PoolerUI.instance.onfireButtonClick();
            }

            if (InputController.GetKeyActionIsDown("Float"))
            {
                PoolerUI.instance.upButton.setValue(true);
                PoolerUI.instance.onUpButtonValueChanged();
            }

            if (InputController.GetKeyActionIsDown("Dive"))
            {
                PoolerUI.instance.downButton.setValue(true);
                PoolerUI.instance.onDownButtonValueChanged();
            }

            if (InputController.GetKeyActionIsUp("Float"))
            {
                PoolerUI.instance.upButton.setValue(false);
                PoolerUI.instance.onUpButtonValueChanged();
            }

            if (InputController.GetKeyActionIsUp("Dive"))
            {
                PoolerUI.instance.downButton.setValue(false);
                PoolerUI.instance.onDownButtonValueChanged();
            }

            if (InputController.GetKeyActionIsDown("Auto balance"))
            {
                PoolerUI.instance.autoButton.setValue(!PoolerUI.instance.autoButton.value);
            }

            if (InputController.GetKeyActionIsDown("Switch view"))
            {
                PoolerUI.instance.periscopeButton.setValue(!PoolerUI.instance.periscopeButton.value);
                PoolerUI.instance.onPeriscopeButtonClick();
            }

            if (InputController.GetKeyActionIsDown("Open light"))
            {
                PoolerUI.instance.lightButton.setValue(!PoolerUI.instance.lightButton.value);
                PoolerUI.instance.onlightButtonClick();
            }

            if (InputController.GetKeyActionIsDown("Hide radar"))
            {
                PoolerUI.instance.radarButton.setValue(!PoolerUI.instance.radarButton.value);
                PoolerUI.instance.onRadarButtonClick();
            }

            if (InputController.GetKeyActionIsDown("Hide small window"))
            {
                PoolerUI.instance.viewButton.setValue(!PoolerUI.instance.viewButton.value);
                PoolerUI.instance.onViewButtonClick();
            }

            if (InputController.GetKeyActionIsDown("Switch weapon"))
            {
                //WeaponSwitch.instance.moveNextWeapon();
            }

            if (InputController.GetKeyActionIsActive("Increase thrust"))
            {
                PoolerUI.instance.powerBar.addValue(0.02f);
                PoolerUI.instance.onPowerBarPush();
            }
            else if (InputController.GetKeyActionIsActive("Reduce thrust"))
            {
                PoolerUI.instance.powerBar.addValue(-0.02f);
                PoolerUI.instance.onPowerBarPush();
            }
            else if (InputController.GetKeyActionIsDown("Full thrust"))
            {
                PoolerUI.instance.powerBar.setValue(1);
                PoolerUI.instance.onPowerBarPush();
            }
            else if (InputController.GetKeyActionIsDown("Stop thrust"))
            {
                PoolerUI.instance.powerBar.setValue(0);
                PoolerUI.instance.onPowerBarPush();
            }
            else if (InputController.GetKeyActionIsDown("Switch Forward/Back"))
            {
                PoolerUI.instance.directionButton.setValue(!PoolerUI.instance.directionButton.value);
                PoolerUI.instance.onDirectionButtonClick();
            }

            if (InputController.GetKeyActionIsDown("Custom button 1"))
            {
                PoolerUI.instance.comButton1.setValue(true);
                PoolerUI.instance.onComButton1ValueChanged();
            }

            if (InputController.GetKeyActionIsDown("Custom button 2"))
            {
                PoolerUI.instance.comButton2.setValue(true);
                PoolerUI.instance.onComButton2ValueChanged();
            }

            if (InputController.GetKeyActionIsUp("Custom button 1"))
            {
                PoolerUI.instance.comButton1.setValue(false);
                PoolerUI.instance.onComButton1ValueChanged();
            }

            if (InputController.GetKeyActionIsUp("Custom button 2"))
            {
                PoolerUI.instance.comButton2.setValue(false);
                PoolerUI.instance.onComButton2ValueChanged();
            }
        }
    }
}