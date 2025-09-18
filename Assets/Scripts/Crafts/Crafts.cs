using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace Scraft
{
    public class Crafts : MonoBehaviour
    {

        static public Crafts instance;

        public IGridScrollView craftsScrollView;
        public Button startButton;
        public Button destroyButton;
        public Transform shipIconsParent;
        public GameObject shipIconPrefabs;

        List<CraftInfo> craftInfos;
        List<CraftShipIcon> shipIcons;
        
        CraftInfo selectedCraftInfo;
        CraftShipIcon selectedshipIcon;

        void Start()
        {
            instance = this;
            GameObject.Find("Canvas/Back").GetComponent<Button>().onClick.AddListener(onBackButtonClick);

            ISecretLoad.init();
            loadWorldShipInfo();                        

            if(craftInfos != null)
            {
                craftsScrollView.initialized(OnCreateCraftListCells);
                craftsScrollView.setInformation(craftInfos);
                craftsScrollView.OnCellClick += onCraftListCellClick;
            }

            startButton.onClick.AddListener(onStartButtonClick);
            destroyButton.onClick.AddListener(onDestroyButtonClick);
        }

        IGridScrollViewCell OnCreateCraftListCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new CraftListCell(gridScrollViewInfo);
        }

        void onCraftListCellClick(IGridScrollViewCell cell)
        {
            CraftListCell carftListCell = cell as CraftListCell;
            selectedCraftInfo = carftListCell.getCraftInfo();
            setShipIconActivity(cell.getIndex());
        }

        void loadWorldShipInfo()
        {          
            string data = ISecretLoad.readWithVerifyMd5(ISecretLoad.shipsInfoNamePath);
            if (data != null)
            {
                craftInfos = new List<CraftInfo>();
                shipIcons = new List<CraftShipIcon>();
                JsonData jsonData = JsonMapper.ToObject(data);
                int count = IUtils.getJsonValue2Int(jsonData, "count");
                JsonData shipsData = jsonData["ships"];
                for (int i = 0; i < count; i++)
                {
                    JsonData shipData = shipsData[i];
                    CraftInfo craftInfo = new CraftInfo(shipData);
                    if (!craftInfo.isDel)
                    {
                        craftInfos.Add(craftInfo);

                        CraftShipIcon craftShipIcon = Instantiate(shipIconPrefabs).GetComponent<CraftShipIcon>();
                        craftShipIcon.transform.SetParent(shipIconsParent);
                        craftShipIcon.initialized(this, i, craftInfo);
                        shipIcons.Add(craftShipIcon);
                    }
        
                }
            }
        }

        public void onShipIconClick(int index)
        {
            craftsScrollView.setCellActivity(index);
            setShipIconActivity(index);
        }

        void setShipIconActivity(int index)
        {
            if (selectedshipIcon != null)
            {
                selectedshipIcon.setActivity(false);
            }
            selectedshipIcon = shipIcons[index];
            selectedshipIcon.setActivity(true);
        }

        void onStartButtonClick()
        {
            if (selectedCraftInfo != null)
            {
                IToast.instance.show("Loading");
                World.mapName = selectedCraftInfo.realName;
                Pooler.FromShip_ShipName = selectedCraftInfo.shipName;
                Pooler.FromShip_AssName = selectedCraftInfo.assName;
                Pooler.FromShip_Position = selectedCraftInfo.position;
                Pooler.FromShip_EulerAngle = selectedCraftInfo.eulerAngles;
                Pooler.IS_Form_StationCenter = true;
                string backgroundPath = string.Format("Menu/Loading/{0}{1}", GameSetting.isCreateAi ? "n" : "s", (int)(Random.value * 2.9f));
                AsyncLoadScene.sprite = Resources.Load(backgroundPath, typeof(Sprite)) as Sprite;
                AsyncLoadScene.asyncloadScene("pooler");
            }
        }

        void onDestroyButtonClick()
        {
            if (selectedCraftInfo != null)
            {
                IConfigBox.instance.show(string.Format(ILang.get("confirm abandon"), selectedCraftInfo.realName), onConfirmDestroy, null);
            }               
        }

        void onConfirmDestroy()
        {
            if (selectedCraftInfo != null)
            {
                selectedCraftInfo.isDel = true;
                ISecretLoad.saveCraftsShipsInfo(craftInfos);
                craftInfos.Remove(selectedCraftInfo);
                shipIcons.Remove(selectedshipIcon);
                Destroy(selectedshipIcon.gameObject);
                selectedCraftInfo = null;
                selectedshipIcon = null;
                craftsScrollView.setInformation(craftInfos);
                IToast.instance.show("abandon successed");
            }
        }

        void onBackButtonClick()
        {
            Application.LoadLevel("Menu");
        }
    }

    public class CraftInfo
    {
        public string realName;
        public string shipName;
        public string assName;
        public Vector3 position;
        public Vector3 eulerAngles;
        public float tonnage;
        public float mass;
        public Vector3 massCenter;
        public float shipOffsetY;
        public bool isDel;

        public CraftInfo(JsonData shipData)
        {
            realName = IUtils.getJsonValue2String(shipData, "realName");
            shipName = IUtils.getJsonValue2String(shipData, "shipName");
            assName = IUtils.getJsonValue2String(shipData, "assName");
            tonnage = IUtils.getJsonValue2Float(shipData, "tonnage");
            mass = IUtils.getJsonValue2Float(shipData, "mass");
            massCenter = IUtils.getJsonValue2Vector3(shipData, "massCenter");
            shipOffsetY = IUtils.getJsonValue2Float(shipData, "shipOffsetY");
            position = IUtils.getJsonValue2Vector3(shipData, "pos");
            eulerAngles = IUtils.getJsonValue2Vector3(shipData, "rot");
            isDel = IUtils.getJsonValue2Bool(shipData, "isDel");
        }
    }
}
