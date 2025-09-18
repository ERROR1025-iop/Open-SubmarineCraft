using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class AttributeUsedColorCell
    {

        AttributeColor attributeColor;
        Color color;
        GameObject cellObject;
        Image image;

        public AttributeUsedColorCell(AttributeColor attributeColor)
        {
            this.attributeColor = attributeColor;

            cellObject = Object.Instantiate(Resources.Load("Prefabs/Assembler/used color cell")) as GameObject;
            cellObject.transform.SetParent(attributeColor.userdColorGridTrans);
            cellObject.GetComponent<Button>().onClick.AddListener(onCellClick);
            image = cellObject.GetComponent<Image>();
        }

        void onCellClick()
        {
            if (color != null)
            {
                attributeColor.onUserColorCellClick(color);
            }
        }

        public void setColor(Color color)
        {
            this.color = color;
            if (color == null)
            {
                cellObject.SetActive(false);
            }
            else if (cellObject != null)
            {
                cellObject.SetActive(true);
                image.color = color;
            }
        }

        public bool equals(Color c)
        {
            return color.Equals(c);
        }
    }
}