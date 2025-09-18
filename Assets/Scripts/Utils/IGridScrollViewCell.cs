using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class IGridScrollViewCell
    {

        protected IGridScrollViewInfo gridScrollViewInfo;       
        protected GameObject cellObject;
        protected RectTransform rectTransform;
        protected Image cellImage;
        protected int index;
        Color unclickColor;
        bool isClicking;

        public IGridScrollViewCell(IGridScrollViewInfo gridScrollViewInfo)
        {
            this.gridScrollViewInfo = gridScrollViewInfo;
            cellObject = Object.Instantiate(gridScrollViewInfo.prefab) as GameObject;
            rectTransform = cellObject.GetComponent<RectTransform>();
            cellImage = cellObject.GetComponent<Image>();
            cellObject.transform.SetParent(gridScrollViewInfo.parent);
            rectTransform.anchoredPosition = gridScrollViewInfo.position;
            index = gridScrollViewInfo.index;
            rectTransform.localScale = Vector3.one;
            unclickColor = cellImage.color;
            isClicking = false;

            cellObject.GetComponent<Button>().onClick.AddListener(onCellClick);
        }

        virtual public void setInformation<T>(T info)
        {
            isClicking = false;
            cellObject.SetActive(true);
        }

        virtual public void clearInformation()
        {
            cellObject.SetActive(false);
        }

        virtual public void onCellClick()
        {
            if (!isClicking)
            {
                cellImage.color = gridScrollViewInfo.clickColor;
                gridScrollViewInfo.gridScrollView.onCellClick(this);
                isClicking = true;
            }
        }

        virtual public void setUnClick()
        {
            cellImage.color = unclickColor;
            isClicking = false;
        }

        public int getIndex()
        {
            return index;
        }
    }
}
