using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class AttributeMaterialCell
    {

        AttributeColor attributeColor;
        DpartMaterial material;
        GameObject cellObject;
        Text nameText;

        public AttributeMaterialCell(AttributeColor attributeColor, DpartMaterial material)
        {
            this.attributeColor = attributeColor;
            this.material = material;

            cellObject = Object.Instantiate(Resources.Load("Prefabs/Assembler/matrial cell")) as GameObject;
            cellObject.transform.SetParent(attributeColor.materialsGridTrans, false);
            cellObject.GetComponent<Button>().onClick.AddListener(onCellClick);
            cellObject.transform.GetChild(0).GetComponent<Text>().text = ILang.get("material." + material.getName());
        }


        void onCellClick()
        {
            attributeColor.onMaterialCellClick(material.getMaterialId(), this);
        }

        public DpartMaterial getDpartMaterial()
        {
            return material;
        }

        public Material getMaterial()
        {
            return material.getMaterial();
        }

        public string getName()
        {
            return material.getName();
        }
    }
}
