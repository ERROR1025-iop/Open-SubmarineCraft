using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft;
using LitJson;
using Scraft.BlockSpace;
using UnityEditor;

namespace Scraft.StationSpace
{
    public class Station : MonoBehaviour
    {

        static public List<Station> stations;

        static public Station satyStation;
        static public bool isSatyInStation;
        static public int isSatyInStationCount;
        static public Station mainStation;

        public Transform spawn;
        public int canStoreSoild;
        public int canStoreLiquid;
        public int canStorePower;
        public QOutline QOutline;

        [Header("Only DefaultExit Write")]
        public List<StationComponent> stationComponents;
        public bool isDefaultExit = false;

        [HideInInspector] bool m_isMainStation;
        [HideInInspector] public string name;
        [HideInInspector] public int id;
        [HideInInspector] public Sprite thumbnailSprite;
        [HideInInspector] public int[] cargoCounts;
        [HideInInspector] public string thumbnailPath;

        bool IsNewPlace = false;
        bool chargePower = false;

        Vector3 shotPosition;
        Vector3 shotEulerAngle;

        int soildCount;
        int liquidCount;
        float storePower;

        StationInfo stationInfo;


        void Awake()
        {
            shotPosition = new Vector3(-0.3755884f, 23.37269f, 25.64762f);
            shotEulerAngle = new Vector3(43.18f, 179.456f, 0);

            if (isDefaultExit)
            {
                stations.Add(this);
            }

            soildCount = 0;
            liquidCount = 0;
            storePower = 0;
            cargoCounts = new int[BlocksManager.instance.getBlockCount()];
        }

        void Start()
        {
            stationInfo = new StationInfo(this);
        }

        void OnDestroy()
        {
            stations.Remove(this);
        }

        public void onPoolerClick()
        {
            PoolerCustomButton.instance.initialized("Modify", onButton1Click, onDeleteClick, onCancelClick);
            PoolerCustomButton.instance.show(true);
            QOutline.enabled = true;
        }

        void onButton1Click()
        {
            if (isDefaultExit)
            {
                IToast.instance.show("The default station cannot be edited", 100);
                return;
            }

            Modify3DItem modify3DItem = gameObject.AddComponent<Modify3DItem>();
            modify3DItem.setIsCanMove(false);
            modify3DItem.setStation(this);
        }

        void onDeleteClick()
        {
            if (isDefaultExit)
            {
                IToast.instance.show("The default station cannot be deleted", 100);
            }
            else
            {
                IConfigBox.instance.show(ILang.get("Are you sure to delete it? Material will not return"), onDeleteConfirmButtonClick, null);
            }

        }

        void onDeleteConfirmButtonClick()
        {
            foreach (StationComponent stationComponent in stationComponents)
            {
                Destroy(stationComponent.gameObject);
            }
            stationComponents.Clear();
            QOutline.enabled = false;
            Destroy(gameObject);
            PoolerCustomButton.instance.show(false);
            PoolerCustomButton.instance.setClickCallNull();
        }

        void onCancelClick()
        {
            QOutline.enabled = false;
        }

        public void addStore(int soild, int liquid, int power)
        {
            canStoreSoild += soild;
            canStoreLiquid += liquid;
            canStorePower += power;
        }

        public void decStore(int soild, int liquid, int power)
        {
            canStoreSoild -= soild;
            canStoreLiquid -= liquid;
            canStorePower -= power;
        }

        public void updateStoreCount()
        {
            soildCount = 0;
            liquidCount = 0;
            int cargoCount = cargoCounts.Length;
            for (int i = 0; i < cargoCount; i++)
            {
                if (cargoCounts[i] > 0)
                {
                    if (BlocksManager.instance.getCanStoreInSoild(i))
                    {
                        soildCount += cargoCounts[i];
                    }
                    if (BlocksManager.instance.getCanStoreInLiquid(i))
                    {
                        liquidCount += cargoCounts[i];
                    }
                }
            }
        }

        /// <summary>
        /// 更新存储数量，数量为负则是减少
        /// </summary>
        public void updateStoreCount(int canStoreInTag, int count)
        {
            if (canStoreInTag == 1)
            {
                soildCount += count;
            }
            else if (canStoreInTag == 2)
            {
                liquidCount += count;
            }
        }

        public int getStoreSoildCount()
        {
            return soildCount;
        }

        public int getStoreLiquidCount()
        {
            return liquidCount;
        }

        public float getStorePowerCount()
        {
            return storePower;
        }

        /// <summary>
        /// 是否可以添加进仓库？True:可以添加;False:仓库已满
        /// </summary>
        public bool isCanAddCargos(int[] cargos)
        {
            int count = Mathf.Min(cargos.Length, cargoCounts.Length);
            Block block;
            int l_soildCount = soildCount;
            int l_liquidCount = liquidCount;
            int l_soildCount0 = 0;
            int l_liquidCount0 = 0;
            for (int i = 0; i < count; i++)
            {
                if (cargos[i] <= 0)
                {
                    continue;
                }
                block = BlocksManager.instance.getBlockById(i);
                if (block.isCanStoreInWarehouse() == 1)
                {
                    var c = cargos[i];
                    l_soildCount0 += c;
                    //Debug.Log(i + "," + c + "," + l_soildCount + l_soildCount0);
                }
                else if (block.isCanStoreInWarehouse() == 2)
                {
                    var c = cargos[i];
                    l_liquidCount0 += c;
                }                
            }
            if (l_soildCount + l_soildCount0 > canStoreSoild || l_liquidCount + l_liquidCount0 > canStoreLiquid)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 是否可以添加进仓库？返回能添加成功的数量
        /// </summary>     
        public int isCanAddCargos(int blockId, int count)
        {
            Block block = BlocksManager.instance.getBlockById(blockId);
            if (block.isCanStoreInWarehouse() == 1)
            {
                if (soildCount + count > canStoreSoild)
                {
                    return canStoreSoild - soildCount;
                }
            }
            else if (block.isCanStoreInWarehouse() == 2)
            {
                if (liquidCount + count > canStoreLiquid)
                {
                    return canStoreLiquid - liquidCount;
                }
            }
            return count;
        }

        public void addCargos(int[] cargos)
        {
            int count = Mathf.Min(cargos.Length, cargoCounts.Length);
            Block block;
            int lastCargoCount;
            for (int i = 0; i < count; i++)
            {
                if (cargos[i] <= 0)
                {
                    continue;
                }

                block = BlocksManager.instance.getBlockById(i);
                lastCargoCount = cargoCounts[i];
                if (block.isCanStoreInWarehouse() == 1)
                {
                    if (soildCount + cargos[i] <= canStoreSoild)
                    {
                        var c = cargos[i];
                        //Debug.Log("x" + i + "," + c + "," + soildCount);
                        cargoCounts[i] += cargos[i];
                    }
                    else
                    {
                        cargoCounts[i] += canStoreSoild - soildCount;
                    }
                    cargoCounts[i] = Mathf.Clamp(cargoCounts[i], 0, canStoreSoild);
                    updateStoreCount(1, cargoCounts[i] - lastCargoCount);
                }
                else if (block.isCanStoreInWarehouse() == 2)
                {
                    if (liquidCount + cargos[i] <= canStoreLiquid)
                    {
                        cargoCounts[i] += cargos[i];
                    }
                    else
                    {
                        cargoCounts[i] += canStoreLiquid - liquidCount;
                    }
                    cargoCounts[i] = Mathf.Clamp(cargoCounts[i], 0, canStoreLiquid);
                    updateStoreCount(2, cargoCounts[i] - lastCargoCount);
                }
            }
        }

        public void addCargos(int blockId, int count)
        {
            Block block = BlocksManager.instance.getBlockById(blockId);
            int lastCargoCount = cargoCounts[blockId];
            if (block.isCanStoreInWarehouse() == 1)
            {
                if (soildCount + count <= canStoreSoild)
                {
                    cargoCounts[blockId] += count;
                }
                else
                {
                    cargoCounts[blockId] += canStoreSoild - soildCount;
                }
            }
            else if (block.isCanStoreInWarehouse() == 2)
            {
                if (liquidCount + count <= canStoreLiquid)
                {
                    cargoCounts[blockId] += count;
                }
                else
                {
                    cargoCounts[blockId] += canStoreLiquid - liquidCount;
                }
            }
            updateStoreCount(block.isCanStoreInWarehouse(), cargoCounts[blockId] - lastCargoCount);
        }

        public bool IsContainCargo(int[,] syntData)
        {
            int makingsCount = syntData.GetLength(0);
            for (int k = 0; k < makingsCount; k++)
            {
                int id = syntData[k, 0];
                int count = syntData[k, 1];
                if (cargoCounts[id] < count)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsContainCargo(int blockId, int count)
        {            
            if (cargoCounts[blockId] < count)
            {
                return false;
            }
            return true;
        }

        public void removeCargos(int[,] syntData)
        {
            int makingsCount = syntData.GetLength(0);
            for (int k = 0; k < makingsCount; k++)
            {
                int id = syntData[k, 0];
                int count = syntData[k, 1];

                int lastCargoCount = cargoCounts[id];
                cargoCounts[id] -= count;

                Block block = BlocksManager.instance.getBlockById(id);
                updateStoreCount(block.isCanStoreInWarehouse(), cargoCounts[id] - lastCargoCount);

            }
        }

        public void removeCargos(int blockId, int count)
        {
            Block block = BlocksManager.instance.getBlockById(blockId);
            int lastCargoCount = cargoCounts[blockId];
            if (lastCargoCount - count >= 0)
            {
                cargoCounts[blockId] -= count;
            }
            else
            {
                cargoCounts[blockId] = 0;
            }
            updateStoreCount(block.isCanStoreInWarehouse(), cargoCounts[blockId] - lastCargoCount);
        }

        /// <summary>
        /// 返回可充入的电能。
        /// </summary>
        public float getCanChargePower()
        {
            float unCharge = canStorePower - storePower;
            return unCharge;
        }

        /// <summary>
        /// 充电,返回未充入的电能。
        /// </summary>
        public float addPower(float power)
        {
            float unCharge = 0;
            if (storePower + power > canStorePower)
            {
                unCharge = storePower + power - canStorePower;
            }
            storePower += power;
            storePower = Mathf.Clamp(storePower, 0, canStorePower);
            return unCharge;
        }

        /// <summary>
        /// 放电,返回放出的电能。
        /// </summary>
        public float decPower(float power)
        {
            float release = 0;
            if (storePower - power >= 0)
            {
                release = power;
                storePower -= power;
            }
            else
            {
                release = power - storePower;
                storePower = 0;
            }
            return release;
        }

        public void OnFirstInstance()
        {
            id = 0;
            name = ILang.get("station") + " 1";
            thumbnailPath = string.Format("{0}station{1}.sthu", GamePath.worldThumbnailFolder, id);
            setMainStation(true);
            addStore(300, 30, 0);
            cargoCounts = loadRemaindeCargo();
            if(cargoCounts != null)
            {
                updateStoreCount();
            }            
        }

        public int[] loadRemaindeCargo()
        {
            if (Pooler.mainStationRemaindeCargoCounts == null)
            {
                string data = IUtils.readFromTxt(GamePath.cacheFolder + "remaindeCargo.txt");
                if(data != null)
                {
                    JsonData jsonData = JsonMapper.ToObject(data);
                    int count = IUtils.getJsonValue2Int(jsonData, "count");
                    int[] f_cargoCounts = new int[BlocksManager.instance.getBlockCount()];
                    count = Mathf.Min(f_cargoCounts.Length, count);
                    for (int i = 0; i < count; i++)
                    {
                        f_cargoCounts[i] = int.Parse(jsonData[i + 1].ToString());
                    }
                    return f_cargoCounts;
                }
                else
                {
                    Pooler.mainStationRemaindeCargoCounts = BlocksManager.instance.getRemaindeCargoCounts();
                }            
            }
            return Pooler.mainStationRemaindeCargoCounts;
        }

        public void OnPreloadInstance(JsonData stationData)
        {
            isDefaultExit = IUtils.getJsonValue2Bool(stationData, "isDefaultExit");
            name = IUtils.getJsonValue2String(stationData, "name");
            id = IUtils.getJsonValue2Int(stationData, "id");
            setMainStation(id == ISecretLoad.mainStationId);


            //读取货物           
            string cargosDataStr = ISecretLoad.readWithVerifyMd5(ISecretLoad.StringMD5("cargo" + id));
            if (cargosDataStr != null)
            {
                JsonData cargosData = JsonMapper.ToObject(cargosDataStr);
                storePower = IUtils.getJsonValue2Float(cargosData, "powerCount");
                if (!m_isMainStation)
                {
                    int cargoscount = IUtils.getJsonValue2Int(cargosData, "cargosCount");
                    cargoCounts = new int[BlocksManager.instance.getBlockCount()];
                    JsonData cargosListData = cargosData["cargos"];
                    int count = Mathf.Min(cargoscount, cargoCounts.Length);
                    for (int i = 0; i < count; i++)
                    {
                        JsonData cargoData = cargosListData[i];
                        cargoCounts[i] = int.Parse(cargoData.ToString());
                    }
                }
                else
                {
                    cargoCounts = loadRemaindeCargo();
                }
                updateStoreCount();
            }

            //
            addStore(300, 30, 0);
            //读取附属设施
            if (!isDefaultExit)
            {
                stationComponents = new List<StationComponent>();
            }
            int componentsCount = IUtils.getJsonValue2Int(stationData, "componentsCount");
            JsonData componentsData = stationData["components"];
            for (int i = 0; i < componentsCount; i++)
            {
                JsonData componentData = componentsData[i];
                bool isComponentDefaultExit = IUtils.getJsonValue2Bool(componentData, "isDefaultExit");
                if (!isComponentDefaultExit)
                {
                    int componentId = IUtils.getJsonValue2Int(componentData, "componentId");
                    ComponentInfo componentInfo = StationsManager.instance.componentInfos[componentId];
                    StationComponent stationComponent = componentInfo.instanceComponet();
                    stationComponent.initialized(componentInfo);
                    stationComponent.onLoad(this, componentData);
                    stationComponents.Add(stationComponent);
                }
            }

            if (!isDefaultExit)
            {
                stations.Add(this);
            }

            if (!Pooler.IS_Form_StationCenter && m_isMainStation)
            {
                ISecretLoad.spwans = getSpawnPosition();
                StartCoroutine(SetMainSubmarinePositionCoroutine());
            }

            thumbnailPath = string.Format("{0}station{1}.sthu", GamePath.worldThumbnailFolder, id);
        }

        IEnumerator SetMainSubmarinePositionCoroutine()
        {
            // 等待一帧
            yield return null;
            MainSubmarine.transform.position = ISecretLoad.spwans[0];
            MainSubmarine.transform.eulerAngles = ISecretLoad.spwans[1];
        }
        
        

        public void setMainStation(bool isSet)
        {
            m_isMainStation = isSet;
            if (isSet)
            {
                if (mainStation != null)
                {
                    mainStation.setMainStation(false);
                }
                mainStation = this;
            }
        }

        public bool isMainStation()
        {
            return m_isMainStation;
        }

        public Vector3[] getSpawnPosition()
        {
            Vector3[] vectors = new Vector3[2];
            foreach (StationComponent stationComponent in stationComponents)
            {
                Transform spawn = stationComponent.getSpawn();
                if (spawn != null)
                {
                    vectors[0] = spawn.position;
                    vectors[1] = spawn.eulerAngles;
                    return vectors;
                }
            }

            if (spawn != null)
            {
                vectors[0] = spawn.position;
                vectors[1] = Vector3.zero;
            }
            else
            {
                vectors[0] = Vector3.zero;
                vectors[1] = Vector3.zero;
            }
            return vectors;
        }

        public void OnPlaceFinish(Station station)
        {
            id = stations.Count;
            name = "station " + id;
            stations.Add(this);
            m_isMainStation = false;
            thumbnailPath = string.Format("{0}station{1}.sthu", GamePath.worldThumbnailFolder, id);

            cargoCounts = new int[BlocksManager.instance.getBlockCount()];
            IUtils.initializedArray(cargoCounts, 0);
            stationComponents = new List<StationComponent>();
        }

        public void setIsNewPlace()
        {
            IsNewPlace = true;
            addStore(300, 30, 0);
        }

        public StationInfo getStationInfo()
        {
            return stationInfo;
        }

        static public void saveStations()
        {
            foreach (var station in stations)
            {
                station.saveStation();
            }
            ISecretLoad.saveStationsEntityInfo(stations);
        }

        static public void loadStations()
        {
            ISecretLoad.loadStationsInfoAndInstance();
        }

        public void saveStation()
        {
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();
            IUtils.keyValue2Writer(writer, "count", stationComponents.Count);
            int i = 0;
            foreach (StationComponent component in stationComponents)
            {
                writer.WritePropertyName(i.ToString());
                writer.WriteObjectStart();
                IUtils.keyValue2Writer(writer, "isDefaultExit", isDefaultExit);
                IUtils.keyValue2Writer(writer, "id", component.componentInfo.id);
                IUtils.keyValue2Writer(writer, "pos", IUtils.vector3Serialize(component.transform.position));
                IUtils.keyValue2Writer(writer, "rot", IUtils.vector3Serialize(component.transform.eulerAngles));
                component.onSave(writer);
                writer.WriteObjectEnd();
                i++;
            }
            writer.WriteObjectEnd();
            //ISecretLoad.saveWithCreateMd5(ISecretLoad.MD5("station" + name), writer.ToString());
        }

        private void Update()
        {
            if (MainSubmarine.isRunSurface)
            {
                float distance = Vector3.Distance(MainSubmarine.transform.position, transform.position);
                if (distance < 21)
                {
                    isSatyInStation = true;
                    satyStation = this;
                    isSatyInStationCount = 10;
                    chargeMainSubmarine();
                }
            }
        }

        public void startChargePower()
        {
            chargePower = true;
        }

        public void chargeMainSubmarine()
        {
            if (chargePower)
            {
                if (Pooler.instance.getCanChargeElectric() > 0.1f)
                {
                    if (storePower > 50)
                    {
                        Pooler.instance.chargeElectric(null, 50);
                        decPower(50);
                    }
                    else
                    {
                        Pooler.instance.chargeElectric(null, storePower);
                        decPower(storePower);
                        chargePower = false;
                    }
                }
                else
                {
                    chargePower = false;
                }
            }           
        }

        public Texture2D createThumbnailImage()
        {
            //拍摄缩略图            
            return PoolerShotCamera.instance.shot(transform.position + shotPosition, shotEulerAngle, 12f, thumbnailPath, new Rect(0, 0, 500, 400));
        }

        public void updateThumbnailImage()
        {
            Texture2D texture2D = createThumbnailImage();
            thumbnailSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        }

        public Sprite getThumbnailSprite()
        {
            if(thumbnailSprite == null)
            {
                Texture2D texture2D;
                try
                {
                    texture2D = IUtils.loadTexture2DFromSD(thumbnailPath);                                   
                }
                catch
                {
                    texture2D = createThumbnailImage();
                }
                thumbnailSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
              
            }
            return thumbnailSprite;
        }
    }
}
