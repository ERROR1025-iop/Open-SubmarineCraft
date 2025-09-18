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

        static public int smallAreaId;

        static public JsonData areasDatas;
        static public JsonData smallAreaDatas;

        static public float stayTemperture;

        void Awake()
        {
            instance = this;

            areas = new List<Area>();
            smallAreas = new List<SmallArea>();

            smallAreaId = 0;

            stayTemperture = 25;

            StartCoroutine(onUpdate());
        }

        IEnumerator onUpdate()
        {
            while (true)
            {
                bool isInSmallArea = false;
                foreach (SmallArea area in smallAreas)
                {
                    if (Vector3.Distance(MainSubmarine.transform.position, area.transform.position) < area.radius)
                    {
                        isInSmallArea = true;
                        staySmallArea = area;
                        break;
                    }                    
                }

                if (!isInSmallArea)
                {
                    staySmallArea = null;
                }


                foreach (Area area in areas)
                {                  

                    if (Vector3.Distance(MainSubmarine.transform.position, area.transform.position) < area.radius)
                    {
                        if (stayArea != null)
                        {
                            if (!stayArea.Equals(area))
                            {
                                stayArea.onExit();
                                area.onEnter();
                                stayArea = area;
                            }                            
                        }
                        else
                        {
                            stayArea = area;
                            area.onEnter();                           
                        }
                        break;
                    }                    
                }
                            

                if (staySmallArea != null)
                {
                    stayTemperture = staySmallArea.getTemperture();
                }
                else if (stayArea != null)
                {
                    stayTemperture = stayArea.getTemperture();
                    stayArea.UpdateLayered();
                }

                yield return new WaitForSeconds(5f);
            }
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
