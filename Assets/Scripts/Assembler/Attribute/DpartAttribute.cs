using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlehub.RTCommon;
using Scraft.DpartSpace;

namespace Scraft
{
    public class DpartAttribute
    {

        public static DpartAttribute instance;
        Builder builder;
        GameObject dpartAttributeObject;
        RectTransform dpartAttributeTrans;
        Image smallArrowImage;
        Sprite[] smallArrowSprites;

        bool isShow;

        public DpartAttribute()
        {
            instance = this;
            dpartAttributeObject = GameObject.Find("dparts attribute");
            dpartAttributeTrans = dpartAttributeObject.GetComponent<RectTransform>();
            smallArrowImage = GameObject.Find("selector small arrow").GetComponent<Image>();
            smallArrowImage.transform.GetComponent<Button>().onClick.AddListener(onSmallArrowClick);
            smallArrowSprites = Resources.LoadAll<Sprite>("Assembler/small-arrow");
            IRT.Selection.SelectionChanged += RuntimeSelection_SelectionChanged;
        }

        void RuntimeSelection_SelectionChanged(Object[] unselectedObjects)
        {
            if (IRT.Selection.activeGameObject != null)
            {
                DpartChild dpartChild = IRT.Selection.activeGameObject.GetComponent<DpartChild>();
                if (dpartChild != null)
                {
                    instance.show(true);
                    Dpart dpart = dpartChild.getDpart();
                }
            }
            else
            {
                instance.show(false);
            }
        }

        public void onSmallArrowClick()
        {
            show(!isShow);
        }

        public void show(bool show)
        {
            isShow = show;
            if (show)
            {
                dpartAttributeTrans.anchoredPosition = new Vector2(-74.9f, 0);
                smallArrowImage.sprite = smallArrowSprites[1];
            }
            else
            {
                dpartAttributeTrans.anchoredPosition = new Vector2(9999f, 0);
                smallArrowImage.sprite = smallArrowSprites[0];
            }
        }
    }
}