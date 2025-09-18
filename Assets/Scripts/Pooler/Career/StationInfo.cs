using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.BlockSpace;
using LitJson;
using UnityEditor;

namespace Scraft.StationSpace
{
    public class StationInfo
    {
        JsonData stationData;
        BlocksManager blocksManager;

        public List<ComponentInfo> componentInfos;
        bool isDefaultExit;
        bool m_isMainStation;
        public string name;
        public int id;
        public Sprite thumbnailSprite;
        public int[] cargoCounts;

        string thumbnailPath;

        Vector3 shotPosition;
        Vector3 shotEulerAngle;

        public int canStoreSoild;
        public int canStoreLiquid;
        public int canStorePower;

        int soildCount;
        int liquidCount;
        float storePower;

        Station station;

        public StationInfo(Station station)
        {
            blocksManager = BlocksManager.instance;
            this.station = station;


            isDefaultExit = station.isDefaultExit;
            m_isMainStation = station.isMainStation();
            name = station.name;
            id = station.id;
            thumbnailSprite = station.thumbnailSprite;
            cargoCounts = station.cargoCounts;

            thumbnailPath = station.thumbnailPath;

            shotPosition = new Vector3(-0.3755884f, 23.37269f, 25.64762f);
            shotEulerAngle = new Vector3(43.18f, 179.456f, 0);

            canStoreSoild = station.canStoreSoild;
            canStoreLiquid = station.canStoreLiquid;
            canStorePower = station.canStorePower;

            soildCount = station.getStoreSoildCount();
            liquidCount = station.getStoreLiquidCount();
        }

        public void updateInfo()
        {
            if (station != null)
            {
                isDefaultExit = station.isDefaultExit;
                m_isMainStation = station.isMainStation();
                name = station.name;
                id = station.id;
                thumbnailSprite = station.thumbnailSprite;
                cargoCounts = station.cargoCounts;

                thumbnailPath = station.thumbnailPath;              

                canStoreSoild = station.canStoreSoild;
                canStoreLiquid = station.canStoreLiquid;

                soildCount = station.getStoreSoildCount();
                liquidCount = station.getStoreLiquidCount();
                storePower = station.getStorePowerCount();
            }
        }

        public StationInfo(JsonData stationData)
        {
            blocksManager = BlocksManager.instance;
            isDefaultExit = IUtils.getJsonValue2Bool(stationData, "isDefaultExit");           
            name = IUtils.getJsonValue2String(stationData, "name");
            id = IUtils.getJsonValue2Int(stationData, "id");
            m_isMainStation = id == ISecretLoad.mainStationId;            

            //读取货物           
            string cargosDataStr = ISecretLoad.readWithVerifyMd5(ISecretLoad.MD5("cargo" + id));
            if (cargosDataStr != null)
            {
                JsonData cargosData = JsonMapper.ToObject(cargosDataStr);
                storePower = IUtils.getJsonValue2Float(cargosDataStr, "powerCount");
                int cargoscount = IUtils.getJsonValue2Int(cargosData, "cargosCount");
                cargoCounts = new int[BlocksManager.instance.getBlockCount()];
                JsonData cargosListData = cargosData["cargos"];
                int count = Mathf.Min(cargoscount, cargoCounts.Length);
                for (int i = 0; i < count; i++)
                {
                    JsonData cargoData = cargosListData[i];
                    cargoCounts[i] = int.Parse(cargoData.ToString());
                }
                updateStoreCount();
            }

            //读取附属设施
            componentInfos = new List<ComponentInfo>();
            int componentsCount = IUtils.getJsonValue2Int(stationData, "componentsCount");
            JsonData componentsData = stationData["components"];
            for (int i = 0; i < componentsCount; i++)
            {
                JsonData componentData = componentsData[i];
                bool isComponentDefaultExit = IUtils.getJsonValue2Bool(componentData, "isDefaultExit");
                int componentId = IUtils.getJsonValue2Int(componentData, "componentId");
                ComponentInfo componentInfo = StationsManager.getInstance().componentInfos[componentId];
                addStore(componentInfo.canStoreSoild, componentInfo.canStoreLiquid, componentInfo.canStorePower);
            }            

            thumbnailPath = string.Format("{0}station{1}.sthu", GamePath.worldThumbnailFolder, id);
        }

        public void saveCargos()
        {
            //保存货物
            JsonWriter cargoWriter = new JsonWriter();
            cargoWriter.WriteObjectStart();

            IUtils.keyValue2Writer(cargoWriter, "powerCount", getStorePowerCount());
            int cargoscount = cargoCounts.Length;
            IUtils.keyValue2Writer(cargoWriter, "cargosCount", cargoscount);
            cargoWriter.WritePropertyName("cargos");
            cargoWriter.WriteObjectStart();
            for (int j = 0; j < cargoscount; j++)
            {
                IUtils.keyValue2Writer(cargoWriter, j.ToString(), cargoCounts[j]);
            }
            cargoWriter.WriteObjectEnd();
            cargoWriter.WriteObjectEnd();
            ISecretLoad.saveWithCreateMd5(ISecretLoad.MD5("cargo" + id), cargoWriter.ToString());
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
                    updateStoreCount(1, cargoCounts[i] - lastCargoCount);
                }
            }
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

        public void addStore(int soild, int liquid, int power)
        {
            canStoreSoild += soild;
            canStoreLiquid += liquid;
            canStorePower += power;
        }

        public Sprite getThumbnailSprite()
        {
            if (thumbnailSprite == null)
            {
                Texture2D texture2D;
                try
                {
                    texture2D = IUtils.loadTexture2DFromSD(thumbnailPath);
                    thumbnailSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                }
                catch
                {
                   
                    
                }       
            }
            return thumbnailSprite;
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
                    if (blocksManager.getCanStoreInSoild(i))
                    {
                        soildCount += cargoCounts[i];
                    }
                    if (blocksManager.getCanStoreInLiquid(i))
                    {
                        liquidCount += cargoCounts[i];
                    }
                }
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

        public bool isMainStation()
        {
            return id == ISecretLoad.mainStationId;
        }
    }
}
