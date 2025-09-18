using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class IFps : MonoBehaviour
    {
        public float fpsMeasuringDelta = 0.5f;

        private float timePassed;
        private int m_FrameCount = 0;
        private float m_FPS = 0.0f;

        Text text;

        private void Start()
        {
            timePassed = 0.0f;
            text = GetComponent<Text>();
        }

        private void Update()
        {
            m_FrameCount = m_FrameCount + 1;
            timePassed = timePassed + Time.deltaTime;

            if (timePassed > fpsMeasuringDelta)
            {
                m_FPS = m_FrameCount / timePassed;
                text.text = "FPS:" + m_FPS;
                timePassed = 0.0f;
                m_FrameCount = 0;
            }
        }
    }
}
