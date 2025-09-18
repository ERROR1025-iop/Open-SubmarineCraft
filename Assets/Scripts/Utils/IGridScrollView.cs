using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class IGridScrollView : MonoBehaviour
    {
        public int maxCellCount;
        public float cellWith, cellHeight;
        public int colCount;
        public GameObject cellPrefab;
        public Color clickColor;

        IGridScrollViewCell[] cellsArray;
        RectTransform rectTransform;
        float rectTransformWith;

        [HideInInspector]
        public IGridScrollViewCell activiteCell;

        public delegate IGridScrollViewCell CreateCellsDelegate(IGridScrollViewInfo gridScrollViewInfo);
        public event CreateCellsDelegate OnCreateCells;

        public delegate void CellClickDelegate(IGridScrollViewCell cell);
        public event CellClickDelegate OnCellClick;

        void Start()
        {

        }

        public void initialized(CreateCellsDelegate OnCreateCells)
        {
            this.OnCreateCells = OnCreateCells;
            rectTransform = GetComponent<RectTransform>();
            rectTransformWith =  rectTransform.sizeDelta.x;
            cellsArray = new IGridScrollViewCell[maxCellCount];
            for (int i = 0; i < maxCellCount; i++)
            {
                float x = i % colCount * cellWith;
                float y = -(i / colCount) * cellHeight;

                IGridScrollViewInfo gridScrollViewInfo = new IGridScrollViewInfo();
                gridScrollViewInfo.gridScrollView = this;               
                gridScrollViewInfo.position = new Vector2(x, y);
                gridScrollViewInfo.index = i;
                gridScrollViewInfo.parent = rectTransform;
                gridScrollViewInfo.prefab = cellPrefab;
                gridScrollViewInfo.clickColor = clickColor;
                cellsArray[i] = OnCreateCells(gridScrollViewInfo);
            }
        }

        public void setInformation<T>(List<T> infos)
        {
            int count = infos.Count;
            int row = count / colCount + 1;
            rectTransform.sizeDelta =  new Vector2(rectTransformWith, row * cellHeight);
            for (int i = 0; i < maxCellCount; i++)
            {
                if (i < count)
                {
                    cellsArray[i].setInformation(infos[i]);
                }
                else
                {
                    cellsArray[i].clearInformation();
                }
            }
        }

        public void onCellClick(IGridScrollViewCell cell)
        {
            if (activiteCell != null)
            {
                activiteCell.setUnClick();
            }
            activiteCell = cell;

            if(OnCellClick != null)
            {
                OnCellClick(cell);
            }
        }

        public void setCellActivity(int index)
        {
            IGridScrollViewCell cell = cellsArray[index];
            if(cell != null)
            {
                cell.onCellClick();
            }
        }
    }
}
