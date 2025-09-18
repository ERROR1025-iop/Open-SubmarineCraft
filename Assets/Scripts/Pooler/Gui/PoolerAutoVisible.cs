using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class PoolerAutoVisible : MonoBehaviour
    {
        public bool VisibleIn2D = true;
        public bool VisibleIn3D = true;
        public bool ManualUnvisible = false;

        bool m_visible = true;
        public bool visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                m_visible = value;
                updateVisible();
            }
        }

        RectTransform rectTransform;
        Vector2 position;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            position = rectTransform.anchoredPosition;
        }


        void Update()
        {
            if (ManualUnvisible)
            {
                visible = false;
                return;
            }
            visible = (World.activeCamera >= 2 && VisibleIn3D) || (World.activeCamera < 2 && VisibleIn2D);
        }

        void updateVisible()
        {
            if (visible)
            {
                rectTransform.anchoredPosition = position;
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(99999, 0);
            }
        }
    }
}