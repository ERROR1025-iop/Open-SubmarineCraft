using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.StationSpace;
using Scraft.BlockSpace;

namespace Scraft
{    
    public class StationUpgrade : MonoBehaviour
    {
        static public StationUpgrade instance;
        RectTransform rectTransform;
        BlocksManager blocksManager;

        public Image thumbnailImage;
        public InputField nameInputField;
        public IChangeImageButton modifyNameButton;
        public IGridScrollView upgradeScrollView;
        public IGridScrollView solidScrollView;
        public IGridScrollView liquildScrollView;
        public StationsManager stationsManager;
        public Button backButton;
        public Button buildButton;
        public Button syntButton;
        public Button delButton;
        public Text syncButtonText;
        public IProgressbar soildProgressbar;
        public IProgressbar liquildProgressbar;
        public IProgressbar powerProgressbar;
        public Text blockNameText;
        public IGridScrollView syntClassScrollView;
        public IGridScrollView syntScrollView;
        public Button syntOneButton;
        public Button synt64Button;
        public Text syntOneText;
        public Text syntClassNameText;
        public Text syntMaterialsText;
        public Text syntBlockInfoText;

        Station station;
        StationInfo stationInfo;
        CardsRegister cardsRegister;        
        List<SyntClass> syntClasses;

        List<BlockCellInfo> solidBlockInfos;
        List<BlockCellInfo> liquildBlockInfos;

        int selectBlockId;

        bool isShowSnyt;
        

        ComponentInfo selectingComponentInfo;

        private void Awake()
        {
            stationsManager = new StationsManager();            
        }

        void Start()
        {
            instance = this;
            blocksManager = BlocksManager.instance;
            rectTransform = GetComponent<RectTransform>();
            cardsRegister = CardsRegister.get();            

            backButton.onClick.AddListener(onBackButtonClick);
            buildButton.onClick.AddListener(onBuildButtonClick);            
            syntButton.onClick.AddListener(onSyntButtonClick);
            syntOneButton.onClick.AddListener(onSyntOneButtonClick);
            synt64Button.onClick.AddListener(onSynt64ButtonClick);
            delButton.onClick.AddListener(onBlockDelButtonClick); 

            //初始化升级组件列表
            upgradeScrollView.initialized(OnCreateUpgradeCells);
            upgradeScrollView.setInformation(stationsManager.componentInfos);
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
        }

        public void initialized(Station station)
        {
            this.station = station;
            //获取缩略图
            thumbnailImage.sprite = station.getThumbnailSprite();
            //初始化站名
            nameInputField.text = station.name;
            nameInputField.readOnly = true;
            modifyNameButton.addListener(onModifyNameButtonClick);
            //初始化仓库存量
            initializedStoreCargoCount();
            initializedStoreCargoCell(station.cargoCounts);
        }

        public void initialized(StationInfo stationInfo)
        {
            this.stationInfo = stationInfo;
            //获取缩略图
            thumbnailImage.sprite = stationInfo.getThumbnailSprite();
            thumbnailImage.color = thumbnailImage.sprite == null ?  Color.black : Color.white;  
            //初始化站名
            nameInputField.text = stationInfo.name;
            nameInputField.readOnly = true;
            modifyNameButton.addListener(onModifyNameButtonClick);
            modifyNameButton.gameObject.SetActive(false);
            //初始化仓库存量
            soildProgressbar.setValue(stationInfo.getStoreSoildCount(), stationInfo.canStoreSoild);
            liquildProgressbar.setValue(stationInfo.getStoreLiquidCount(), stationInfo.canStoreLiquid);
            powerProgressbar.setValue(stationInfo.getStorePowerCount(), stationInfo.canStorePower);
            initializedStoreCargoCell(stationInfo.cargoCounts);
            buildButton.gameObject.SetActive(false);
            syntButton.gameObject.SetActive(false);
            delButton.gameObject.SetActive(false);
        }

        void initializedStoreCargoCount()
        {
            soildProgressbar.setValue(station.getStoreSoildCount(), station.canStoreSoild);
            liquildProgressbar.setValue(station.getStoreLiquidCount(), station.canStoreLiquid);
            powerProgressbar.setValue(station.getStorePowerCount(), station.canStorePower);
        }

        void initializedStoreCargoCell(int[] cargosCount)
        {
            solidBlockInfos.Clear();
            liquildBlockInfos.Clear();
            int count = cargosCount.Length;
            for (int i=0; i< count; i++)
            {
                if (cargosCount[i] > 0)
                {
                    Block block = blocksManager.getBlockById(i);
                    if(block.isCanStoreInWarehouse() == 1)
                    {
                        solidBlockInfos.Add(new BlockCellInfo(block, cargosCount[i]));
                    }else if (block.isCanStoreInWarehouse() == 2)
                    {
                        liquildBlockInfos.Add(new BlockCellInfo(block, cargosCount[i]));
                    }
                }
            }

            solidScrollView.setInformation(solidBlockInfos);
            liquildScrollView.setInformation(liquildBlockInfos);
        }

        void initializedSynt()
        {            
            syntClasses = new List<SyntClass>();
            List<CardInfo> cardInfos = cardsRegister.cardInfos;
            foreach (CardInfo cardInfo in cardInfos)
            {
                if (!cardInfo.isUnlock)
                {
                    SyntClass syntClass = new SyntClass(cardInfo);
                    syntClasses.Add(syntClass);
                }              
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

        void onSyntOneButtonClick()
        {
            SyntCell syntCell = syntScrollView.activiteCell as SyntCell;
            if (syntCell != null && syntCell.syntInfos != null)
            {
                SyntInfo syntInfo = syntCell.syntInfos[0];
                if (station.IsContainCargo(syntInfo.syntData))
                {
                    if (station.isCanAddCargos(syntInfo.produeBlockStatic.getId(), syntInfo.produeNumber) == syntInfo.produeNumber)
                    {
                        station.removeCargos(syntInfo.syntData);
                        station.addCargos(syntInfo.produeBlockStatic.getId(), syntInfo.produeNumber);
                        

                        initializedStoreCargoCount();
                        initializedStoreCargoCell(station.cargoCounts);
                    }
                }
            }
        }

        void onSynt64ButtonClick()
        {
            int produeCount = 0;
            SyntCell syntCell = syntScrollView.activiteCell as SyntCell;
            if (syntCell != null && syntCell.syntInfos != null)
            {
                SyntInfo syntInfo = syntCell.syntInfos[0];
                for (int i = 0; i < 64; i++)
                {
                    if (station.IsContainCargo(syntInfo.syntData))
                    {
                        if (station.isCanAddCargos(syntInfo.produeBlockStatic.getId(), syntInfo.produeNumber) == syntInfo.produeNumber)
                        {
                            station.removeCargos(syntInfo.syntData);
                            station.addCargos(syntInfo.produeBlockStatic.getId(), syntInfo.produeNumber);
                            produeCount += syntInfo.produeNumber;

                            initializedStoreCargoCount();
                            initializedStoreCargoCell(station.cargoCounts);
                        }
                    }
                    else
                    {
                        break;
                    }

                    if(produeCount >= 64)
                    {
                        break;
                    }
                }
            }
        }

        void onStoreCellClick(IGridScrollViewCell cell)
        {
            StationBlockCell stationBlockCell = cell as StationBlockCell;
            blockNameText.text = stationBlockCell.getBlock().getLangName();
            selectBlockId = stationBlockCell.getBlock().getId();
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

            syntMaterialsText.text = IUtils.getSyntString(syntCell.syntInfos, "\n");
            syntOneText.text = string.Format(ILang.get("Synt {0}"), syntCell.syntInfos[0].produeNumber);
        }

        IGridScrollViewCell OnCreateUpgradeCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new StationUpgradeCell(gridScrollViewInfo);
        }

        IGridScrollViewCell OnCreateSoildCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new StationBlockCell(gridScrollViewInfo);
        }

        IGridScrollViewCell OnCreateLiquildCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new StationBlockCell(gridScrollViewInfo);
        }

        IGridScrollViewCell OnCreateSyntClassCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new SyntClassCell(gridScrollViewInfo);
        }

        IGridScrollViewCell OnCreateSyntCells(IGridScrollViewInfo gridScrollViewInfo)
        {
            return new SyntCell(gridScrollViewInfo);
        }

        void onBlockDelButtonClick()
        {
            if(selectBlockId > 0 && station != null)
            {
                station.removeCargos(selectBlockId, 10);
                initializedStoreCargoCount();
                initializedStoreCargoCell(station.cargoCounts);
            }
        }

        void onBackButtonClick()
        {
            setShow(false);
        }        

        void onBuildButtonClick()
        {
            StationUpgradeCell cell = upgradeScrollView.activiteCell as StationUpgradeCell;
            if(cell != null)
            {
                if (station.IsContainCargo(cell.componentInfo.synt) || !GameSetting.isCareer)
                {
                    StationComponent stationComponent = (Instantiate(Resources.Load("Stations/Prefabs/" + cell.componentInfo.name)) as GameObject).GetComponent<StationComponent>();
                    stationComponent.initialized(cell.componentInfo);
                    selectingComponentInfo = cell.componentInfo;

                    Place3DItem place3DItem = stationComponent.gameObject.AddComponent<Place3DItem>();
                    place3DItem.setStation(station);
                    place3DItem.OnPlaceFinish += OnComponentPlaceFinish;
                }
                else
                {
                    IToast.instance.show("Inadequate blocks.", 100);
                }               

                setShow(false);
            }
        }

        void OnComponentPlaceFinish(Station station)
        {
            if (GameSetting.isCareer)
            {
                station.removeCargos(selectingComponentInfo.synt);
            }            
        }

        void onSyntButtonClick()
        {
            setShowSync(!isShowSnyt);
        }

        void onModifyNameButtonClick()
        {
            if (modifyNameButton.value)
            {
                nameInputField.readOnly = false;
                nameInputField.Select();
            }
            else
            {
                nameInputField.readOnly = true;
                station.name = nameInputField.text;
            }
        }

        public void setShow(bool show)
        {
            if (show)
            {           
                rectTransform.anchoredPosition = Vector2.zero;              
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(9999, 9999);
            }
            isShowSnyt = false;
        }

        public void setShowSync(bool show)
        {
            isShowSnyt = show; 
            if (show)
            {
                rectTransform.anchoredPosition = new Vector2(-470f, 0);
                syncButtonText.text = ILang.get("Back"); 
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(0, 0);
                syncButtonText.text = ILang.get("Synt");
            }
        }

    }
}
