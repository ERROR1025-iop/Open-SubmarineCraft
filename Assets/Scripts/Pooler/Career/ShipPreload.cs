using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Scraft.DpartSpace;

namespace Scraft
{
    public class ShipPreload : MonoBehaviour
    {
        static public List<ShipPreload> shipPreloads;
               
        public Icon3D icon3D;

        [HideInInspector]public string realName;
        [HideInInspector]public string shipName;
        [HideInInspector]public string assName;
        [HideInInspector]public float tonnage;
        [HideInInspector]public float mass;
        [HideInInspector]public Vector3 massCenter;
        [HideInInspector]public float shipOffsetY;
        [HideInInspector]public bool isDel;
        [HideInInspector]public QOutline outline;

        Vector3 position;
        Vector3 eulerAngle;

        bool isInstance;
        float distance;
        int interval_time;

        void Start()
        {
            shipPreloads.Add(this);
            icon3D = GetComponent<Icon3D>();
            distance = 400;
            interval_time = (int)(Random.value * 80f);            
        }

        private void OnDestroy()
        {
            shipPreloads.Remove(this);
        }

        public void SetAllDescendantsToSelfShipTag()
        {
            // 设自身Tag
            gameObject.tag = "preload ship";
            // 递归遍历所有子物体（含孙物体等所有层级）
            foreach (Transform child in transform)
            {
                child.gameObject.tag = "preload ship";
                // 递归处理子物体的子物体
                foreach (Transform grandChild in child)
                {
                    SetAllDescendantsToSelfShipTagRecursive(grandChild);
                }
            }
        }

        // 内部递归辅助（必须保留，否则无法遍历多层级，且不额外暴露给外部）
        private void SetAllDescendantsToSelfShipTagRecursive(Transform current)
        {
            current.gameObject.tag = "preload ship";
            foreach (Transform child in current)
            {
                SetAllDescendantsToSelfShipTagRecursive(child);
            }
        }

        public void OnLoaded(JsonData shipData)
        {
            realName = IUtils.getJsonValue2String(shipData, "realName");
            shipName = IUtils.getJsonValue2String(shipData, "shipName");
            assName = IUtils.getJsonValue2String(shipData, "assName");
            tonnage = IUtils.getJsonValue2Float(shipData, "tonnage");
            mass = IUtils.getJsonValue2Float(shipData, "mass");
            massCenter = IUtils.getJsonValue2Vector3(shipData, "massCenter");
            shipOffsetY = IUtils.getJsonValue2Float(shipData, "shipOffsetY");
            position = IUtils.getJsonValue2Vector3(shipData, "pos");
            eulerAngle = IUtils.getJsonValue2Vector3(shipData, "rot");
            isDel = IUtils.getJsonValue2Bool(shipData, "isDel");

            gameObject.name = string.Format("preload ship({0})", realName);
            transform.position = position;
            transform.eulerAngles = eulerAngle;
            
        }


        void Update()
        {
            if (interval_time > 100)
            {
                if (Vector3.Distance(MainSubmarine.transform.position, transform.position) < distance)
                {
                    instanceGameObject();
                }
                interval_time = 0;
            }
            interval_time++;
        }

        public void instanceGameObject()
        {
            if (isInstance)
            {
                return;
            }

            if(Pooler.IS_Form_StationCenter && Pooler.FromShip_AssName.Equals(assName))
            {
                Destroy(gameObject);
                return;
            }

            StartCoroutine(createDpart());
            outline = gameObject.AddComponent<QOutline>();
            outline.OutlineColor = Color.green;
            outline.OutlineWidth = 5;
            outline.enabled = false;

            isInstance = true;
        }

        public void onPoolerClick()
        {
            PoolerCustomButton.instance.initialized("Drive", onButton1Click, onDeleteClick, onCancelClick);
            PoolerCustomButton.instance.show(true);
            outline.enabled = true;
        }

        void onButton1Click()
        {
            IToast.instance.show("Loading");
            Pooler.instance.savePoolerData(false);
            World.mapName = realName;
            Pooler.FromShip_ShipName = shipName;
            Pooler.FromShip_AssName = assName;
            Pooler.FromShip_Position = position;
            Pooler.FromShip_EulerAngle = eulerAngle;
            Pooler.IS_Form_StationCenter = true;           
            string backgroundPath = string.Format("Menu/Loading/{0}{1}", GameSetting.isCreateAi ? "n" : "s", (int)(Random.value * 2.9f));
            AsyncLoadScene.sprite = Resources.Load(backgroundPath, typeof(Sprite)) as Sprite;
            AsyncLoadScene.asyncloadScene("pooler");
        }

        void onDeleteClick()
        {
            IConfigBox.instance.show(ILang.get("Are you sure to delete it? Material will not return"), onDeleteConfirmButtonClick, null);

        }

        void onDeleteConfirmButtonClick()
        {
            outline.enabled = false;
            Destroy(gameObject);
            PoolerCustomButton.instance.show(false);
            PoolerCustomButton.instance.setClickCallNull();
        }

        void onCancelClick()
        {
            outline.enabled = false;
        }

        IEnumerator createDpart()
        {
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;

            DpartsEngine dpartsEngine = new DpartsEngine(gameObject);
            DpartsManager dpartsManager = DpartsManager.instance;

            dpartsEngine.clearDpartArr();
            string assData = ISecretLoad.readWithVerifyMd5(assName);
            JsonData jsonData = JsonMapper.ToObject(assData);
            int blocksCount = IUtils.getJsonValue2Int(jsonData, "count");

            if (blocksCount == 0)
            {
                TextAsset textAsset = Resources.Load("Pooler/submarine") as TextAsset;
                jsonData = JsonMapper.ToObject(textAsset.text);
                blocksCount = IUtils.getJsonValue2Int(jsonData, "count");
            }

            JsonData blocksArrData = jsonData["dparts"];

            JsonData dpartData;
            Dpart dpart;
            Transform parentTrans = gameObject.transform;

            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();

            for (int i = 0; i < blocksCount; i++)
            {
                dpartData = blocksArrData[i];

                int dpartId = IUtils.getJsonValue2Int(dpartData, "id");
                if (dpartId == -1)
                {
                    GroupDpart groupDpart = new GroupDpart(-1, gameObject);
                    groupDpart.initGroupDpart("group_" + i);                    
                    dpart = groupDpart;
                }
                else
                {
                    dpart = dpartsEngine.createDPart(dpartsManager.getDPartById(dpartId));
                }
                dpart.onBuilderModeLoad(dpartData, dpartsEngine);
                dpart.onPoolerModeCreate();    
                yield return 0;
            }
                     
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();

            rigidbody.useGravity = false;
            rigidbody.mass = mass;
            rigidbody.centerOfMass = massCenter; 
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            // foreach (Collider collider in colliders)
            // {
            //     Destroy(collider);
            // }
          
            IUtils.centerOnChildrens(gameObject, new Vector3(0, 0, 0));           
            //combineRenderMeshes(gameObject);         
            //Bounds bounds = meshFilter.mesh.bounds;
            // BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            // boxCollider.center = bounds.center;
            // boxCollider.size = bounds.size;
            transform.position = position;
            transform.eulerAngles = eulerAngle;

            LowBuoyancy lowBuoyancy = gameObject.AddComponent<LowBuoyancy>();
            lowBuoyancy.Initialized(tonnage, massCenter, shipOffsetY);
            
            SetAllDescendantsToSelfShipTag();
        }

        static void combineRenderMeshes(GameObject gos)
        {
            DpartChild[] dpartChilds = gos.GetComponentsInChildren<DpartChild>();
            DpartChild child;

            int normalCount = 0;
            int combineCount = 0;
            for (int i = 0; i < dpartChilds.Length; i++)
            {
                child = dpartChilds[i];
                if (child.name.Equals("Lightbeam"))
                {
                    child.isCombineRenderMesh = false;
                }
                else
                {
                    normalCount++;
                    combineCount++;
                    child.isCombineRenderMesh = true;
                }     
            }

            if (combineCount == 1)
            {
                return;
            }

            CombineInstance[] combine = new CombineInstance[combineCount];

            int combineStack = 0;
            for (int i = 0; i < dpartChilds.Length; i++)
            {
                child = dpartChilds[i];
                if (child.isCombineRenderMesh)
                {
                    MeshFilter meshFilters = child.transform.GetComponent<MeshFilter>();
                    if (meshFilters != null)
                    {
                        combine[combineStack].mesh = meshFilters.sharedMesh;
                        combine[combineStack].transform = meshFilters.transform.localToWorldMatrix;
                        combineStack++;
                    }
                    child.gameObject.SetActive(false);
                    Destroy(child.gameObject);
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "Render mesh";
            mesh.CombineMeshes(combine);

            gos.GetComponent<MeshFilter>().mesh = mesh;
            gos.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Dparts/ColorMat/normal", typeof(Material)) as Material;
        }

    }
}
