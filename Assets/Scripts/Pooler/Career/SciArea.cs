using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.BlockSpace;
using LitJson;
using UnityEngine.Events;
using System;

namespace Scraft
{
    public class SciArea : AreaDetectorActive
    {       
        public float baseSicPoint = 10f;
        public bool randomNameSuffix = false;
        public string suffix = "";
        public int[] collectedScientificLayered;
        public bool collected = false;

        IEnumerator Start()
        {
            suffix = "_" + System.Guid.NewGuid().ToString("N").Substring(0, 6);
            collectedScientificLayered = new int[BlocksManager.instance.getCanCollectedScientificBlockCount()];
            IUtils.initializedArray(collectedScientificLayered, 0);
            AreaManager.instance.RegisterSciArea(this);

            // wait one frame so other managers can finish initialization
            yield return null;

            if (AreaManager.sciAreaDatas != null)
            {
                try
                {
                    JsonData areaDatas = AreaManager.sciAreaDatas[GetName()];
                    onLoad(areaDatas);
                }
                catch
                {
                    //Debug.LogError("SciArea Load Error: " + name + " " + e.ToString());
                }
            }
        }

        protected override void OnEnterArea()
        {
            ScientificSelector.instance.OnSciAreaEnter(this);
        }

        protected override void OnExitArea()
        {
            ScientificSelector.instance.OnSciAreaLeave(this);
        }

        public string GetName()
        {
            return name + (randomNameSuffix ? suffix : "");
        }

        public string GetLangName()
        {
            return ILang.get(name, "area") + (randomNameSuffix ? suffix : "");
        }

        public void addCollectedScientificLayered(int csId, int layered=0)
        {
            collectedScientificLayered[csId] |= (1 << layered); 
            collected = true;
        }

        public void decCollectedScientificLayered(int csId, int layered=0)
        {
            collectedScientificLayered[csId] &= ~(1 << layered); 
        }

        public bool getIsCollectedScientific(int csId)
        {
            int layered = 0;
            return ((collectedScientificLayered[csId] & (0x1 << layered)) >> layered) == 1;
        }

        public void onSave(JsonWriter writer)
        {            
            int count = collectedScientificLayered.Length;           
            IUtils.keyValue2Writer(writer, "csData", IUtils.serializeIntArray(collectedScientificLayered));
        }

        public void onLoad(JsonData jsonData)
        {
            int[] csData = IUtils.unserializeIntArray(IUtils.getJsonValue2String(jsonData, "csData"));
            int count = Mathf.Min(collectedScientificLayered.Length, csData.Length);
            for (int i=0;i< count; i++)
            {
                collectedScientificLayered[i] = csData[i];
            }
            collected = true;
        }
    }
}