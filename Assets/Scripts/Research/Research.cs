using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;

namespace Scraft
{
    public class Research : MonoBehaviour
    {
        static public Research instance;
        public Transform cellParent;

        public Image InformationPointsIconImage;
        public Image InformationPointsIconSpriteImage;
        public Image informationResearchButtonImage;
        public Text informationResearchButtonValueText;
        public Text InformationPointsNameText;
        public RectTransform informationRectTransform;
        public Text blockInformationText;

        static int MAX_Drawer_COUNT = 10;
        ResearchDrawer[] drawersArray;
        float informationRectTransformWith;

        Sprite researchButtonSprite1;
        Sprite researchButtonSprite2;

        List<ResearchPoint> researchPoints;
        ResearchPoint selectingResearchPoint;
        BlocksManager bm;


        ResearchPoint smallShips;
        ResearchPoint primarySubmersible;
        ResearchPoint miningTechnology;
        ResearchPoint powerGenerationTechnology;
        ResearchPoint motorPreliminary;
        ResearchPoint largeMotor;
        ResearchPoint smeltingPreliminary;
        ResearchPoint refinementPreliminary;
        ResearchPoint smeltingAutomationTechnology;
        ResearchPoint centrifugalTechnology;
        ResearchPoint advancedMachinery;
        ResearchPoint deepExploration;
        ResearchPoint shipFoundation;
        ResearchPoint materialScience;
        ResearchPoint UltraAbyssExploration;
        ResearchPoint circuitPreliminary;
        ResearchPoint advancedShipTechnology;
        ResearchPoint deepSeaVoyage;
        ResearchPoint nuclearEnergy;
        ResearchPoint sensorTechnology;
        ResearchPoint heightCircuitTechnology;
        ResearchPoint advancedCircuitTechnology;
        ResearchPoint armedTechnology;
        ResearchPoint detectionTechnology;
        ResearchPoint electrolysisTechnology;
        ResearchPoint highTemperatureSmelting;

        public void registerResearchPoints()
        {
            smallShips = registerResearchPoint("smallShips", 0, null, new Block[] { bm.steamEngine, bm.wood, bm.coal, bm.smallPropeller, bm.shaft, bm.wireless });
            miningTechnology = registerResearchPoint("miningTechnology", 5, smallShips, new Block[] { bm.drillCore, bm.gearSet, bm.woodCargohold });
            powerGenerationTechnology = registerResearchPoint("powerGenerationTechnology", 15, smallShips, new Block[] { bm.generator, bm.battery, bm.copper, bm.winding });
            primarySubmersible = registerResearchPoint("primarySubmersible", 5, smallShips, new Block[] { bm.pump, bm.valve, bm.steel, bm.cargohold });            
            smeltingPreliminary = registerResearchPoint("smeltingPreliminary", 25, miningTechnology, new Block[] { bm.stoneFurnace, bm.sulfur, bm.sulphuricAcid, bm.sulfurDioxide, bm.fineSteel });
           
            shipFoundation = registerResearchPoint("shipFoundation", 110, primarySubmersible, new Block[] { bm.dieselEngine, bm.dieselGenerator, bm.diesel, bm.propeller, bm.sidePropeller });
            deepExploration = registerResearchPoint("deepExploration", 310, primarySubmersible, new Block[] { bm.advPump, bm.terrainSonar, bm.searchlight, bm.thermometer });

            motorPreliminary = registerResearchPoint("motorPreliminary", 75, powerGenerationTechnology, new Block[] { bm.smallElectorEngine });

            refinementPreliminary = registerResearchPoint("refinementPreliminary", 150, smeltingPreliminary, new Block[] { bm.grinder, bm.coarseIronOre, bm.coarseCopperOre, bm.coarseSulfurOre, bm.dinas });
            materialScience = registerResearchPoint("materialScience", 75, smeltingPreliminary, new Block[] { bm.silicon, bm.siliconCarbide, bm.charcoal, bm.semiconductor, bm.circuitBoard, bm.asphalt , bm.coalDiansMixture});

            advancedMachinery = registerResearchPoint("advancedMachinery", 355, refinementPreliminary, new Block[] { bm.ironFurnace, bm.electorHeater });            
            highTemperatureSmelting = registerResearchPoint("highTemperatureSmelting", 750, advancedMachinery, new Block[] { bm.steelFurnace, bm.temperatureInfraredSensor, bm.advElectorHeater});
            centrifugalTechnology = registerResearchPoint("centrifugalTechnology", 1155, highTemperatureSmelting, new Block[] { bm.centrifuge, bm.fineIronOre, bm.fineCopperOre, bm.fineSulfurOre, bm.ironPowder, bm.copperPowder, bm.sulfurPowder, bm.uraniumPowder, bm.coalPowder, bm.sulfurPowder });
            electrolysisTechnology = registerResearchPoint("electrolysisTechnology", 1155, advancedMachinery, new Block[] { bm.electrolyser, bm.refrigerator,  bm.hydrogen, bm.chlorine, bm.distilledWater });
            smeltingAutomationTechnology = registerResearchPoint("smeltingAutomationTechnology", 355, refinementPreliminary, new Block[] { bm.conveyor, bm.conveyorCorner, bm.transfer, bm.crane, bm.pushCrane , bm.machineBattery, bm.goodsSensor});
          
           
            UltraAbyssExploration = registerResearchPoint("UltraAbyssExploration", 750, deepExploration, new Block[] { bm.advTerrainSonar, bm.sampleCollector });
            circuitPreliminary = registerResearchPoint("circuitPreliminary", 275, materialScience, new Block[] { bm.cable, bm.comparatorGate, bm.switchGate, bm.signalButton, bm.signalLamp, bm.refailButton, bm.logicGate, bm.andGate, bm.orGate, bm.numericalDisplay });
            largeMotor = registerResearchPoint("largeMotor", 750, shipFoundation, new Block[] { bm.electorEngine });
            advancedShipTechnology = registerResearchPoint("advancedShipTechnology", 1250, largeMotor, new Block[] { bm.steamTurbine, bm.auxiliaryGenerator });
           
            deepSeaVoyage = registerResearchPoint("deepSeaVoyage", 750, deepExploration, new Block[] { bm.stirlingEngine, bm.compressedAir});
            heightCircuitTechnology = registerResearchPoint("heightCircuitTechnology", 550, circuitPreliminary, new Block[] { bm.addGate, bm.decGate, bm.mulGate, bm.divGate, bm.remainderGate });
            advancedCircuitTechnology = registerResearchPoint("advancedCircuitTechnology", 950, heightCircuitTechnology, new Block[] { bm.functionGenerator });
            sensorTechnology = registerResearchPoint("sensorTechnology", 750, heightCircuitTechnology, new Block[] { bm.temperatureSensor, bm.pressSensor, bm.shaftSensor, bm.advGoodsSensor });
            nuclearEnergy = registerResearchPoint("nuclearEnergy", 1750, sensorTechnology, new Block[] { bm.uranium, bm.controlRod, bm.alarm });
            
          
            armedTechnology = registerResearchPoint("armedTechnology", 1550, electrolysisTechnology, new Block[] { bm.torpedp, bm.smallTorpedp, bm.torpedpTube,bm.turretCore, bm.turretShell });
            detectionTechnology = registerResearchPoint("detectionTechnology", 2750, armedTechnology, new Block[] { bm.advSonar, bm.predictor});
        }    

        void Start()
        {
            instance = this;
            bm = new BlocksManager();
            ISecretLoad.init();
            researchPoints = new List<ResearchPoint>();

            GameObject.Find("Canvas/Back").GetComponent<Button>().onClick.AddListener(onBackButtonClick);
            informationResearchButtonImage.GetComponent<Button>().onClick.AddListener(onResearchButtonClick);

            researchButtonSprite1 = Resources.Load("Menu/Research/button6", typeof(Sprite)) as Sprite;
            researchButtonSprite2 = Resources.Load("Menu/Research/button7", typeof(Sprite)) as Sprite;

            drawersArray = new ResearchDrawer[MAX_Drawer_COUNT];
            informationRectTransformWith = informationRectTransform.sizeDelta.x;
            for (int i = 0; i < MAX_Drawer_COUNT; i++)
            {
                drawersArray[i] = new ResearchDrawer(i, informationRectTransform);
            }

            registerResearchPoints();
            initializedReserchPoints();
            onResearchPointCellClick(researchPoints[0]);
        }


        public ResearchPoint registerResearchPoint(string name, float unlockSci, ResearchPoint root, Block[] unlockBlocks)
        {
            ResearchPoint researchPoint = new ResearchPoint(name, unlockSci, unlockBlocks, root, cellParent);
            researchPoints.Add(researchPoint);
            return researchPoint;
        }

        void initializedReserchPoints()
        {
            int[] unlockData = bm.getUnlockData();
            if (unlockData != null)
            {
                int count = unlockData.Length;
                foreach (var rp in researchPoints)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (rp.unlockBlocks[0].getId() == unlockData[i])
                        {
                            rp.initialized(true);
                            break;
                        }
                        rp.initialized(false);
                    }
                }
            }
            else
            {
                foreach (var rp in researchPoints)
                {
                    rp.initialized(false);
                }
            }

        }

        void onResearchButtonClick()
        {
            if (selectingResearchPoint != null && selectingResearchPoint.canUnlock())
            {
                float scientific = ISecretLoad.getScientific();
                float consume = selectingResearchPoint.getUnlockConsume();
                if (scientific >= consume)
                {
                    selectingResearchPoint.setUnlock();
                    ISecretLoad.saveResearch(researchPoints);
                    ISecretLoad.loadResearch();
                    ISecretLoad.setScientific(scientific - consume);
                    IScientificAndDiamonds.instance.UpdateNumberText();
                    informationResearchButtonImage.sprite = researchButtonSprite2;
                }
                else
                {
                    IToast.instance.show("Insufficient scientific points", 100);
                }
            }
        }

        public void onResearchPointCellClick(ResearchPoint researchPoint)
        {
            selectingResearchPoint = researchPoint;
            InformationPointsIconImage.sprite = researchPoint.cellImage.sprite;
            InformationPointsIconSpriteImage.sprite = researchPoint.iconImage.sprite;
            InformationPointsNameText.text = ILang.get(researchPoint.name, "research");
            informationResearchButtonValueText.text = researchPoint.getUnlockConsume().ToString("f0");
            informationResearchButtonImage.sprite = researchPoint.canUnlock() ? researchButtonSprite1 : researchButtonSprite2;

            int count = researchPoint.unlockBlocks.Length;
            informationRectTransform.sizeDelta = new Vector2(informationRectTransformWith, count * 31f);
            for (int i = 0; i < MAX_Drawer_COUNT; i++)
            {
                if (i < count)
                {
                    drawersArray[i].setInformation(researchPoint.unlockBlocks[i]);
                }
                else
                {
                    drawersArray[i].clearInformation();
                }
            }
        }

        public void onResearchInformationPointCellClick(Block block)
        {
            blockInformationText.text = block.getBasicInformation();
        }

        void onBackButtonClick()
        {
            Application.LoadLevel("Menu");
        }
    }
}
