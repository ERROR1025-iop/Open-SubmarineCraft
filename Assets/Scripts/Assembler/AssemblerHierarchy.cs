using UnityEngine;
using System.Collections;


namespace Scraft
{
    public class AssemblerHierarchy : MonoBehaviour
    {
        static public AssemblerHierarchy instance;

        public Transform cellParent;
        public float refreshInterval;

        Transform DPartsMap;

        static int MAX_Cell_COUNT = 50;
        AssemblerHierarchyCell[] cellsArray;

        RectTransform rectTransform;
        float rectTransformWith;

        RectTransform mainRectTrans;
        bool isShow;

        void Start()
        {
            instance = this;

            DPartsMap = GameObject.Find("3D DParts Map").transform;

            mainRectTrans = GetComponent<RectTransform>();

            rectTransform = cellParent.GetComponent<RectTransform>();
            rectTransformWith = rectTransform.sizeDelta.x;

            cellsArray = new AssemblerHierarchyCell[MAX_Cell_COUNT];

            for (int i=0;i< MAX_Cell_COUNT; i++)
            {
                cellsArray[i] = new AssemblerHierarchyCell(i, cellParent);
            }

            setShow(false);
        }

        private void Update()
        {
            if (!isShow)
            {
                return;
            }

            clearCellContent();
            int count = Mathf.Clamp(DPartsMap.childCount, 0, MAX_Cell_COUNT);
            GameObject gameObject;
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                gameObject = DPartsMap.GetChild(i).gameObject;
                if (gameObject.activeSelf)
                {
                    cellsArray[index].setContent(gameObject.gameObject);
                    index++;
                }
            }
            rectTransform.sizeDelta = new Vector2(rectTransformWith, count * 22.4f);
        }         

        void clearCellContent()
        {
            for (int i = 0; i < MAX_Cell_COUNT; i++)
            {
                cellsArray[i].clearContent();
            }
        }

        public void setShow(bool show)
        {
            isShow = show;
            if (show)
            {
                mainRectTrans.anchoredPosition = new Vector2(0, -34f);
            }
            else
            {
                mainRectTrans.anchoredPosition = new Vector2(1000, 0);
            }
        }
    }
}