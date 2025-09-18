using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class SelectorRS : RunScript
    {
        public static List<SelectorRS> selectorRSList;
        public static bool isInitGlobalRSList = false;


        public bool hasCustomButton1 = false;
        public string custom1ButtonName;
        public bool hasCustomButton2 = false;
        public string custom2ButtonName;

        public bool hasJoystick1 = false;
        public bool hasJoystick2 = false;
        public bool hasJoystick3 = false;

        List<SelectorRS> sameRSList;
        bool isInitSameRSList;
        bool m_isInitGlobalRSList = false;

        Dpart dpart;

        [HideInInspector]
        public bool isSelecting;
        [HideInInspector]
        public bool isMainSelecting;

        BoxCollider boxCollider;

        private void Awake()
        {
            if (World.GameMode != World.GameMode_Freedom)
            {
                Destroy(this);
                return;
            }

            if (Pooler.IS_MainSubmarine_initialized_Finish)
            {
                return;
            }

            initializedRSList();
            selectorRSList.Add(this);

            GameObject colliderGo = new GameObject("Collider Layer");
            colliderGo.transform.SetParent(transform);
            colliderGo.transform.localPosition = Vector3.zero;
            colliderGo.layer = 12;

            Bounds bounds = IUtils.GetBounds(gameObject);
            boxCollider = colliderGo.AddComponent<BoxCollider>();
            boxCollider.size = bounds.size;
            boxCollider.center = colliderGo.transform.InverseTransformPoint(bounds.center);
        }

        void Start()
        {
            
            sameRSList = new List<SelectorRS>();

            dpart = GetComponent<DpartChild>().getDpart();

            isSelecting = false;
            isMainSelecting = false;
            isInitSameRSList = false;
            isInitGlobalRSList = false;
        }

        public float getBoundSize()
        {
            return boxCollider.bounds.size.magnitude;
        }

        private void initializedRSList()
        {
            if (!isInitGlobalRSList)
            {
                if (selectorRSList == null)
                {
                    selectorRSList = new List<SelectorRS>();
                }
                else
                {
                    selectorRSList.Clear();
                }
                isInitGlobalRSList = true;                
            }
            m_isInitGlobalRSList = true;
        }

        public void onRaycastHit()
        {
            setMainSelecting(true);
            dpart.setOutline(true);
            PoolerItemSelector.instance.show(true);
        }

        public void setMainSelecting(bool s)
        {
            isMainSelecting = s;
            if (!PoolerItemSelector.isLocal)
            {
                foreach (SelectorRS rs in sameRSList)
                {
                    rs.setSelecting(s);
                }
            }
        }

        public void setSelecting(bool s)
        {
            isSelecting = s;
            dpart.setOutline(s);
        }

        public void cancel()
        {
            setMainSelecting(false);
        }

        public void onCustom1ButtonClick()
        {

        }

        public void onLocalButtonClick()
        {
            foreach (SelectorRS rs in sameRSList)
            {
                if (!rs.Equals(this))
                {
                    rs.isSelecting = !PoolerItemSelector.isLocal;
                }
            }
        }

        void LateUpdate()
        {
            initializedSameRSList();
        }

        private void initializedSameRSList()
        {
            if (m_isInitGlobalRSList && !isInitSameRSList)
            {
                foreach (SelectorRS rs in selectorRSList)
                {
                    if (rs.getDpart().equal(dpart))
                    {
                        sameRSList.Add(rs);
                    }
                }
                isInitSameRSList = true;
            }
        }

        public Dpart getDpart()
        {
            return dpart;
        }

    }
}