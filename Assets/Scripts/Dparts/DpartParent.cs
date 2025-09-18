using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.DpartSpace
{
    public class DpartParent : MonoBehaviour
    {
        public bool isMirrorCanNegativeScale = true;
              
        DpartChild[] childrens;
        Dpart dpart;        

        void Start()
        {
            if (World.GameMode == World.GameMode_Assembler && IRT.Selection != null)
            {
                IRT.Selection.SelectionChanged += OnSelectionChanged;
            }
        }        

        public void init(Dpart dpart)
        {
            this.dpart = dpart;

            if (childrens == null)
            {
                childrens = GetComponentsInChildren<DpartChild>();
            }
        }

        public Dpart getDpart()
        {
            return dpart;
        }

        public int getChildrensCount()
        {
            return childrens.Length;
        }

        public DpartChild getChild(int index)
        {
            if (index < childrens.Length)
            {
                return childrens[index];
            }
            return null;
        }

        public void changeDpartChindrens(System.Action<DpartChild> execute)
        {
            DpartChild child;
            for (int i = 0; i < childrens.Length; i++)
            {
                child = childrens[i];
                if (child != null)
                {
                    execute(child);
                }
            }
        }

        public void OnSelectionChanged(Object[] unselectedObjects)
        {
            if (this != null && IRT.Selection.activeGameObject != null && dpart != null)
            {
                GameObject[] go = IRT.Selection.gameObjects;
                int count = go.Length;
                bool isSelecting = false;
                for (int i = 0; i < count; i++)
                {
                    if (go[i].Equals(gameObject))
                    {
                        isSelecting = true;
                        break;
                    }
                }
                dpart.isSelecting = isSelecting;
            }
        }

        void Update()
        {
            if (World.GameMode == World.GameMode_Assembler)
            {
                if (dpart != null)
                {
                    if (dpart.isSelecting)
                    {
                        dpart.onBuilderModeSelecting();
                    }
                    else
                    {
                        dpart.onBuilderModeUnSelecting();
                    }
                }

            }
        }

        private void LateUpdate()
        {
            if (World.GameMode == World.GameMode_Assembler)
            {
                if (dpart != null)
                {
                    dpart.LateUpdate();
                }
            }
        }
    }
}

