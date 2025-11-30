using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft
{
    public class AreaManager : MonoBehaviour
    {
        static public AreaManager instance;

        static public Area stayArea;
        static public SmallArea staySmallArea;

        static public List<Area> areas;
        static public List<SmallArea> smallAreas;
        static public List<SciArea> sciAreas;

        static public int smallAreaId;

        static public JsonData areasDatas;
        static public JsonData smallAreaDatas;
        static public JsonData sciAreaDatas;

        void Awake()
        {
            instance = this;
            areas = new List<Area>();
            smallAreas = new List<SmallArea>();
            sciAreas = new List<SciArea>();
            smallAreaId = 0;
        }

        public void RegisterArea(Area area)
        {
            areas.Add(area);
            area.onEnter.AddListener(OnEnterArea);
            area.onExit.AddListener(OnExitArea);
        }

        public void RegisterSciArea(SciArea area)
        {
            sciAreas.Add(area);
        }

        public void OnEnterArea(Area area)
        {
            staySmallArea = area;
            stayArea = area;
            stayArea.UpdateLayered();
        }

        public void DecCollectedScientificLayeredByName(string name, int layered, int csId)
        {
            foreach (SciArea area in sciAreas)
            {
                if (area.GetName() == name)
                {
                    area.decCollectedScientificLayered(csId, layered);
                    return;
                }
            }
            foreach (Area area in areas)
            {
                if (area.name == name)
                {
                    area.decCollectedScientificLayered(layered, csId);
                    return;
                }
            }
        }

        void Update()
        {
            if (stayArea != null)
            {
                stayArea.UpdateLayered();
            }
        }

        public void OnExitArea(Area area)
        {
            staySmallArea = null;
            stayArea = null;
        }

        static public string saveAreas()
        {
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();

            IUtils.keyValue2Writer(writer, "count", areas.Count);
            writer.WritePropertyName("areas");
            writer.WriteObjectStart();

            foreach (Area area in areas)
            {
                writer.WritePropertyName(area.name);
                writer.WriteObjectStart();
                area.onSave(writer);
                writer.WriteObjectEnd();
            }
            writer.WriteObjectEnd();
            writer.WriteObjectEnd();
            return writer.ToString();
        }

        static public string saveSciAreas()
        {
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();

            IUtils.keyValue2Writer(writer, "count", sciAreas.Count);
            writer.WritePropertyName("areas");
            writer.WriteObjectStart();

            foreach (SciArea area in sciAreas)
            {
                if (area.collected)
                {
                    writer.WritePropertyName(area.GetName());
                    writer.WriteObjectStart();
                    area.onSave(writer);
                    writer.WriteObjectEnd();
                }                
            }
            writer.WriteObjectEnd();
            writer.WriteObjectEnd();
            return writer.ToString();
        }


        static public string saveSmallAreas()
        {
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();

            int count = smallAreas.Count;
            IUtils.keyValue2Writer(writer, "count", smallAreas.Count);
            writer.WritePropertyName("areas");
            writer.WriteObjectStart();

            foreach (SmallArea area in smallAreas)
            {
                writer.WritePropertyName(area.name);
                writer.WriteObjectStart();
                area.onSave(writer);
                writer.WriteObjectEnd();
            }
            writer.WriteObjectEnd();
            writer.WriteObjectEnd();
            return writer.ToString();
        }

    }
}
