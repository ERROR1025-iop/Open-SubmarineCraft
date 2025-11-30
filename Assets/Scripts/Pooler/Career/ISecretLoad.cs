using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Scraft.StationSpace;

namespace Scraft
{
    public class ISecretLoad
    {  
        static bool isInit = false;
        static bool isEncrypt;

        static public string researchNamePath;
        static public string sciAndDiaNamePath;
        static public string stationInfoNamePath;
        static public string mainStationNamePath;
        static public string mainShipNamePath;
        static public string mainAssNamePath;
        static public string shipsInfoNamePath;
        static public string areasNamePath;
        static public string smallAreasNamePath;
        static public string sciAreasNamePath;

        static public string DataPath;
        static public string Md5Path;
        static public string Suffix;

        static float m_scientific;
        static float m_diamonds;

        static public int mainStationId;
        static public JsonData stationsData;

        static GameObject stationPrefab;
        static GameObject shipPrefab;

        static public Vector3[] spwans;

        static public void init()
        {
            isEncrypt = true;

            if (!isInit)
            {                
                researchNamePath = "research_name";
                sciAndDiaNamePath = "sci_name";
                stationInfoNamePath = "station_info";
                mainStationNamePath = "station_name";
                shipsInfoNamePath = "ships_info";
                areasNamePath = "areas_name";
                smallAreasNamePath = "small_areas_name";
                sciAreasNamePath = "sci_areas_name";
                

                GamePath.init();
                DataPath = GamePath.worldFolder;
                Md5Path = GamePath.worldFolder;
                Suffix = ".json";

                loadScientificAndDiamonds();

                spwans = new Vector3[2];
                IUtils.initializedArray(spwans, Vector3.zero);

                isInit = true;
            }
        }

        static public void saveWorldAreaInfo()
        {                                 
            saveWithCreateMd5(areasNamePath, AreaManager.saveAreas());
            saveWithCreateMd5(smallAreasNamePath, AreaManager.saveSmallAreas());
            saveWithCreateMd5(sciAreasNamePath, AreaManager.saveSciAreas());
        }

        static public void loadWorldAreaInfo()
        {
            string areasData = readWithVerifyMd5(areasNamePath);
            if (areasData != null)
            {
                JsonData jsonData = JsonMapper.ToObject(areasData);
                AreaManager.areasDatas = jsonData["areas"];
            }

            string smallAreasData = readWithVerifyMd5(smallAreasNamePath);
            if (smallAreasData != null)
            {
                JsonData jsonData = JsonMapper.ToObject(smallAreasData);
                AreaManager.smallAreaDatas = jsonData["areas"];
            }

            string sciAreasData = readWithVerifyMd5(sciAreasNamePath);
            if (sciAreasData != null)
            {
                JsonData jsonData = JsonMapper.ToObject(sciAreasData);
                AreaManager.sciAreaDatas = jsonData["areas"];
            }
        }

        static public void saveWorldShipsInfo(bool isSaveMainShip)
        {
            
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();

            int count = ShipPreload.shipPreloads.Count;
            IUtils.keyValue2Writer(writer, "count", isSaveMainShip ? count + 1 : count);

            writer.WritePropertyName("ships");
            writer.WriteObjectStart();
            if (isSaveMainShip)
            {
                //保存本体
                writer.WritePropertyName("0");
                writer.WriteObjectStart();
                IUtils.keyValue2Writer(writer, "realName", World.mapName);
                IUtils.keyValue2Writer(writer, "shipName", mainShipNamePath);
                IUtils.keyValue2Writer(writer, "assName", mainAssNamePath);
                IUtils.keyValue2Writer(writer, "pos", MainSubmarine.transform.position);
                IUtils.keyValue2Writer(writer, "rot", MainSubmarine.transform.eulerAngles);
                IUtils.keyValue2Writer(writer, "tonnage", MainSubmarine.tonnage);
                IUtils.keyValue2Writer(writer, "mass", MainSubmarine.rigidbody.mass);
                IUtils.keyValue2Writer(writer, "massCenter", MainSubmarine.weightCenter);
                IUtils.keyValue2Writer(writer, "shipOffsetY", Pooler.shipOffsetY);
                IUtils.keyValue2Writer(writer, "isDel",false);

                writer.WriteObjectEnd();
            }
           

            //保存其他
            for (int i = 0; i < count; i++)
            {
                ShipPreload shipPreload = ShipPreload.shipPreloads[i];
                string pn = (isSaveMainShip ? i + 1 : i).ToString();
                writer.WritePropertyName(pn);
                writer.WriteObjectStart();
                IUtils.keyValue2Writer(writer, "realName", shipPreload.realName);
                IUtils.keyValue2Writer(writer, "shipName", shipPreload.shipName);
                IUtils.keyValue2Writer(writer, "assName", shipPreload.assName);
                IUtils.keyValue2Writer(writer, "pos", shipPreload.transform.position);
                IUtils.keyValue2Writer(writer, "rot", shipPreload.transform.eulerAngles);
                IUtils.keyValue2Writer(writer, "tonnage", shipPreload.tonnage);
                IUtils.keyValue2Writer(writer, "mass", shipPreload.mass);
                IUtils.keyValue2Writer(writer, "massCenter", shipPreload.massCenter);
                IUtils.keyValue2Writer(writer, "shipOffsetY", shipPreload.shipOffsetY);
                IUtils.keyValue2Writer(writer, "isDel", shipPreload.isDel);
                writer.WriteObjectEnd();
            }

            writer.WriteObjectEnd();
            writer.WriteObjectEnd();
            saveWithCreateMd5(shipsInfoNamePath, writer.ToString());
        }

        static public void saveCraftsShipsInfo(List<CraftInfo> craftInfos)
        {

            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();

            int count = craftInfos.Count;
            IUtils.keyValue2Writer(writer, "count", count);

            writer.WritePropertyName("ships");
            writer.WriteObjectStart();    
           
            for (int i = 0; i < count; i++)
            {
                CraftInfo craftInfo = craftInfos[i];
                string pn = (i).ToString();
                writer.WritePropertyName(pn);
                writer.WriteObjectStart();
                IUtils.keyValue2Writer(writer, "realName", craftInfo.realName);
                IUtils.keyValue2Writer(writer, "shipName", craftInfo.shipName);
                IUtils.keyValue2Writer(writer, "assName", craftInfo.assName);
                IUtils.keyValue2Writer(writer, "pos", craftInfo.position);
                IUtils.keyValue2Writer(writer, "rot", craftInfo.eulerAngles);
                IUtils.keyValue2Writer(writer, "tonnage", craftInfo.tonnage);
                IUtils.keyValue2Writer(writer, "mass", craftInfo.mass);
                IUtils.keyValue2Writer(writer, "massCenter", craftInfo.massCenter);
                IUtils.keyValue2Writer(writer, "shipOffsetY", craftInfo.shipOffsetY);
                IUtils.keyValue2Writer(writer, "isDel", craftInfo.isDel);
                writer.WriteObjectEnd();
            }

            writer.WriteObjectEnd();
            writer.WriteObjectEnd();
            saveWithCreateMd5(shipsInfoNamePath, writer.ToString());
        }

        static public void loadWorldShipInfo()
        {
            if (shipPrefab == null)
            {
                shipPrefab = Resources.Load("Prefabs/Pooler/ship preload") as GameObject;
            }

            string data = readWithVerifyMd5(shipsInfoNamePath);
            if(data != null)
            {
                JsonData jsonData = JsonMapper.ToObject(data);
                int count = IUtils.getJsonValue2Int(jsonData, "count");
                JsonData shipsData = jsonData["ships"];
                for (int i = 0; i < count; i++)
                {
                    JsonData shipData = shipsData[i];
                    string assName = IUtils.getJsonValue2String(shipData, "assName");
                    bool isDel = IUtils.getJsonValue2Bool(shipData, "isDel");
                    if (!isDel && !(Pooler.IS_Form_StationCenter && Pooler.FromShip_AssName.Equals(assName)))
                    {     
                        ShipPreload preload = Object.Instantiate(shipPrefab).GetComponent<ShipPreload>();
                        preload.OnLoaded(shipData);
                    }                    
                }             
            }           
        }   

        static public void saveMainShip(string blocksMapData)
        {
           

            if (!Pooler.IS_Form_StationCenter)
            {
                mainShipNamePath = StringMD5((Random.value * 1000000).ToString());
                mainAssNamePath = StringMD5(mainShipNamePath + "Ass");
            }
            else
            {
                mainShipNamePath = Pooler.FromShip_ShipName;
                mainAssNamePath = Pooler.FromShip_AssName;
            }           

            saveWithCreateMd5(mainShipNamePath, blocksMapData);
            if (!Pooler.IS_Form_StationCenter)
            {
                saveWithCreateMd5(mainAssNamePath, Pooler.assData);
            }                
        }

        static public void saveStationsEntityInfo(List<Station> stations)
        {
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();
            IUtils.keyValue2Writer(writer, "count", stations.Count);
            Vector3[] vectors = Station.mainStation.getSpawnPosition();
            IUtils.keyValue2Writer(writer, "spwanPos", IUtils.vector3Serialize(vectors[0]));
            IUtils.keyValue2Writer(writer, "spwanRot", IUtils.vector3Serialize(vectors[1]));
            writer.WritePropertyName("stations");
            writer.WriteObjectStart();
            int i = 0;
            foreach (Station station in stations)
            {
                writer.WritePropertyName(station.id.ToString());
                writer.WriteObjectStart();
                IUtils.keyValue2Writer(writer, "isDefaultExit", station.isDefaultExit);
                IUtils.keyValue2Writer(writer, "id", station.id);
                IUtils.keyValue2Writer(writer, "name", station.name);                
                IUtils.keyValue2Writer(writer, "pos", IUtils.vector3Serialize(station.transform.position));
                IUtils.keyValue2Writer(writer, "rot", IUtils.vector3Serialize(station.transform.eulerAngles));

                //保存主站
                if (station.isMainStation())
                {
                    saveMainStation(station.id);
                }

                //保存货物
                JsonWriter cargoWriter = new JsonWriter();
                cargoWriter.WriteObjectStart();

                IUtils.keyValue2Writer(cargoWriter, "powerCount", station.getStorePowerCount());
                int cargoscount = station.cargoCounts.Length;
                IUtils.keyValue2Writer(cargoWriter, "cargosCount", cargoscount);
                cargoWriter.WritePropertyName("cargos");
                cargoWriter.WriteObjectStart();
                for (int j = 0; j < cargoscount; j++)
                {
                    IUtils.keyValue2Writer(cargoWriter, j.ToString(), station.cargoCounts[j]);
                }
                cargoWriter.WriteObjectEnd();
                cargoWriter.WriteObjectEnd();
                saveWithCreateMd5(StringMD5( "cargo" + station.id), cargoWriter.ToString());


                //保存附属设施
                int componentsCount = station.stationComponents.Count;
                IUtils.keyValue2Writer(writer, "componentsCount", componentsCount);
                writer.WritePropertyName("components");
                writer.WriteObjectStart();
                for (int j = 0; j < componentsCount; j++)
                {
                    writer.WritePropertyName(j.ToString());
                    writer.WriteObjectStart();
                    station.stationComponents[j].onSave(writer);
                    writer.WriteObjectEnd();
                }
                writer.WriteObjectEnd();

                writer.WriteObjectEnd();
                i++;
            }
            writer.WriteObjectEnd();
            writer.WriteObjectEnd();
            saveWithCreateMd5(stationInfoNamePath, writer.ToString());
        }

        static public void saveMainStation(int stationId)
        {
            mainStationId = stationId;
            IUtils.write2txt(DataPath + mainStationNamePath + Suffix, stationId.ToString());
        }

        static public int loadMainStation()
        {
            int stationId = 0;
            try
            {
                stationId = int.Parse(IUtils.readFromTxt(DataPath + mainStationNamePath + Suffix));
            }
            catch
            {

            }
            return stationId;
        }

        static public void loadStationsInfoAndInstance()
        {
            if(stationPrefab == null)
            {
                stationPrefab = Resources.Load("Prefabs/Station/station preload point") as GameObject;
            }
            
            string data = readWithVerifyMd5(stationInfoNamePath);
            mainStationId = loadMainStation();
            int loadCount = 0;
            if (data != null)
            {
                JsonData jsonData = JsonMapper.ToObject(data);
                int count = IUtils.getJsonValue2Int(jsonData, "count");            
                stationsData = jsonData["stations"];                
                for (int i = 0; i < count; i++)
                {
                    JsonData stationData = stationsData[i];
                    int id = IUtils.getJsonValue2Int(stationData, "id");
                    bool isDefaultExit = IUtils.getJsonValue2Bool(stationData, "isDefaultExit");                   
                    if (isDefaultExit)
                    {
                        Station.stations[0].OnPreloadInstance(stationData);
                    }
                    else
                    {
                        IPreload preload = Object.Instantiate(stationPrefab).GetComponent<IPreload>();
                        preload.transform.SetParent(GameObject.Find("runtime_gen").transform, true);
                        preload.setInfo(stationData);
                        preload.OnInstanceFinish += OnInstanceFinish;
                        preload.transform.position = IUtils.vector3Parse(IUtils.getJsonValue2String(stationData, "pos"));
                        preload.transform.eulerAngles = IUtils.vector3Parse(IUtils.getJsonValue2String(stationData, "rot"));
                        if(id == mainStationId)
                        {
                            preload.instanceGameObject();
                        }
                    }
                    loadCount++;
                }
            }

            if (loadCount < 1)
            {
                Station.stations[0].OnFirstInstance();
                if (!Pooler.IS_Form_StationCenter)
                {
                    spwans = Station.stations[0].getSpawnPosition();
                    Debug.Log(MainSubmarine.transform.position);
                }       
            }            
        }

        static public void OnInstanceFinish(IPreload preload, GameObject gameObject)
        {
            Station station = gameObject.GetComponent<Station>();
            station.OnPreloadInstance(preload.getInfo());
        }

        static public int loadStationsInfo(out StationInfo[] stationInfos)
        {
            string data = readWithVerifyMd5(stationInfoNamePath);
            mainStationId = loadMainStation();          
            if (data != null)
            {
                JsonData jsonData = JsonMapper.ToObject(data);               
                int count = IUtils.getJsonValue2Int(jsonData, "count");
                stationInfos = new StationInfo[count];
                stationsData = jsonData["stations"];
                GameObject stationPrefab = Resources.Load("Prefabs/Station/station preload point") as GameObject;
                for (int i = 0; i < count; i++)
                {
                    JsonData stationData = stationsData[i];                    
                    stationInfos[i] = new StationInfo(stationData);
                }
            }
            else
            {
                stationInfos = null;
            }
            return mainStationId;
        }

        static public void setScientific(float value)
        {
            m_scientific = value;
            saveScientificAndDiamonds();
        }

        static public void setDiamonds(float value)
        {
            m_diamonds = value;
            saveScientificAndDiamonds();
        }

        static public float getScientific()
        {
            return m_scientific;
        }

        static public float getDiamonds()
        {
            return m_diamonds;
        }

        static void saveScientificAndDiamonds()
        {
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();
            IUtils.keyValue2Writer(writer, "scientific_key", m_scientific);
            IUtils.keyValue2Writer(writer, "diamonds_key", m_diamonds);
            IUtils.keyValue2Writer(writer, "Interference", Time.time);
            writer.WriteObjectEnd();
            saveWithCreateMd5(sciAndDiaNamePath, writer.ToString());

            GameSetting.diamonds = m_diamonds;
            GameSetting.save();
        }

        static public void loadScientificAndDiamonds()
        {
            string data = readWithVerifyMd5(sciAndDiaNamePath);
            if (data != null)
            {
                JsonData jsonData = JsonMapper.ToObject(data);
                m_scientific = IUtils.getJsonValue2Float(jsonData, "scientific_key");
                m_diamonds = IUtils.getJsonValue2Float(jsonData, "diamonds_key");

                m_diamonds = GameSetting.diamonds >= 0 ? GameSetting.diamonds : 0;                
            }
        }

        static public void saveResearch(List<ResearchPoint> researchPoints)
        {
            List<int> unlockData = new List<int>();            
            foreach (var point in researchPoints)
            {
                if (point.isUnlock)
                {
                    foreach (var block in point.unlockBlocks)
                    {
                        unlockData.Add(block.getId());
                    }
                }
            }

            string data = IUtils.serializeIntArray(unlockData.ToArray());          
            saveWithCreateMd5(researchNamePath, data);
        }

        static public int[] loadResearch()
        {
            string data = readWithVerifyMd5(researchNamePath);
            if(data != null)
            {                
                return IUtils.unserializeIntArray(data);
            }
            return null;
        }

        static public void saveWithCreateMd5(string NamePath, string data)
        {
            // if (isEncrypt)
            // {
            //     data = IEncrypt.AesEncrypt(data, key);
            // }            
            // IUtils.write2txt(DataPath + MD5(NamePath) + Suffix, data);
            // IUtils.write2txt(Md5Path + NamePath + Suffix, MD5(data));
            var filepath = DataPath + NamePath + Suffix;
            IUtils.write2txt(filepath, data);
            Debug.Log("Saved: " + filepath);
        }

        static public string readWithVerifyMd5(string NamePath)
        {
            // string data = IUtils.readFromTxt(DataPath + MD5(NamePath) + Suffix);
            // string md5 = IUtils.readFromTxt(Md5Path + NamePath + Suffix);
            // if (data != null && md5 != null)
            // {
            //     if(MD5(data).Equals(md5))
            //     {
            //         return isEncrypt ? IEncrypt.AesDecrypt(data, key) : data;                    
            //     }
            // }
            // return null;
            var filepath = DataPath + NamePath + Suffix;
            Debug.Log("Read: " + filepath);
            return IUtils.readFromTxt(filepath);            
        }

        static public bool isFileExit(string NamePath)
        {
            string path = Md5Path + NamePath + Suffix;
            FileInfo fi = new FileInfo(path);
            return fi.Exists;
        }

        public static string StringMD5(string source)
        {
            source += "BFE2D63477221ECFCAD196F3D923E966";
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));
            }
            string md5Result = strbul.ToString();
            //Debug.Log("source:" + source + ",md5:" + md5Result);
            return md5Result;
        }

        public static string CalculateFileMD5(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    return System.BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }
        }
    }
}
