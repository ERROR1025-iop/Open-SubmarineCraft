using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class ElectricMeter
    {

        RectTransform electricBar;
        Text electricText;
        Text electricSpeedText;
        float barWith;

        float max_electric;
        float electric;

        float last_electric;
        float speed_electric;
        float last_speed_electric;
        int last_stack;
        int last_e_history_stack;

        public ElectricMeter()
        {
            electricBar = GameObject.Find("Canvas/electric bar").GetComponent<RectTransform>();
            electricText = GameObject.Find("Canvas/electric meter/text").GetComponent<Text>();
            electricSpeedText = GameObject.Find("Canvas/electric meter/text2").GetComponent<Text>();
            barWith = electricBar.sizeDelta.x;

            max_electric = 0;
            electric = 0;
            last_electric = 0;
            last_stack = 0;
            last_e_history_stack = 0;
            last_speed_electric = 0;
        }

        public void setMaxElectirc(float m)
        {
            max_electric = m;
            updateElectric();
        }

        public void setElectric(float e)
        {
            electric = e;
            updateElectric();
        }

        public float getElectric()
        {
            return electric;
        }

        public float getMaxElectric()
        {
            return max_electric;
        }

        void updateElectric()
        {
            float r;
            if (max_electric == 0)
            {
                electric = 0;
                r = 0;
            }
            else if (electric >= max_electric)
            {
                electric = max_electric;
                r = barWith;
            }
            else
            {
                r = barWith * (electric / max_electric);
            }

            electricText.text = electric.ToString("f0") + "/" + max_electric.ToString("f0");
            electricBar.sizeDelta = new Vector2(r, electricBar.sizeDelta.y);


        }

        public void updataPower()
        {
            if (last_stack < 128)
            {
                last_stack++;
                return;
            }
            if (last_e_history_stack < 1)
            {
                last_e_history_stack++;
                return;
            }
            speed_electric = (electric - last_electric) * 0.0434f;
            last_speed_electric = speed_electric;
            last_electric = electric;
            last_stack = 0;

            if (speed_electric > 1000)
            {
                return;
            }

            electricSpeedText.text = speed_electric.ToString("f1") + "u/t";
        }
    }
}