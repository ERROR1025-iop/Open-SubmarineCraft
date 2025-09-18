using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;

namespace Scraft
{
    public class ResearchDrawer
    {    
        GameObject drawerObject;
        RectTransform rectTransform;
        Transform parent;   
        Image iconImage;
        Text drawerName;
        Block block;
        int index;       

        public ResearchDrawer(int index, Transform parent)
        {      
            this.index = index;
            this.parent = parent;       

            drawerObject = Object.Instantiate(Resources.Load("Prefabs/Research/research drawer")) as GameObject;
            rectTransform = drawerObject.GetComponent<RectTransform>();
            drawerObject.transform.SetParent(parent);
            rectTransform.anchoredPosition = new Vector2(0, -31f * index);
            rectTransform.localScale = Vector3.one;

            drawerObject.GetComponent<Button>().onClick.AddListener(onDrawerClick);

            iconImage = drawerObject.transform.GetChild(0).GetComponent<Image>();
            drawerName = drawerObject.transform.GetChild(1).GetComponent<Text>();

            clearInformation();

            //GameObject drawerPriceObject = drawerObject.transform.GetChild(2).gameObject;
            //Text drawerPrice = drawerPriceObject.GetComponent<Text>();
            //drawerPrice.text = "$" + block.getTotalPrice().ToString();     
        }

        public void setInformation(Block block)
        {
            this.block = block;
            iconImage.color = Color.white;
            iconImage.sprite = block.getSyntIconSprite();
            drawerName.text = block.getLangName();
        }

        public void clearInformation()
        {
            iconImage.color = Color.clear;
            drawerName.text = "";
        }

        void onDrawerClick()
        {
            Research.instance.onResearchInformationPointCellClick(block);
        }    

        public Block getBlock()
        {
            return block;
        }
    }
}
