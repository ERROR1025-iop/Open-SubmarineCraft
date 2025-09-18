using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Scraft.BlockSpace;

namespace Scraft.StationSpace
{
    public class StationComponent : MonoBehaviour
    {

        Place3DItem place3DItem;
        
       
        public Transform spawn;
        public float generateWoodTime;        
        public float generatePowerTime;
        public QOutline QOutline;

        [Header("Only DefaultExit Write")]
        public bool isDefaultExit = false;
        public Station station;
        public ComponentInfo componentInfo;
        public int componentId;

        [HideInInspector] public bool isFromLoad = false;
        [HideInInspector] public int canStoreSoild;
        [HideInInspector] public int canStoreLiquid;
        [HideInInspector] public int canStorePower;
        BlocksManager blocksManager;

        void Start()
        {
            if (!isDefaultExit && !isFromLoad)
            {
                place3DItem = GetComponent<Place3DItem>();
                place3DItem.OnPlaceFinish += OnPlaceFinish;
            }
            else
            {
                componentInfo = StationsManager.instance.componentInfos[componentId];
                initialized(componentInfo);
            }

            if (isDefaultExit)
            {
                addStationCargosStore();
            }

            blocksManager = BlocksManager.instance;

            if (generateWoodTime != 0 )
            {
                StartCoroutine(generateWoodMethod());
            }

            if (generatePowerTime != 0)
            {
                StartCoroutine(generatePowerMethod());
            }
        }

        public void initialized(ComponentInfo componentInfo)
        {
            this.componentInfo = componentInfo;
            componentId = componentInfo.id;
            canStoreSoild = componentInfo.canStoreSoild;
            canStoreLiquid = componentInfo.canStoreLiquid;
            canStorePower = componentInfo.canStorePower;
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
            modify3DItem.setIsCanMove(true);
            modify3DItem.setStation(station);
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
            QOutline.enabled = false;
            Destroy(gameObject);
            PoolerCustomButton.instance.show(false);
            PoolerCustomButton.instance.setClickCallNull();
        }

        void onCancelClick()
        {
            QOutline.enabled = false;            
        }

        IEnumerator generateWoodMethod()
        {
            int woodId = blocksManager.wood.getId();
            while (true)
            {             
                if(station != null)
                {
                    if (station.isCanAddCargos(woodId, 1) == 1)
                    {
                        station.addCargos(woodId, 1);                       
                    }
                }                
                yield return new WaitForSeconds(generateWoodTime);
            }
        }

        IEnumerator generatePowerMethod()
        {          
            while (true)
            {
                if (station != null)
                {
                    station.addPower(10);
                }                    
                yield return new WaitForSeconds(generatePowerTime);
            }
        }

        public void onSave(JsonWriter writer)
        {
            IUtils.keyValue2Writer(writer, "isDefaultExit", isDefaultExit);
            IUtils.keyValue2Writer(writer, "componentId", componentId);
            IUtils.keyValue2Writer(writer, "pos", IUtils.vector3Serialize(transform.position));
            IUtils.keyValue2Writer(writer, "rot", IUtils.vector3Serialize(transform.eulerAngles));
        }

        public void onLoad(Station station ,JsonData componentData)
        {
            this.station = station;
            isFromLoad = true;
            transform.position = IUtils.vector3Parse(IUtils.getJsonValue2String(componentData, "pos"));
            transform.eulerAngles = IUtils.vector3Parse(IUtils.getJsonValue2String(componentData, "rot"));
            addStationCargosStore();
        }

        void OnPlaceFinish(Station station)
        {
            this.station = station;
            station.stationComponents.Add(this);
            addStationCargosStore();
        }
        
        void addStationCargosStore()
        {
            station.addStore(canStoreSoild, canStoreLiquid, canStorePower);
        }

        void OnDestroy()
        {
            if(station != null)
            {
                station.decStore(canStoreSoild, canStoreLiquid, canStorePower);
                station.stationComponents.Remove(this);
            }
        }

        public Transform getSpawn()
        {
            return spawn;
        }
    }
}
