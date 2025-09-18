using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class IGridScrollViewInfo
    {
        public IGridScrollView gridScrollView;
        public Vector2 position;
        public int index;
        public Transform parent;
        public GameObject prefab;   
        public Color clickColor;
    }
}

