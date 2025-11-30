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
            float x = 0;
            float y = 0;
            bool flag = false;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PoolerUI.instance.onMenuButtonClick();
            }

            if (InputController.GetKeyActionIsActive("Turn left"))
            {
                x = -0.5f;
                flag = true;
            }
            else if (InputController.GetKeyActionIsActive("Turn right"))
            {
                x = 0.5f;
                flag = true;
            }

            if (InputController.GetKeyActionIsUp("Turn left") || InputController.GetKeyActionIsUp("Turn right"))
            {
                x = 0f;
                flag = true;
            }
            if (InputController.GetKeyActionIsActive("Pitch up"))
            {
                y = -0.5f;
                flag = true;
            }
            else if (InputController.GetKeyActionIsActive("Pitch down"))
            {
                y = 0.5f;
                flag = true;
            }
            if (InputController.GetKeyActionIsUp("Pitch up") || InputController.GetKeyActionIsUp("Pitch down"))
            {
                y = 0f;
                flag = true;
            }

            if (flag)
            {                
                PoolerUI.instance.dirController.SetValue(x, y);
                PoolerUI.instance.dirController.changing = true;
            }

            if (InputController.GetKeyActionIsActive("Roll Left"))
            {
                PoolerUI.instance.rollBar.setValue(0);
                PoolerUI.instance.onRollBarPush();
            }
            else if (InputController.GetKeyActionIsActive("Roll Right"))
            {
                PoolerUI.instance.rollBar.setValue(1);
                PoolerUI.instance.onRollBarPush();
            }
            if (InputController.GetKeyActionIsUp("Roll Left") || InputController.GetKeyActionIsUp("Roll Right"))
            {
                PoolerUI.instance.rollBar.setValue(0.5f);
                PoolerUI.instance.onRollBarPush();
            }

            flag = false;
            x = 0;
            y = 0;
            if (InputController.GetKeyActionIsActive("Joint 1 Up"))
            {
                y = 0.5f;
                flag = true;
            }
            if (InputController.GetKeyActionIsActive("Joint 1 Down"))
            {
                y = -0.5f;
                flag = true;
            }
            if (InputController.GetKeyActionIsUp("Joint 1 Up") || InputController.GetKeyActionIsUp("Joint 1 Down"))
            {
                y = 0f;
                flag = true;
            }

            if (InputController.GetKeyActionIsActive("Joint 1 Left"))
            {
                x = -0.5f;
                flag = true;
            }
            if (InputController.GetKeyActionIsActive("Joint 1 Right"))
            {
                x = 0.5f;
                flag = true;
            }
            if (InputController.GetKeyActionIsUp("Joint 1 Left") || InputController.GetKeyActionIsUp("Joint 1 Right"))
            {
                x = 0f;
                flag = true;
            }


            if (flag)
            {       
                var joy1 = PoolerItemSelector.instance.GetJoystick1();         
                joy1.SetValue(x, y);
                joy1.changing = true;
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