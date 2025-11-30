using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.StationSpace;
using Scraft.BlockSpace;

namespace Scraft {
    public class Stations : MonoBehaviour
    {

        static public Stations instance;

        public RectTransform rectTransform;
        public IGridScrollView stationsScrollView;
        public Image thumbnailImage;
        public IGridScrollView solidScrollView;
        public IGridScrollView liquildScrollView;
        public IProgressbar soildProgressbar;
        public IProgressbar liquildProgressbar;
        public IProgressbar powerProgressbar;
        public Text stationNameText;
        public Text blockNameText;
        public Button switchButton;
        public Button syntButton;
        public Button del10Button;
        public Button del100Button;
        public Button del1000Button;
        public Text syncButtonText;        
        public IGridScrollView syntClassScrollView;
        public IGridScrollView syntScrollView;
        public Button syntOneButton;
        public Button synt64Button;
        public Text syntOneText;
        public Text syntClassNameText;
        public Text syntMaterialsText;
        public TMPro.TextMeshProUGUI syntBlockInfoText;

        BlocksManager blocksManager;

        List<StationInfo> stationInfos;
        StationInfo selectedStationInfo;
        int mainStationIndex;

        List<BlockCellInfo> solidBlockInfos;
        List<BlockCellInfo> liquildBlockInfos;
        List<SyntClass> syntClasses;
        CardsRegister cardsRegister;
        int selectBlockId;

        bool isShowSnyt;

        void Start()
        {
            instance = this;
            GameObject.Find("Canvas/Back").GetComponent<Button>().onClick.AddListener(onBackButtonClick);

            if(BlocksManager.instance == null)
            {
                blocksManager = new BlocksManager();                
            }
            else
            {
                blocksManager = BlocksManager.instance;
            }
            blocksManager.loadUnlockData();

            ISecretLoad.init();
            loadStationInfos();

            if (stationInfos != null)
            {
                stationsScrollView.initialized(OnCreateStationListCells);
                stationsScrollView.setInformation(stationInfos);
                stationsScrollView.OnCellClick += onStationListCellClick;
            }

            //初始化固体仓库列表
            solidScrollView.initialized(OnCreateSoildCells);
            solidScrollView.OnCellClick += onStoreCellClick;
            solidBlockInfos = new List<BlockCellInfo>();
            //初始化液体仓库列表
            liquildScrollView.initialized(OnCreateLiquildCells);
            liquildScrollView.OnCellClick += onStoreCellClick;
            liquildBlockInfos = new List<BlockCellInfo>();
            //初始化合成列表
            syntClassScrollView.initialized(OnCreateSyntClassCells);
            syntClassScrollView.OnCellClick += onSyntClassCellClick;
            syntScrollView.initialized(OnCreateSyntCells);
            syntScrollView.OnCellClick += onSyntCellClick;
            initializedSynt();
            syntClassScrollView.setInformation(syntClasses);

            if (stationInfos != null)
            {
                stationsScrollView.setCellActivity(0);
            }

            switchButton.onClick.AddListener(onSwitchButtonClick);
            syntButton.onClick.AddListener(onSyntButtonClick);
            syntOneButton.onClick.AddListener(onSyntOneButtonClick);
            synt64Button.onClick.AddListener(onSynt64ButtonClick);
            del10Button.onClick.AddListener(onBlockDel10ButtonClick);
            del100Button.onClick.AddListener(onBlockDel100ButtonClick);
            del1000Button.onClick.AddListener(onBlockDel1000ButtonClick);
        }

        void initializedSynt()
        {
            syntClasses = new List<SyntClass>();
            cardsRegister = CardsRegister.get();
            List<CardInfo> cardInfos = cardsRegister.cardInfos;
            foreach (CardInfo cardInfo in cardInfos)
            {
                SyntClass syntClass = new SyntClass(cardInfo);
                syntClasses.Add(syntClass);
            }

            for (int i = 0; i < BlocksManager.MAX_BLOCK_ID; i++)
            {
                Block block = blocksManager.getBlockById(i);
                if (block != null && block.getAttributeCardName() != "null" && block.getSyntInfo(blocksManager) != null)
                {
                    if (!GameSetting.isCareer || blocksManager.getIsUnlock(i))
                    {
                        SyntClass syntClass = SyntClass.getSyntClassByName(syntClasses, block.getAttributeCardName());
                        if (syntClass != null)
                        {
                            syntClass.addBlock(block);
                        }
                    }
                }
            }
        }

        IGridScrollViewCell OnCreateSyntClassCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new SyntClassCell(gridScrollViewInfo);
        }

        IGridScrollViewCell OnCreateSyntCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new SyntCell(gridScrollViewInfo);
        }

        void onSyntClassCellClick(IGridScrollViewCell cell)
        {
            SyntClassCell syntClassCell = cell as SyntClassCell;
            syntScrollView.setInformation(syntClassCell.syntClass.blocks);
            syntClassNameText.text = ILang.get(syntClassCell.syntClass.name, "card");
            //syntScrollView.setCellActivity(0);
        }

        void onSyntCellClick(IGridScrollViewCell cell)
        {
            SyntCell syntCell = cell as SyntCell;
            syntBlockInfoText.text = syntCell.block.getBasicInformation();

            syntOneText.text = string.Format(ILang.get("Synt {0}"), syntCell.syntInfos[0].produeNumber);
            syntMaterialsText.text = IUtils.getSyntString(syntCell.syntInfos, "\n");
        }

        void onSwitchButtonClick()
        {
            if(selectedStationInfo != null)
            {
                mainStationIndex = selectedStationInfo.id;
                ISecretLoad.saveMainStation(mainStationIndex);
                stationsScrollView.setInformation(stationInfos);
            }

        }

        void onSyntButtonClick()
        {
            setShowSync(!isShowSnyt);
        }

        void onSyntOneButtonClick()
        {
            SyntCell syntCell = syntScrollView.activiteCell as SyntCell;
            if (syntCell != null && syntCell.syntInfos != null && selectedStationInfo != null)
            {
                SyntInfo syntInfo = syntCell.syntInfos[0];
                if (selectedStationInfo.IsContainCargo(syntInfo.syntData))
                {
                    if (selectedStationInfo.isCanAddCargos(syntInfo.produeBlockStatic.getId(), syntInfo.produeNumber) == syntInfo.produeNumber)
                    {
                        selectedStationInfo.removeCargos(syntInfo.syntData);
                        selectedStationInfo.addCargos(syntInfo.produeBlockStatic.getId(), syntInfo.produeNumber);


                        initializedStoreCargoCount();
                        initializedStoreCargoCell(selectedStationInfo.cargoCounts);
                    }
                }
                else
                {
                    IToast.instance.show("Not enough materials", 100);
                }
            }
        }

        void onSynt64ButtonClick()
        {
            int produeCount = 0;
            SyntCell syntCell = syntScrollView.activiteCell as SyntCell;
            if (syntCell != null && syntCell.syntInfos != null && selectedStationInfo != null)
            {
                SyntInfo syntInfo = syntCell.syntInfos[0];
                for (int i = 0; i < 64; i++)
                {
                    if (selectedStationInfo.IsContainCargo(syntInfo.syntData))
                    {
                        if (selectedStationInfo.isCanAddCargos(syntInfo.produeBlockStatic.getId(), syntInfo.produeNumber) == syntInfo.produeNumber)
                        {
                            selectedStationInfo.removeCargos(syntInfo.syntData);
                            selectedStationInfo.addCargos(syntInfo.produeBlockStatic.getId(), syntInfo.produeNumber);
                            produeCount += syntInfo.produeNumber;

                            initializedStoreCargoCount();
                            initializedStoreCargoCell(selectedStationInfo.cargoCounts);
                        }
                    }
                    else
                    {
                        if(produeCount > 0)
                        {
                            IToast.instance.show("Insufficient materials, only partially synthesized", 100);
                            break;
                        }
                        else
                        {
                            IToast.instance.show("Not enough materials", 100);
                        }                        
                    }

                    if (produeCount >= 64)
                    {
                        break;
                    }                    
                }
            }
        }

        void onBlockDel10ButtonClick()
        {
            delBlock(10);
        }

        void onBlockDel100ButtonClick()
        {
            delBlock(100);
        }

        void onBlockDel1000ButtonClick()
        {
            delBlock(1000);
        }

        void delBlock(int count)
        {
            if (selectBlockId > 0 && selectedStationInfo != null)
            {
                selectedStationInfo.removeCargos(selectBlockId, count);
                initializedStoreCargoCount();
                initializedStoreCargoCell(selectedStationInfo.cargoCounts);
            }
        }



        void onStoreCellClick(IGridScrollViewCell cell)
        {
            StationBlockCell stationBlockCell = cell as StationBlockCell;
            blockNameText.text = stationBlockCell.getBlock().getLangName();
            selectBlockId = stationBlockCell.getBlock().getId();
        }

        IGridScrollViewCell OnCreateSoildCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new StationBlockCell(gridScrollViewInfo);
        }

        IGridScrollViewCell OnCreateLiquildCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new StationBlockCell(gridScrollViewInfo);
        }

        IGridScrollViewCell OnCreateStationListCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new StationsListCell(gridScrollViewInfo);
        }

        void onStationListCellClick(IGridScrollViewCell cell)
        {
            StationsListCell stationsListCell = cell as StationsListCell;
            selectedStationInfo = stationsListCell.getStationInfo();     
           
            //获取缩略图
            thumbnailImage.sprite = selectedStationInfo.getThumbnailSprite();
            thumbnailImage.color = thumbnailImage.sprite == null ? Color.black : Color.white;
            //初始化站名
            stationNameText.text = selectedStationInfo.name;            
            //初始化仓库存量
            soildProgressbar.setValue(selectedStationInfo.getStoreSoildCount(), selectedStationInfo.canStoreSoild);
            liquildProgressbar.setValue(selectedStationInfo.getStoreLiquidCount(), selectedStationInfo.canStoreLiquid);
            powerProgressbar.setValue(selectedStationInfo.getStorePowerCount(), selectedStationInfo.canStorePower);
            initializedStoreCargoCell(selectedStationInfo.cargoCounts);
        }

        void initializedStoreCargoCount()
        {
            soildProgressbar.setValue(selectedStationInfo.getStoreSoildCount(), selectedStationInfo.canStoreSoild);
            liquildProgressbar.setValue(selectedStationInfo.getStoreLiquidCount(), selectedStationInfo.canStoreLiquid);
            powerProgressbar.setValue(selectedStationInfo.getStorePowerCount(), selectedStationInfo.canStorePower);
        }

        void initializedStoreCargoCell(int[] cargosCount)
        {
            solidBlockInfos.Clear();
            liquildBlockInfos.Clear();
            int count = cargosCount.Length;
            for (int i = 0; i < count; i++)
            {
                if (cargosCount[i] > 0)
                {
                    Block block = blocksManager.getBlockById(i);
                    if (block.isCanStoreInWarehouse() == 1)
                    {
                        solidBlockInfos.Add(new BlockCellInfo(block, cargosCount[i]));
                    }
                    else if (block.isCanStoreInWarehouse() == 2)
                    {
                        liquildBlockInfos.Add(new BlockCellInfo(block, cargosCount[i]));
                    }
                }
            }

            solidScrollView.setInformation(solidBlockInfos);
            liquildScrollView.setInformation(liquildBlockInfos);
        }

        void loadStationInfos()
        {
            StationInfo[] l_stationInfos;
            mainStationIndex = ISecretLoad.loadStationsInfo(out l_stationInfos);
            if(l_stationInfos != null)
            {
                stationInfos = new List<StationInfo>(l_stationInfos);
            }     
        }

        public void setShowSync(bool show)
        {
            isShowSnyt = show;
            if (show)
            {
                rectTransform.anchoredPosition = new Vector2(-278f, -35.59199f);
                syncButtonText.text = ILang.get("Back");
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(178.5499f, -35.59199f);
                syncButtonText.text = ILang.get("Synt");
            }
        }

        void onBackButtonClick()
        {
            if(stationInfos != null)
            {
                foreach(StationInfo stationInfo in stationInfos)
                {
                    stationInfo.saveCargos();
                }
            }
            Application.LoadLevel("Menu");
        }
    }
}
