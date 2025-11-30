using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.BlockSpace;
using LitJson;
using UnityEngine.Events;

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
        private bool isInside = false;

        [Header("Callbacks")]
        [Tooltip("当主角进入区域时触发（可在Inspector中绑定方法）")]
        public UnityEvent<Area> onEnter;    

        [Tooltip("当主角离开区域时触发（可在Inspector中绑定方法）")]
        public UnityEvent<Area> onExit;
        void Start()
        {
            stayLayered = 0;
            last_stayLayered = 0;                             
            AreaManager.instance.RegisterArea(this);

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

        private void Update()
        {
            // 运行时持续检测主角是否在区域中
            if (MainSubmarine.instance == null || MainSubmarine.transform == null) return;

            bool nowInside = IsPointInArea(MainSubmarine.transform.position);

            if (nowInside && !isInside)
            {
                // 发生进入
                isInside = true;
                onEnter?.Invoke(this);
                OnEnterArea();
            }
            else if (!nowInside && isInside)
            {
                // 发生离开
                isInside = false;
                onExit?.Invoke(this);
                OnExitArea();
            }
        }

        public void OnEnterArea()
        {
            Debug.Log(string.Format("[Area]Enter Area <{0}>_{1}", name, stayLayered));
            ScientificSelector.instance.OnAreaEnter(this);
        }

        public void OnExitArea()
        {
            Debug.Log(string.Format("[Area]Exit Area <{0}>_{1}", name, stayLayered));
            ScientificSelector.instance.OnAreaLeave(this);
        }

        public void onLayeredChanged()
        {
            ScientificSelector.instance.UpdateAreaValue();
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
            stayLayered = deep < 5 ? 0 : Mathf.Clamp((int)(deep / 700) + 1, 0, 13);
            layeredScientific = (scientific + 1) * (stayLayered + 1) * (stayLayered + 1) * 0.5f;

            if(stayLayered != last_stayLayered)
            {
                onLayeredChanged();
            }
            last_stayLayered = stayLayered;
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
