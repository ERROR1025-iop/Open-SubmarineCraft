using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;
using LitJson;

namespace Scraft
{
    public class ScientificSelector : MonoBehaviour
    {
        static public ScientificSelector instance;

        Transform objectTrans;
        PoolerInput poolerInput;      
        
        Text nameText;
        Text layeredText1;
        Text valueText;

        Area stayArea;
        SolidBlock selectBlock;

        SciArea staySciArea;

        void Awake()
        {
            instance = this;
            objectTrans = gameObject.transform;
            objectTrans.GetChild(0).GetComponent<Button>().onClick.AddListener(onCollectButtonClick);
            objectTrans.GetChild(1).GetComponent<Button>().onClick.AddListener(onCancelButtonClick);
            nameText = objectTrans.GetChild(2).GetComponent<Text>();
            layeredText1 = objectTrans.GetChild(3).GetComponent<Text>();
            valueText = objectTrans.GetChild(4).GetComponent<Text>();           

            show(false, null);
        }

        void onCollectButtonClick()
        {
            var bcsid = selectBlock.getCollectedScientificId();

            if(staySciArea != null)
            {
                if (staySciArea.getIsCollectedScientific(bcsid))
                {
                    IToast.instance.show("No scientific points can be collected in this area.", 50);
                    return;
                }
            }

            if(stayArea != null)
            {
                if (stayArea.getIsCollectedScientific(bcsid))
                {
                    IToast.instance.show("No scientific points can be collected in this area.", 50);
                    return;
                }
            }

            if (staySciArea == null && stayArea == null)
            {
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

        public void OnSciAreaEnter(SciArea sciArea)
        {
            staySciArea = sciArea;
            AreaNameView.instance.SetSciString(sciArea.GetLangName());
            UpdateAreaValue();
        }

        public void OnSciAreaLeave(SciArea sciArea)
        {
            staySciArea = null;
            AreaNameView.instance.SetSciString(null);
            UpdateAreaValue();
        }

        public void OnAreaEnter(Area area)
        {
            stayArea = area;
            UpdateAreaValue();
        }

        public void OnAreaLeave(Area area)
        {
            stayArea = null;
            UpdateAreaValue();
        }

        void onCollectComfirmClick()
        {
            CollectedScientificInfo csi = null;
            int bcsid = selectBlock.getCollectedScientificId();
            if (staySciArea != null)
            {
                csi = new CollectedScientificInfo(staySciArea.name, staySciArea.baseSicPoint, 0);
                staySciArea.addCollectedScientificLayered(bcsid);

            }else if(stayArea != null)
            {
                csi = new CollectedScientificInfo(stayArea.name, stayArea.layeredScientific, stayArea.stayLayered);                
                stayArea.addCollectedScientificLayered(bcsid);                
            }

            if (csi != null)
            {
                selectBlock.onCollectScientific(csi);
                updateBlockValue();
            }
        }
        

        void onCancelButtonClick()
        {
            show(false, null);
        }

        public void UpdateAreaValue()
        {            
            if (staySciArea != null)
            {
                nameText.text = staySciArea.GetLangName();
                layeredText1.text = "";
            }
            else
            {
                if (stayArea == null)
                {
                    nameText.text = "";
                    layeredText1.text = "";
                    AreaNameView.instance.SetAreaString("");
                    return;
                }
                nameText.text = stayArea.getLangName();
                layeredText1.text = stayArea.getLayeredName();
                AreaNameView.instance.SetAreaString(stayArea.getLangName() + " " + stayArea.getLayeredName());
            }
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
        public string name;
        public int layered;
        public float point;

        public CollectedScientificInfo(string name, float point, int layered=0)
        {
            this.name = name;
            this.point = point;
            this.layered = layered;
        }

        public void onSave(JsonWriter writer)
        {
            IUtils.keyValue2Writer(writer, "a", name);
            IUtils.keyValue2Writer(writer, "l", layered);
        }

        static public CollectedScientificInfo onLoad(JsonData jsonData)
        {
            string name = IUtils.getJsonValue2String(jsonData, "a");
            int layered = IUtils.getJsonValue2Int(jsonData, "l");
            return new CollectedScientificInfo(name, layered);
        }
    }
}