using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;
using LitJson;
using Scraft.StationSpace;

namespace Scraft
{
    public class Shop : MonoBehaviour
    {
        public IGridScrollView gridScrollView;
        public Button addCartButton;
        public Button decCartButton;
        public Button buyButton;
        public Text totalPriceText;
        public Text cartListText;

        TpHttpManager tpHttpManager1;
        TpHttpManager tpHttpManager2;
        List<ShopCellInfo> shopCellInfos;
        ShopCellInfo selectingShopCellInfo;

        BlocksManager blocksManager;
        StationInfo mainStationInfo;

        int[] cartList;
        float totalPrice;

        void Start()
        {
            tpHttpManager1 = new TpHttpManager(IConfigBox.instance, IToast.instance);
            tpHttpManager1.setListener(onGetCatalogueResponse);
            tpHttpManager1.setTpPost("ApiShop", "shopCatalogueCon");
            tpHttpManager1.addVersion();
            tpHttpManager1.send();

            gridScrollView.initialized(OnCreateShopCells);          
            gridScrollView.OnCellClick += onShopCellClick;

            shopCellInfos = new List<ShopCellInfo>();
            blocksManager = BlocksManager.instance == null ? new BlocksManager() : BlocksManager.instance;

            GameObject.Find("Canvas/Back").GetComponent<Button>().onClick.AddListener(onBackButtonClick);

            addCartButton.onClick.AddListener(onAddCartButtonClick);
            decCartButton.onClick.AddListener(onDecCartButtonClick);
            buyButton.onClick.AddListener(onBuyButtonClick);

            cartList = new int[blocksManager.getBlockCount()];

            IUtils.initializedArray(cartList, 0);
            totalPrice = 0;

            loadStationInfos();
        }

        public void loadStationInfos()
        {
            StationInfo[] stationInfos;
            int mainStationIndex = ISecretLoad.loadStationsInfo(out stationInfos);
            if (stationInfos != null)
            {
                mainStationInfo = stationInfos[mainStationIndex];
            }
        }

        void onAddCartButtonClick()
        {
            if(selectingShopCellInfo != null)
            {
                cartList[selectingShopCellInfo.block.getId()] += 1;
                totalPrice += selectingShopCellInfo.price;
                updateCartList();
            }
        }

        void onDecCartButtonClick()
        {
            if (selectingShopCellInfo != null)
            {                
                if(cartList[selectingShopCellInfo.block.getId()] > 0)
                {
                    cartList[selectingShopCellInfo.block.getId()] -= 1;
                    totalPrice -= selectingShopCellInfo.price;
                    if (totalPrice < 0)
                    {
                        totalPrice = 0;
                    }
                }                             
                updateCartList();
            }
        }

        void onBuyButtonClick()
        {
            if(totalPrice > 0)
            {
                if (ISecretLoad.getDiamonds() >= totalPrice)
                {
                    IConfigBox.instance.show(string.Format(ILang.get("confirm buy"), totalPrice.ToString("f0")), onBuyConfirmButtonClick, null);
                }
                else
                {
                    IToast.instance.show("Insufficient Diamonds");
                }
            }
            else
            {
                IToast.instance.show("Please add to the shopping cart first", 100);
            }
            
        }

        void onBuyConfirmButtonClick()
        {
            if (mainStationInfo != null)
            {
                ISecretLoad.setDiamonds(ISecretLoad.getDiamonds() - totalPrice);
                IScientificAndDiamonds.instance.UpdateNumberText();
                mainStationInfo.addCargos(cartList);
                mainStationInfo.saveCargos();
                sendBuyInfo();
                UnityAndroidEnter.CallStatisticsBuyPrice(totalPrice);
                IUtils.initializedArray(cartList, 0);
                totalPrice = 0;
                updateCartList();                
                IToast.instance.show("buy successed", 100);                
            }
            else
            {
                IToast.instance.show("no file");
            }
        }

        void sendBuyInfo()
        {
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();
            writer.WritePropertyName("buy");
            writer.WriteArrayStart();
            int count = cartList.Length;           
            for (int i = 0; i<count; i++)
            {
                if (cartList[i] > 0)
                {
                    writer.WriteObjectStart();
                    IUtils.keyValue2Writer(writer, "blockId", i);
                    IUtils.keyValue2Writer(writer, "count", cartList[i]);
                    writer.WriteObjectEnd();
                }
            }
            writer.WriteArrayEnd();
            writer.WriteObjectEnd();

            tpHttpManager2 = new TpHttpManager(null, null);
            tpHttpManager2.setTpPost("ApiShop", "shopBuyCon");
            tpHttpManager2.setListener(onBuyInfoResponsed);
            tpHttpManager2.addPostParam("buy", writer.ToString());
            tpHttpManager2.addVersion();
            tpHttpManager2.addToken();
            tpHttpManager2.send();
        }

        void onBuyInfoResponsed()
        {
            //IToast.instance.show(tpHttpManager2.result);
        }

        void updateCartList()
        {
            totalPriceText.text = totalPrice.ToString("f0");
            int count = cartList.Length;
            string listStr = "";
            for (int i = 0; i < count; i++)
            {
                if(cartList[i] > 0)
                {
                    listStr += string.Format("{0}X{1}   ", blocksManager.getBlockById(i).getLangName(), cartList[i]);
                }
            }
            cartListText.text = listStr;
        }

        IGridScrollViewCell OnCreateShopCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new ShopCell(gridScrollViewInfo);
        }

        void onShopCellClick(IGridScrollViewCell cell)
        {
            selectingShopCellInfo = (cell as ShopCell).getShopCellInfo();
        }

        void onGetCatalogueResponse()
        {
            JsonData jsonData;
            if (tpHttpManager1.result.Contains("msg"))
            {
                jsonData = JsonMapper.ToObject(tpHttpManager1.result);
                IToast.instance.show(IUtils.getJsonValue2String(jsonData, "msg"), 100);
                return;
            }
            jsonData = JsonMapper.ToObject(tpHttpManager1.result);
            int count = jsonData.Count;
            for(int i=0;i<count; i++)
            {
                JsonData blockData = jsonData[i];
                Block block = blocksManager.getBlockById(IUtils.getJsonValue2Int(blockData, "blockId"));
                float price = IUtils.getJsonValue2Float(blockData, "price");
                ShopCellInfo shopCellInfo = new ShopCellInfo(block, price);
                shopCellInfos.Add(shopCellInfo);
            }
            if(count > 0)
            {
                gridScrollView.setInformation(shopCellInfos);
            }            
        }

        void Update()
        {
            tpHttpManager1.updata();
            //tpHttpManager2.updata();
        }

        void onBackButtonClick()
        {
            Application.LoadLevel("Menu");
        }
    }

}