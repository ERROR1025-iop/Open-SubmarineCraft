using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;
using Scraft.StationSpace;

namespace Scraft
{
    public class PoolerStationMenu : MonoBehaviour
    {
        static public PoolerStationMenu instance;
        BlocksEngine blocksEngine;
        RectTransform rectTrans;
        GameObject wharfButtonGameObject;

        int[] recoveryBlocks;

        void Start()
        {
            instance = this;
            rectTrans = GetComponent<RectTransform>();
            rectTrans.GetChild(1).GetComponent<Button>().onClick.AddListener(onRecoveryShipButtonClick);
            rectTrans.GetChild(2).GetComponent<Button>().onClick.AddListener(onRecoveryCargoButtonClick);
            rectTrans.GetChild(3).GetComponent<Button>().onClick.AddListener(onRecoveryPowerButtonClick);          
            rectTrans.GetChild(4).GetComponent<Button>().onClick.AddListener(onUpgradeStationButtonClick);
            rectTrans.GetChild(5).GetComponent<Button>().onClick.AddListener(onResumeGameButtonClick);
            rectTrans.GetChild(6).GetComponent<Button>().onClick.AddListener(onChargePowerButtonClick);
            rectTrans.GetChild(7).GetComponent<Button>().onClick.AddListener(onRecoveryScientificButtonClick);

            wharfButtonGameObject = GameObject.Find("Canvas/radar rect/radar text rect/wharf");

            blocksEngine = BlocksEngine.instance;
        }


        void onRecoveryShipButtonClick()
        {
            if (GameSetting.isCareer)
            {
                recoveryBlocks = Pooler.instance.getRecoveryShipBlocks();
                if (Station.satyStation.isCanAddCargos(recoveryBlocks))
                {
                    onRecoveryShipConfirmClick();
                }
                else
                {
                    IConfigBox.instance.show(ILang.get("Can't recycle all, do you recycle only part of the block?"), onRecoveryShipConfirmClick, null);
                }
            }
            else
            {
                IToast.instance.show("Free mode is not available", 100);
            }
        
        }

        void onRecoveryShipConfirmClick()
        {
            IToast.instance.show("Loading");
            Station.satyStation.setMainStation(true);
            Station.satyStation.addCargos(recoveryBlocks);
            Pooler.instance.savePoolerData(true);
            Application.LoadLevel("Menu");
        }

        void onRecoveryCargoButtonClick()
        {           
            foreach (IPoint coor in Pooler.instance.cargoCoors)
            {
                Block block = blocksEngine.getBlock(coor);
                if (block != null && !block.isNeedDelete() && block.isCanStoreInWarehouse() > 0)
                {
                    Station.satyStation.addCargos(block.getId(), 1);
                    blocksEngine.removeBlock(coor);
                }
            }
        }

        public void onRecoveryPowerButtonClick()
        {
            float canCharge = Station.satyStation.getCanChargePower();
            float receive = Pooler.instance.requireElectric(null, canCharge);
            Station.satyStation.addPower(receive);
        }

        public void onChargePowerButtonClick()
        {
            Station.satyStation.startChargePower();
        }

        public void onRecoveryScientificButtonClick()
        {
            float result = Pooler.instance.recoveryBlocksScientific();
            IToast.instance.showWithoutILang(string.Format("{0}({1}{2})", ILang.get("Successful recovery"), result.ToString("f1"), ILang.get("point")), 150);
        }

        void onUpgradeStationButtonClick()
        {
            StationUpgrade.instance.initialized(Station.satyStation);
            StationUpgrade.instance.setShow(true);
            show(false);
        }

        void onResumeGameButtonClick()
        {
            show(false);
        }

        public void show(bool show)
        {
            if (show)
            {
                World.stopUpdata = true;
                Time.timeScale = 0;
                Pooler.isGlobalShowIcon = false;
                rectTrans.anchoredPosition = new Vector2(0, 0);
            }
            else
            {
                World.stopUpdata = false;
                Time.timeScale = 1;
                Pooler.isGlobalShowIcon = true;
                rectTrans.anchoredPosition = new Vector2(1000, 0);
            }
        }

        private void Update()
        {
            wharfButtonGameObject.gameObject.SetActive(Station.isSatyInStation);
            Station.isSatyInStation = false;            
        }
    }    
}
