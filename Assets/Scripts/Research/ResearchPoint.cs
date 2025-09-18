using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;

namespace Scraft
{
    public class ResearchPoint
    {
        public RectTransform rectTransform;
        public string name;
        public bool isUnlock;
        public float unlockSci;
        public Block[] unlockBlocks;
        static Sprite[] cellSprites;
        static Sprite[] lineSprites;
        public Image cellImage;
        public Image iconImage;
        public ResearchPoint root;
        RectTransform lineRectTransform;
        Image lineImage;
        GameObject cellObject;
        int lineStartSprite;

        int col, row;
        int childCount;

        public ResearchPoint(string name, float unlockSci, Block[] unlockBlocks, ResearchPoint root, Transform cellParent)
        {
            this.name = name;
            this.root = root;
            this.unlockBlocks = unlockBlocks;
            this.unlockSci = unlockSci;
            cellSprites = Resources.LoadAll<Sprite>("Menu/Research/research-cell");
            cellObject = Object.Instantiate(Resources.Load("Prefabs/Research/research point cell")) as GameObject;
            cellObject.GetComponent<Button>().onClick.AddListener(onCellClick);
            cellImage = cellObject.GetComponent<Image>();
            rectTransform = cellObject.GetComponent<RectTransform>();
            rectTransform.SetParent(cellParent);
            iconImage = cellObject.transform.GetChild(0).GetComponent<Image>();
            iconImage.sprite = unlockBlocks[0].getSyntIconSprite();

            lineSprites = Resources.LoadAll<Sprite>("Menu/Research/research-line");
            lineRectTransform = (Object.Instantiate(Resources.Load("Prefabs/Research/research line")) as GameObject).GetComponent<RectTransform>();
            lineRectTransform.SetParent(cellParent);           
            lineImage = lineRectTransform.GetComponent<Image>();

            col = root == null ? 1 : root.getColumn() + 1;
            row = root == null ? 1 : root.getChildCount();       

            childCount = 0;

            if(root != null)
            {
                root.addChild(this);
            }
            else
            {
                Debug.Log(name + " Root is Null");
            }
        }

        public void initialized(bool isUnlock)
        {
            this.isUnlock = isUnlock || root == null;

            float y = 0;
            float x = col * 100;           
            int lineFlip = 1;
            lineStartSprite = 0;
            if (root != null)            
            {
                y = root.rectTransform.anchoredPosition.y + (root.getChildCount() - 1) * 64 - row * 128;
                switch(root.getChildCount())
                {
                    case 1:
                        lineStartSprite = 2;break;
                    case 2: lineStartSprite = 0; break;
                    case 3:
                        lineStartSprite = row == 1? lineStartSprite = 2 : lineStartSprite = 4; break;
                    default:
                        lineStartSprite = 2; break;
                }
                lineFlip = row > 0 ? -1 : 1;
            }
            else
            {
                lineRectTransform.gameObject.SetActive(false);
            }
            rectTransform.anchoredPosition = new Vector2(x, y);
            lineRectTransform.anchoredPosition = new Vector2(x - 16f, y + lineFlip * 6f);
            if(root != null && root.getChildCount() == 3)
            {
                lineRectTransform.localScale = new Vector3(0.5339f, lineFlip * 1f, 0.5339f);
            }
            else
            {
                lineRectTransform.localScale = new Vector3(0.5339f, lineFlip * 0.5339f, 0.5339f);
            }
           

            if (this.isUnlock)
            {
                cellImage.sprite = cellSprites[0];
                lineImage.sprite = lineSprites[lineStartSprite];
            }
            else
            {
                cellImage.sprite = cellSprites[1];
                lineImage.sprite = lineSprites[lineStartSprite + 1];
            }          
        }

        public bool canUnlock()
        {
            return !isUnlock && (root == null || root.isUnlock);
        }

        public float getUnlockConsume()
        {
            return unlockSci;
        }

        public void setUnlock()
        {
            isUnlock = true;
            cellImage.sprite = cellSprites[0];
            lineImage.sprite = lineSprites[lineStartSprite];
        }

        void onCellClick()
        {
            Research.instance.onResearchPointCellClick(this);
        }

        public int getColumn()
        {
            return col;
        }

        public int getChildCount()
        {
            return childCount;
        }

        public void addChild(ResearchPoint researchPoint)
        {
            childCount++;
        }

    }
}