using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class AirMeter
    {
        IProgressbar progressbar;

        float max_air;
        float air;

        public AirMeter()
        {
            progressbar = GameObject.Find("Canvas/air meter").GetComponent<IProgressbar>();
        }

        public void setMaxAir(float m)
        {
            max_air = m;
            updateAir();
        }

        public void setAir(float e)
        {
            air = e;
            updateAir();
        }

        public float getAir()
        {
            return air;
        }

        public float getMaxAir()
        {
            return max_air;
        }

        void updateAir()
        {
            progressbar.setValue(air, max_air);
        }
    }
}
