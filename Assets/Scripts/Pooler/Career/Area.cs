using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.BlockSpace;
using LitJson;

namespace Scraft
{
    public class Area : SmallArea
    {

        [Header("Area")]
        public float scientific;

        [Header("Read only")]
        public int stayLayered;    
        public float layeredScientific;      
        public int[] collectedScientificLayered;

        int last_stayLayered;

        void Start()
        {
            stayLayered = 0;
            last_stayLayered = 0;                             
            AreaManager.areas.Add(this);

            onStart();

            collectedScientificLayered = new int[BlocksManager.instance.getCanCollectedScientificBlockCount()];
            IUtils.initializedArray(collectedScientificLayered, 0);

            if (AreaManager.areasDatas != null)
            {
                JsonData areaDatas = AreaManager.areasDatas[name];
                onLoad(areaDatas);
            }
            updateOreRatio();
            updateEnrichment();
        } 

        public void onEnter()
        {
            Debug.Log(string.Format("[Area]Enter Area <{0}>_{1}", name, stayLayered));
            ScientificSelector.intance.updateAreaValue(this);
        }

        public void onExit()
        {

        }

        public void onLayeredChanged()
        {
            ScientificSelector.intance.updateAreaValue(this);
        }

        public bool getIsCollectedScientific(int layered, int csId)
        {
            return ((collectedScientificLayered[csId] & (0x1 << layered)) >> layered) == 1;
        }

        public bool getIsCollectedScientific(int csId)
        {
            return getIsCollectedScientific(stayLayered, csId);
        }

        public void addCollectedScientificLayered(int layered, int csId)
        {
            collectedScientificLayered[csId] |= (1 << layered); 
        }

        public void addCollectedScientificLayered(int csId)
        {
            addCollectedScientificLayered(stayLayered, csId);
        }

        public void decCollectedScientificLayered(int layered, int csId)
        {
            collectedScientificLayered[csId] &= ~(1 << layered); 
        }

        public void decCollectedScientificLayered(int csId)
        {
            decCollectedScientificLayered(stayLayered, csId);
        }

        public string getLangName()
        {
            return ILang.get(name, "area");
        }

        public string getLayeredName()
        {
            return ILang.get("layered_" + stayLayered);
        }

        public void UpdateLayered()
        {
            float deep = MainSubmarine.deep;
            stayLayered = deep < 5 ? 0 : Mathf.Clamp((int)(deep / 1000) + 1, 0, 7);
            layeredScientific = (scientific + 1) * (stayLayered + 1) * (stayLayered + 1) * 0.5f;

            if(stayLayered != last_stayLayered)
            {
                onLayeredChanged();
            }
            last_stayLayered = stayLayered;
        }

        public override float getTemperture()
        {
            return Mathf.Clamp(temperture - MainSubmarine.deep * 0.1f, 1f, 1000);
        }

        public override void onSave(JsonWriter writer)
        {
            base.onSave(writer);
            int count = collectedScientificLayered.Length;           
            IUtils.keyValue2Writer(writer, "csData", IUtils.serializeIntArray(collectedScientificLayered));

        }

        public override void onLoad(JsonData jsonData)
        {
            base.onLoad(jsonData);   
            int[] csData = IUtils.unserializeIntArray(IUtils.getJsonValue2String(jsonData, "csData"));
            int count = Mathf.Min(collectedScientificLayered.Length, csData.Length);
            for (int i=0;i< count; i++)
            {
                collectedScientificLayered[i] = csData[i];
            }
        }
    }
}
