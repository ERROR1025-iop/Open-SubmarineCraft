using Battlehub.RTHandles;
using Scraft.DpartSpace;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Scraft
{
    public class AssemblerHierarchyCell 
    {
        GameObject dpartGameObject;
        GameObject cellGameObject;
        RectTransform rectTransform;
        Transform parent;

        Text name;
        Image image;

        static Color selectedColor = new Color(0.25f, 0.25f, 0.25f);
        static Color unSelectedColor = new Color(0.16f, 0.16f, 0.16f); 

        bool isActivity;

        List<Object> selection;

        public AssemblerHierarchyCell(int index, Transform parent)
        {
            this.parent = parent;
            cellGameObject = Object.Instantiate(Resources.Load("Prefabs/Assembler/hierarchy cell")) as GameObject;
            cellGameObject.transform.SetParent(parent);
            rectTransform = cellGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -22.4f * index);
            rectTransform.localScale = Vector3.one;
            cellGameObject.GetComponent<Button>().onClick.AddListener(onCellClick);

            image = cellGameObject.GetComponent<Image>();
            name = cellGameObject.transform.GetChild(0).GetComponent<Text>();

            image.color = unSelectedColor;
            isActivity = true;
            clearContent();
        }

        public void setSelected(bool isSelecting)
        {
            image.color = isSelecting ? selectedColor : unSelectedColor;
        }

        public void setContent(GameObject gameObject)
        {
            isActivity = true;
            this.dpartGameObject = gameObject;
            name.text = ILang.get(dpartGameObject.name.Replace("(Clone)", ""), "dpart");
            setSelected(gameObject.GetComponent<DpartParent>().getDpart().isSelect());
        }

        public void clearContent()
        {
            if (isActivity)
            {
                isActivity = false;
                setSelected(false);
                name.text = "";               
            }      
        }

        void onCellClick()
        {
            if (isActivity)
            {
                if (RuntimeSelectionComponent.Multiselect && IRT.Selection.objects != null)
                {
                    selection = IRT.Selection.objects.ToList();
                    if (selection.Contains(dpartGameObject))
                    {
                        selection.Remove(dpartGameObject);                       
                    }
                    else
                    {
                        selection.Insert(0, dpartGameObject);
                    }
                    IRT.Selection.objects = selection.ToArray();
                }
                else
                {
                    IRT.Selection.activeObject = dpartGameObject;
                }                                
            }
        }
    }
}
