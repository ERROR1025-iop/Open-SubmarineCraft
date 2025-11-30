using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Boxophobic.StyledGUI;

namespace Scraft
{
    public class IJoystickV2 : IJoystick
    {
        public IPressButton[] pressButtons = new IPressButton[4];
        public bool[] values = new bool[4];

        void Start()
        {
            pressButtons[0] = transform.GetChild(0).GetComponent<IPressButton>();  // up
            pressButtons[1] = transform.GetChild(1).GetComponent<IPressButton>();  // down
            pressButtons[2] = transform.GetChild(2).GetComponent<IPressButton>();  // left
            pressButtons[3] = transform.GetChild(3).GetComponent<IPressButton>();  // right
           
            pressButtons[0].setValueChangeListener(OnUpClick);
            pressButtons[1].setValueChangeListener(OnDownClick);
            pressButtons[2].setValueChangeListener(OnLeftClick);
            pressButtons[3].setValueChangeListener(OnRightClick);
            isPointed = false;
        }

        private void OnUpClick()
        {
            values[0] = pressButtons[0].value;            
        }

        private void OnDownClick()
        {
            values[1] = pressButtons[1].value;
        }

        private void OnLeftClick()
        {
            values[2] = pressButtons[2].value;
        }

        private void OnRightClick()
        {
            values[3] = pressButtons[3].value;
        }

        void Update()
        {
            isPointed = values[0] || values[1] || values[2] || values[3];
            x = ((values[3] ? 1 : 0) - (values[2] ? 1 : 0)) * 0.5f;
            y = ((values[0] ? 1 : 0) - (values[1] ? 1 : 0)) * 0.5f;
        }

        override public void SetValue(float x, float y)
        {
            //pressButtons和values内部元素都不能为空，否则直接返回
            if (pressButtons == null || values == null) return;
            if (pressButtons.Length < 4 || values.Length < 4) return;
            for (int i = 0; i < pressButtons.Length; i++)
            {
                if (pressButtons[i] == null) return;
            }
            if (x > 0.25f)
            {
                pressButtons[3].setValue(true);
                values[3] = true;
            }else if (x < -0.25f)
            {
                pressButtons[2].setValue(true);
                values[2] = true;
            }
            else
            {
                pressButtons[2].setValue(false);
                pressButtons[3].setValue(false);
                values[2] = false;
                values[3] = false;
            }

            if (y > 0.25f)
            {
                pressButtons[0].setValue(true);
                values[0] = true;
            }
            else if (y < -0.25f)
            {
                pressButtons[1].setValue(true);
                values[1] = true;
            }
            else
            {
                pressButtons[0].setValue(false);
                pressButtons[1].setValue(false);
                values[0] = false;
                values[1] = false;
            }
        }
    }
}