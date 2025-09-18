using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;
using LitJson;

namespace Scraft
{
    public class ScientificSelector 
    {
        static public ScientificSelector intance;

        Transform objectTrans;
        PoolerInput poolerInput;      
        
        Text nameText;
        Text layeredText;
        Text valueText;

        Area stayArea;
        SolidBlock selectBlock;

        public ScientificSelector(PoolerInput poolerInput)
        {
            intance = this;
            this.poolerInput = poolerInput;
            objectTrans = GameObject.Find("Canvas/Scientific selector").transform;
            objectTrans.GetChild(0).GetComponent<Button>().onClick.AddListener(onCollectButtonClick);
            objectTrans.GetChild(1).GetComponent<Button>().onClick.AddListener(onCancelButtonClick);
            nameText = objectTrans.GetChild(2).GetComponent<Text>();
            layeredText = objectTrans.GetChild(3).GetComponent<Text>();
            valueText = objectTrans.GetChild(4).GetComponent<Text>();           

            show(false, null);
        }

        void onCollectButtonClick()
        {
            if(stayArea == null)
            {
                return;
            }

            if (stayArea.getIsCollectedScientific(selectBlock.getCollectedScientificId()))
            {
                IToast.instance.show("No scientific points can be collected in this area.", 50);
                return;
            }

            if(selectBlock.getCollectedScientific() > 0)
            {
                IConfigBox.instance.show(ILang.get("Does it cover the scientific points that have been collected?"), onCollectComfirmClick, null);
            }
            else
            {
                onCollectComfirmClick();
            }
          
        }

        void onCollectComfirmClick()
        {
            selectBlock.onCollectScientific(new CollectedScientificInfo(stayArea));
            updateBlockValue();
            stayArea.addCollectedScientificLayered(selectBlock.getCollectedScientificId());
        }

        void onCancelButtonClick()
        {
            show(false, null);
        }

        public void updateAreaValue(Area stayArea)
        {
            this.stayArea = stayArea;
            nameText.text = stayArea.getLangName();
            layeredText.text = stayArea.getLayeredName();            
        }

        public void updateBlockValue()
        {
            if(selectBlock != null)
            {
                valueText.text = selectBlock.getCollectedScientific().ToString("f1");
            }            
        }

        public void show(bool isShow, SolidBlock selectBlock)
        {
            objectTrans.gameObject.SetActive(isShow);
            if (isShow)
            {
                this.selectBlock = selectBlock;
                updateBlockValue();
            }
        }
    }

    public class CollectedScientificInfo
    {
        public Area area;
        public int layered;

        public CollectedScientificInfo(Area area)
        {
            this.area = area;
            layered = area.stayLayered;
        }

        public CollectedScientificInfo(Area area, int layered)
        {
            this.area = area;
            this.layered = layered;
        }

        public void onSave(JsonWriter writer)
        {
            IUtils.keyValue2Writer(writer, "a", area.name);
            IUtils.keyValue2Writer(writer, "l", layered);
        }

        static public CollectedScientificInfo onLoad(JsonData jsonData)
        {
            string name = IUtils.getJsonValue2String(jsonData, "a");
            int layered = IUtils.getJsonValue2Int(jsonData, "l");
            GameObject gameObject = GameObject.Find("Aresa/" + name);
            if(gameObject != null)
            {
                Area area = gameObject.GetComponent<Area>();
                return new CollectedScientificInfo(area, layered);
            }
            return null;
        }
    }
}