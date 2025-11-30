using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.StationSpace;
using LitJson;

namespace Scraft.BlockSpace
{
    public class BlocksManager
    {

        public static BlocksManager instance;

        public const int MAX_BLOCK_ID = 5000;
        int idStack;
        int blockCount;
        int csIdStack;
        bool isRegisterEnd;
        Block[] blocksArr;
        bool[] unlockArr;
        int[] unlockData;
        /// <summary>
        /// 0:不可存储;1:存于固体;2:存于液体
        /// </summary>
        int[] canStoreIn;
        int[] mainStationCargoCounts;
        int[] consumeCargoCounts;
        StationInfo[] stationInfo;
        int mainStationIndex;


        public Block air;
        public Block border;
        public Block steel;
        public Block water;
        public Block waterMushy;
        public Block waterGas;
        public Block propeller;
        public Block pump;
        public Block steelLiquid;
        public Block steelMushy;
        public Block diesel;
        public Block dieselMushy;
        public Block dieselGas;
        public Block fire;
        public Block dieselEngine;
        public Block shaft;
        public Block torpedp;
        public Block torpedpTube;
        public Block battery;
        public Block generator;
        public Block electorEngine;
        public Block stirlingEngine;
        public Block neutron;
        public Block uranium;
        public Block uraniumLiquid;
        public Block uraniumMushy;
        public Block controlRod;
        public Block crane;
        public Block temperatureSensor;
        public Block numericalDisplay;
        public Block cable;
        public Block signalLamp;
        public Block signalButton;
        public Block refailButton;
        public Block wifi;
        public Block logicGate;
        public Block orGate;
        public Block andGate;
        public Block pressSensor;
        public Block copper;
        public Block siliconCarbide;
        public Block copperLiquid;
        public Block copperMushy;
        public Block ice;
        public Block steelAsh;
        public Block wireless;
        public Block advPump;
        public Block searchlight;
        public Block valve;
        public Block sidePropeller;
        public Block cooler;
        public Block smallTorpedp;
        public Block comparatorGate;
        public Block pressurizer;
        public Block functionGenerator;
        public Block pressGenerator;
        public Block addGate;
        public Block decGate;
        public Block mulGate;
        public Block divGate;
        public Block pushCrane;
        public Block goodsSensor;
        public Block shaftSensor;
        public Block steamTurbine;
        public Block alarm;
        public Block wood;
        public Block coal;
        public Block steamEngine;
        public Block terrainSonar;
        public Block ironFurnace;
        public Block electorHeater;
        public Block machineBattery;
        public Block refrigerator;
        public Block auxiliaryGenerator;
        public Block dieselGenerator;
        public Block gearSet;
        public Block smallPropeller;
        public Block advSonar;
        public Block advTerrainSonar;
        public Block turretShell;
        public Block turretCore;
        public Block predictor;
        public Block remainderGate;
        public Block switchGate;

        public Block cargohold;
        public Block woodCargohold;
        public Block drillCore;
        public Block smallElectorEngine;
        public Block stone;
        public Block soil;
        public Block ironOre;
        public Block copperOre;
        public Block sulfurOre;
        public Block stoneLiquid;
        public Block stoneFurnace;
        public Block sulfur;
        public Block sulfurLiquid;
        public Block sulfurMushy;
        public Block sulfurGas;
        public Block sulphuricAcid;
        public Block sulfurDioxide;
        public Block conveyor;
        public Block conveyorCorner;
        public Block transfer;
        public Block grinder;
        public Block centrifuge;
        public Block advGoodsSensor;

        public Block fineSteel;
        public Block coarseIronOre;
        public Block coarseCopperOre;
        public Block coarseSulfurOre;
        public Block fineIronOre;
        public Block fineCopperOre;
        public Block fineSulfurOre;
        public Block dinas;
        public Block asphalt;
        public Block asphaltLiquid;
        public Block asphaltMushy;
        public Block asphaltGas;
        public Block electrolyser;
        public Block distilledWater;
        public Block distilledWaterMushy;
        public Block hydrogen;
        public Block gasHydrate;
        public Block oil;
        public Block oilSolid;
        public Block naturalGas;
        public Block sulphuricAcidSolid;
        public Block sulphuricAcidGas;
        public Block sulfurDioxideSolid;
        public Block sulfurDioxideMushy;
        public Block sulfurDioxideLiquid;
        public Block sulphuricAcidMushy;
        public Block hydrogenLiquid;
        public Block naturalGasLiquid;
        public Block hydrogenMushy;
        public Block naturalGasMushy;
        public Block copperPowder;
        public Block ironPowder;
        public Block sulfurPowder;
        public Block uraniumPowder;
        public Block coalPowder;
        public Block chlorine;
        public Block chlorineMushy;
        public Block chlorineLiquid;
        public Block chlorineSolid;
        public Block semiconductor;
        public Block circuitBoard;
        public Block winding;
        public Block silicon;
        public Block coalDiansMixture;
        public Block compressedAir;
        public Block steelFurnace;
        public Block fineSteelLiquid;
        public Block temperatureInfraredSensor;
        public Block fineSteelGas;
        public Block charcoal;
        public Block mineralDetector;
        public Block sampleCollector;
        public Block thermometer;
        public Block advElectorHeater;
        public Block depthChargeThrowerCore;
        public Block depthCharge;
        public Block advCrane;
        public Block lead;
        public Block leadLiquid;
        public Block leadMushy;
        public Block leadGas;
        public Block leadOre;
        public Block coarseLeadOre;
        public Block fineLeadOre;
        public Block leadPowder;
        public Block vacuumPump;
        public Block selector;
        public Block remoteSignalReceiver;
        public Block remoteSignalTransmitter;
        public Block neutronReflector;
        public Block uraniumGas;
        public Block itemExtractor;
        public Block itemDepositor;

        void registerBlocks()
        {
            air = registerBlock(new Air(getUnuserId(), null, null));
            border = registerBlock(new Border(getUnuserId(), null, null));
            steel = registerBlock(new Steel(getUnuserId(), null, null));
            steelLiquid = registerBlock(new SteelLiquid(getUnuserId(), null, null));
            steelMushy = registerBlock(new SteelMushy(getUnuserId(), null, null));
            water = registerBlock(new Water(getUnuserId(), null, null));//5
            waterMushy = registerBlock(new WaterMushy(getUnuserId(), null, null));
            waterGas = registerBlock(new WaterGas(getUnuserId(), null, null));
            shaft = registerBlock(new Shaft(getUnuserId(), null, null));
            propeller = registerBlock(new Propeller(getUnuserId(), null, null));
            pump = registerBlock(new Pump(getUnuserId(), null, null));//10
            diesel = registerBlock(new Diesel(getUnuserId(), null, null));
            dieselMushy = registerBlock(new DieselMushy(getUnuserId(), null, null));
            dieselGas = registerBlock(new DieselGas(getUnuserId(), null, null));
            fire = registerBlock(new Fire(getUnuserId(), null, null));
            dieselEngine = registerBlock(new DieselEngine(getUnuserId(), null, null));//15
            wireless = registerBlock(new Wireless(getUnuserId(), null, null));
            torpedp = registerBlock(new Torpedp(getUnuserId(), null, null));
            torpedpTube = registerBlock(new TorpedpTube(getUnuserId(), null, null));
            battery = registerBlock(new Battery(getUnuserId(), null, null));
            generator = registerBlock(new Generator(getUnuserId(), null, null));//20
            electorEngine = registerBlock(new ElectorEngine(getUnuserId(), null, null));
            stirlingEngine = registerBlock(new StirlingEngine(getUnuserId(), null, null));
            neutron = registerBlock(new Neutron(getUnuserId(), null, null));
            uranium = registerBlock(new Uranium(getUnuserId(), null, null));
            uraniumLiquid = registerBlock(new UraniumLiquid(getUnuserId(), null, null));
            uraniumMushy = registerBlock(new UraniumMushy(getUnuserId(), null, null));
            controlRod = registerBlock(new ControlRod(getUnuserId(), null, null));
            crane = registerBlock(new Crane(getUnuserId(), null, null));
            cable = registerBlock(new Cable(getUnuserId(), null, null));
            temperatureSensor = registerBlock(new TemperatureSensor(getUnuserId(), null, null));//30
            pressSensor = registerBlock(new PressSensor(getUnuserId(), null, null));
            numericalDisplay = registerBlock(new NumericalDisplay(getUnuserId(), null, null));
            signalLamp = registerBlock(new SignalLamp(getUnuserId(), null, null));
            signalButton = registerBlock(new SignalButton(getUnuserId(), null, null));
            refailButton = registerBlock(new RefailButton(getUnuserId(), null, null));
            wifi = registerBlock(new Wifi(getUnuserId(), null, null));
            logicGate = registerBlock(new LogicGate(getUnuserId(), null, null));
            orGate = registerBlock(new OrGate(getUnuserId(), null, null));
            andGate = registerBlock(new AndGate(getUnuserId(), null, null));
            copper = registerBlock(new Copper(getUnuserId(), null, null));//40
            siliconCarbide = registerBlock(new SiliconCarbide(getUnuserId(), null, null));
            copperLiquid = registerBlock(new CopperLiquid(getUnuserId(), null, null));
            copperMushy = registerBlock(new CopperMushy(getUnuserId(), null, null));
            ice = registerBlock(new Ice(getUnuserId(), null, null));
            steelAsh = registerBlock(new SteelAsh(getUnuserId(), null, null));
            advPump = registerBlock(new AdvPump(getUnuserId(), null, null));
            searchlight = registerBlock(new Searchlight(getUnuserId(), null, null));
            valve = registerBlock(new Valve(getUnuserId(), null, null));
            sidePropeller = registerBlock(new SidePropeller(getUnuserId(), null, null));
            cooler = registerBlock(new Cooler(getUnuserId(), null, null));//50
            smallTorpedp = registerBlock(new SmallTorpedp(getUnuserId(), null, null));
            comparatorGate = registerBlock(new ComparatorGate(getUnuserId(), null, null));
            pressurizer = registerBlock(new Pressurizer(getUnuserId(), null, null));
            functionGenerator = registerBlock(new FunctionGenerator(getUnuserId(), null, null));
            pressGenerator = registerBlock(new PressGenerator(getUnuserId(), null, null));
            addGate = registerBlock(new AddGate(getUnuserId(), null, null));
            decGate = registerBlock(new DecGate(getUnuserId(), null, null));
            mulGate = registerBlock(new MulGate(getUnuserId(), null, null));
            divGate = registerBlock(new DivGate(getUnuserId(), null, null));
            pushCrane = registerBlock(new PushCrane(getUnuserId(), null, null));//60
            goodsSensor = registerBlock(new GoodsSensor(getUnuserId(), null, null));
            shaftSensor = registerBlock(new ShaftSensor(getUnuserId(), null, null));
            steamTurbine = registerBlock(new SteamTurbine(getUnuserId(), null, null));
            alarm = registerBlock(new Alarm(getUnuserId(), null, null));
            wood = registerBlock(new Wood(getUnuserId(), null, null));//65
            coal = registerBlock(new Coal(getUnuserId(), null, null));
            steamEngine = registerBlock(new SteamEngine(getUnuserId(), null, null));
            terrainSonar = registerBlock(new TerrainSonar(getUnuserId(), null, null));
            ironFurnace = registerBlock(new IronFurnace(getUnuserId(), null, null));
            electorHeater = registerBlock(new ElectorHeater(getUnuserId(), null, null));//70
            machineBattery = registerBlock(new MachineBattery(getUnuserId(), null, null));
            refrigerator = registerBlock(new Refrigerator(getUnuserId(), null, null));
            auxiliaryGenerator = registerBlock(new AuxiliaryGenerator(getUnuserId(), null, null));
            dieselGenerator = registerBlock(new DieselGenerator(getUnuserId(), null, null));
            gearSet = registerBlock(new GearSet(getUnuserId(), null, null));
            smallPropeller = registerBlock(new SmallPropeller(getUnuserId(), null, null));//76
            advSonar = registerBlock(new AdvSonar(getUnuserId(), null, null));
            advTerrainSonar = registerBlock(new AdvTerrainSonar(getUnuserId(), null, null));
            turretShell = registerBlock(new TurretShell(getUnuserId(), null, null));
            turretCore = registerBlock(new TurretCore(getUnuserId(), null, null));//80
            predictor = registerBlock(new Predictor(getUnuserId(), null, null));
            remainderGate = registerBlock(new RemainderGate(getUnuserId(), null, null));
            switchGate = registerBlock(new SwitchGate(getUnuserId(), null, null));
            cargohold = registerBlock(new Cargohold(getUnuserId(), null, null));
            woodCargohold = registerBlock(new WoodCargohold(getUnuserId(), null, null));//85
            drillCore = registerBlock(new DrillCore(getUnuserId(), null, null));
            smallElectorEngine = registerBlock(new SmallElectorEngine(getUnuserId(), null, null));
            stoneFurnace = registerBlock(new StoneFurnace(getUnuserId(), null, null));
            stone = registerBlock(new Stone(getUnuserId(), null, null));
            soil = registerBlock(new Soil(getUnuserId(), null, null));//90
            ironOre = registerBlock(new IronOre(getUnuserId(), null, null));
            copperOre = registerBlock(new CopperOre(getUnuserId(), null, null));
            sulfurOre = registerBlock(new SulfurOre(getUnuserId(), null, null));
            stoneLiquid = registerBlock(new StoneLiquid(getUnuserId(), null, null));
            sulfur = registerBlock(new Sulfur(getUnuserId(), null, null));//95
            sulfurLiquid = registerBlock(new SulfurLiquid(getUnuserId(), null, null));
            sulfurMushy = registerBlock(new SulfurMushy(getUnuserId(), null, null));
            sulfurGas = registerBlock(new SulfurGas(getUnuserId(), null, null));
            sulfurDioxide = registerBlock(new SulfurDioxide(getUnuserId(), null, null));
            sulphuricAcid = registerBlock(new SulphuricAcid(getUnuserId(), null, null));//100
            conveyor = registerBlock(new Conveyor(getUnuserId(), null, null));
            conveyorCorner = registerBlock(new ConveyorCorner(getUnuserId(), null, null));
            transfer = registerBlock(new Transfer(getUnuserId(), null, null));
            grinder = registerBlock(new Grinder(getUnuserId(), null, null));
            centrifuge = registerBlock(new Centrifuge(getUnuserId(), null, null));//105
            advGoodsSensor = registerBlock(new AdvGoodsSensor(getUnuserId(), null, null));

            fineSteel = registerBlock(new FineSteel(getUnuserId(), null, null));
            coarseIronOre = registerBlock(new CoarseIronOre(getUnuserId(), null, null));
            coarseCopperOre = registerBlock(new CoarseCopperOre(getUnuserId(), null, null));
            coarseSulfurOre = registerBlock(new CoarseSulfurOre(getUnuserId(), null, null));//110
            fineIronOre = registerBlock(new FineIronOre(getUnuserId(), null, null));
            fineCopperOre = registerBlock(new FineCopperOre(getUnuserId(), null, null));
            fineSulfurOre = registerBlock(new FineSulfurOre(getUnuserId(), null, null));
            electrolyser = registerBlock(new Electrolyser(getUnuserId(), null, null));
            dinas = registerBlock(new Dinas(getUnuserId(), null, null));//115
            asphalt = registerBlock(new Asphalt(getUnuserId(), null, null));
            asphaltLiquid = registerBlock(new AsphaltLiquid(getUnuserId(), null, null));
            asphaltMushy = registerBlock(new AsphaltMushy(getUnuserId(), null, null));
            asphaltGas = registerBlock(new AsphaltGas(getUnuserId(), null, null));         
            distilledWater = registerBlock(new DistilledWater(getUnuserId(), null, null));//120
            distilledWaterMushy = registerBlock(new DistilledWaterMushy(getUnuserId(), null, null));
            hydrogen = registerBlock(new Hydrogen(getUnuserId(), null, null));
            gasHydrate = registerBlock(new GasHydrate(getUnuserId(), null, null));
            oil = registerBlock(new Oil(getUnuserId(), null, null));
            oilSolid = registerBlock(new OilSolid(getUnuserId(), null, null));//125
            naturalGas = registerBlock(new NaturalGas(getUnuserId(), null, null));
            sulphuricAcidSolid = registerBlock(new SulphuricAcidSolid(getUnuserId(), null, null));
            sulphuricAcidGas = registerBlock(new SulphuricAcidGas(getUnuserId(), null, null));
            sulfurDioxideSolid = registerBlock(new SulfurDioxideSolid(getUnuserId(), null, null));
            sulfurDioxideMushy = registerBlock(new SulfurDioxideMushy(getUnuserId(), null, null));//130
            sulfurDioxideLiquid = registerBlock(new SulfurDioxideLiquid(getUnuserId(), null, null));
            sulphuricAcidMushy = registerBlock(new SulphuricAcidMushy(getUnuserId(), null, null));
            hydrogenLiquid = registerBlock(new HydrogenLiquid(getUnuserId(), null, null));
            naturalGasLiquid = registerBlock(new NaturalGasLiquid(getUnuserId(), null, null));
            hydrogenMushy = registerBlock(new HydrogenMushy(getUnuserId(), null, null));//135
            naturalGasMushy = registerBlock(new NaturalGasMushy(getUnuserId(), null, null));
            ironPowder = registerBlock(new IronPowder(getUnuserId(), null, null));
            copperPowder = registerBlock(new CopperPowder(getUnuserId(), null, null));           
            sulfurPowder = registerBlock(new SulfurPowder(getUnuserId(), null, null));
            coalPowder = registerBlock(new CoalPowder(getUnuserId(), null, null));//140  
            uraniumPowder = registerBlock(new UraniumPowder(getUnuserId(), null, null));
            chlorine = registerBlock(new Chlorine(getUnuserId(), null, null));
            chlorineMushy = registerBlock(new ChlorineMushy(getUnuserId(), null, null));
            chlorineLiquid = registerBlock(new ChlorineLiquid(getUnuserId(), null, null));
            chlorineSolid = registerBlock(new ChlorineSolid(getUnuserId(), null, null));//145
            semiconductor = registerBlock(new Semiconductor(getUnuserId(), null, null));
            circuitBoard = registerBlock(new CircuitBoard(getUnuserId(), null, null));
            winding = registerBlock(new Winding(getUnuserId(), null, null));
            silicon = registerBlock(new Silicon(getUnuserId(), null, null));
            coalDiansMixture = registerBlock(new CoalDiansMixture(getUnuserId(), null, null));//150
            compressedAir = registerBlock(new CompressedAir(getUnuserId(), null, null));
            steelFurnace = registerBlock(new SteelFurnace(getUnuserId(), null, null));
            fineSteelLiquid = registerBlock(new FineSteelLiquid(getUnuserId(), null, null));
            temperatureInfraredSensor = registerBlock(new TemperatureInfraredSensor(getUnuserId(), null, null));
            fineSteelGas = registerBlock(new FineSteelGas(getUnuserId(), null, null));//155
            charcoal = registerBlock(new Charcoal(getUnuserId(), null, null));
            mineralDetector = registerBlock(new MineralDetector(getUnuserId(), null, null));
            sampleCollector = registerBlock(new SampleCollector(getUnuserId(), null, null));
            thermometer = registerBlock(new Thermometer(getUnuserId(), null, null));
            advElectorHeater = registerBlock(new AdvElectorHeater(getUnuserId(), null, null));//160
            depthChargeThrowerCore = registerBlock(new DepthChargeThrowerCore(getUnuserId(), null, null));
            depthCharge = registerBlock(new DepthCharge(getUnuserId(), null, null));
            advCrane = registerBlock(new AdvCrane(getUnuserId(), null, null));
            lead = registerBlock(new Lead(getUnuserId(), null, null));
            leadLiquid = registerBlock(new LeadLiquid(getUnuserId(), null, null));//165
            leadMushy = registerBlock(new LeadMushy(getUnuserId(), null, null));
            leadGas = registerBlock(new LeadGas(getUnuserId(), null, null));
            leadOre = registerBlock(new LeadOre(getUnuserId(), null, null));
            coarseLeadOre = registerBlock(new CoarseLeadOre(getUnuserId(), null, null));
            fineLeadOre = registerBlock(new FineLeadOre(getUnuserId(), null, null));//170
            leadPowder = registerBlock(new LeadPowder(getUnuserId(), null, null));
            vacuumPump = registerBlock(new VacuumPump(getUnuserId(), null, null));
            selector = registerBlock(new Selector(getUnuserId(), null, null));
            remoteSignalTransmitter = registerBlock(new RemoteSignalTransmitter(getUnuserId(), null, null));
            remoteSignalReceiver = registerBlock(new RemoteSignalReceiver(getUnuserId(), null, null));//175         
            neutronReflector = registerBlock(new NeutronReflector(getUnuserId(), null, null));
            uraniumGas = registerBlock(new UraniumGas(getUnuserId(), null, null));
            itemExtractor = registerBlock(new ItemExtractor(getUnuserId(), null, null));
            itemDepositor = registerBlock(new ItemDepositor(getUnuserId(), null, null));
            
        }

        static public BlocksManager get()
        {
            return instance == null ? new BlocksManager() : instance;            
        }

        public BlocksManager()
        {
            instance = this;
            blocksArr = new Block[MAX_BLOCK_ID];
            unlockArr = new bool[MAX_BLOCK_ID];
            idStack = 0;
            blockCount = 0;
            isRegisterEnd = false;
            registerBlocks();
            initializedUnlockData();
            loadUnlockData();
            initializedStoreData();
            loadStationInfos();
            initializedCollectedScientificId();
            createJson();            
            isRegisterEnd = true;
        }
       

        public Block registerBlock(Block block)
        {
            block = block.clone(null, this, null);
            block.onRegister();
            int id = block.getId();
            blocksArr[id] = block;
            blockCount++;
            Debug.Log(string.Format("{0}, heatCapacity:{1}", block.getName(), block.heatCapacity));
            return block;
        }

        public void setBlocksOnRegister()
        {
            for (int i = 0; i < idStack; i++)
            {
                blocksArr[i].onRegister();
            }               
        }

        void initializedUnlockData()
        {
            for (int i = 0; i < MAX_BLOCK_ID; i++)
            {
                if (blocksArr[i] != null)
                {
                    unlockArr[i] = blocksArr[i].isRootUnlock();
                }
                else
                {
                    unlockArr[i] = false;
                }
            }
        }

        public void loadUnlockData()
        {
            ISecretLoad.init();
            unlockData = ISecretLoad.loadResearch();
            if (unlockData != null)
            {
                for (int i = 0; i < unlockData.Length; i++)
                {
                    unlockArr[unlockData[i]] = true;
                }
            }
        }

        public int[] getUnlockData()
        {
            return unlockData;
        }

        public bool getIsUnlock(Block block)
        {
            return unlockArr[block.getId()];
        }

        public bool getIsUnlock(int blockId)
        {
            return unlockArr[blockId];
        }

        void initializedStoreData()
        {
            canStoreIn = new int[idStack];
            for (int i = 0; i < idStack; i++)
            {
                canStoreIn[i] = blocksArr[i].isCanStoreInWarehouse();
            }
        }

        public bool getCanStoreInSoild(int blockId)
        {
            return canStoreIn[blockId] == 1;
        }

        public bool getCanStoreInLiquid(int blockId)
        {
            return canStoreIn[blockId] == 2;
        }

        public void loadStationInfos()
        {
            mainStationIndex = ISecretLoad.loadStationsInfo(out stationInfo);
            if (stationInfo != null)
            {
                mainStationCargoCounts = stationInfo[mainStationIndex].cargoCounts;
            }
            else
            {
                mainStationCargoCounts = new int[idStack];
                IUtils.initializedArray(mainStationCargoCounts, 0);
                initFirstInCargoCounts(ref mainStationCargoCounts);
            }

            consumeCargoCounts = new int[idStack];
            IUtils.initializedArray(consumeCargoCounts, 0);
        }

        static public void initFirstInCargoCounts(ref int[] cargosCount)
        {
            cargosCount[8] = 24;//转轴
            cargosCount[10] = 6;//水泵
            cargosCount[16] = 5;//无线电接收器
            cargosCount[65] = 200;//木头
            cargosCount[66] = 40;//煤炭
            cargosCount[67] = 32;//蒸汽引擎
            cargosCount[76] = 32;//小型螺旋桨
            cargosCount[85] = 100;//木制货舱
            cargosCount[86] = 3;//钻头核心
        }

        public void initializedCollectedScientificId()
        {
            csIdStack = 0;
            for (int i = 0; i < idStack; i++)
            {
                SolidBlock solidBlock = blocksArr[i] as SolidBlock;
                if (solidBlock != null && solidBlock.isCanCollectScientific())
                {
                    solidBlock.setCollectedScientificId(csIdStack);
                    csIdStack++;
                }
            }
        }

        public int getCanCollectedScientificBlockCount()
        {
            return csIdStack;
        }

        public int getMainStationCargoCount(Block block)
        {

            if (block.isCareerModeStoreUnlimit())
            {
                return int.MaxValue;
            }

            return mainStationCargoCounts[block.getId()];
        }

        public void addConsumeCargoCount(Block block, int count)
        {
            if (consumeCargoCounts != null && !block.isCareerModeStoreUnlimit())
            {
                consumeCargoCounts[block.getId()] += count;
                mainStationCargoCounts[block.getId()] -= count;
            }
        }

        public int[] getRemaindeCargoCounts()
        {
            return mainStationCargoCounts;
        }

        public int getUnuserId()
        {
            return idStack++;
        }

        public Block getBlockById(int id)
        {
            return blocksArr[id];
        }

        public Block getBlockByName(string name)
        {
            if (isRegisterEnd == false)
            {
                return null;
            }
            if (name == "null")
            {
                return null;
            }

            for (int i = 0; i < MAX_BLOCK_ID; i++)
            {
                Block block = blocksArr[i];
                if (block != null)
                {
                    if (block.getName().Equals(name))
                    {
                        return block;
                    }
                }
            }
            Debug.Log("[Warning]can't find " + name);

            return null;
        }

        public int getBlockCount()
        {
            return blockCount;
        }

        void createJson()
        {
            if (GameSetting.isAndroid || GameSetting.isSteam)
            {
                return;
            }

            Debug.Log("[Warning]Create block json!");

            //createBlocksShopJson();
            //createBlocksWikiJson();
        }

        void createBlocksShopJson()
        {
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();
            writer.WritePropertyName("RECORDS");
            writer.WriteArrayStart();
            for (int i = 0; i < idStack; i++)
            {
                Block block = blocksArr[i];

                writer.WriteObjectStart();
                IUtils.keyValue2Writer(writer, "ID", i);
                IUtils.keyValue2Writer(writer, "blockId", i);
                IUtils.keyValue2Writer(writer, "blockName", block.getLangName());
                IUtils.keyValue2Writer(writer, "price", 0);
                IUtils.keyValue2Writer(writer, "can_sell", 0);
                IUtils.keyValue2Writer(writer, "is_sell", 0);
                IUtils.keyValue2Writer(writer, "type", 0);
                IUtils.keyValue2Writer(writer, "sell_count", 0);

                writer.WriteObjectEnd();
            }
            writer.WriteArrayEnd();
            writer.WriteObjectEnd();
            IUtils.write2txt(GamePath.SDPATH + "blocksShop.json", writer.ToString());
        }

        void createBlocksWikiJson()
        {
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();
            writer.WritePropertyName("RECORDS");
            writer.WriteArrayStart();
            for (int i = 0; i < idStack; i++)
            {
                Block block = blocksArr[i];

                writer.WriteObjectStart();
                CardInfo cardInfo = CardsRegister.get().getCardInfoByName(block.getAttributeCardName());
                IUtils.keyValue2Writer(writer, "blockId", i);
                IUtils.keyValue2Writer(writer, "blockGameName", block.getName());                
                IUtils.keyValue2Writer(writer, GameSetting.lang == 0 ? "blockName_eng" : "blockName", block.getLangName());
                IUtils.keyValue2Writer(writer, "cardId", cardInfo == null ? 0 : cardInfo.rank + 1);
                IUtils.keyValue2Writer(writer, GameSetting.lang == 0 ? "pstate_eng" : "pstate", PState.getPStateLangName(block.getPState()));
                IUtils.keyValue2Writer(writer, "mass", block.getMass());
                IUtils.keyValue2Writer(writer, "storeAir", block.getCanStoreAir());
                //IUtils.keyValue2Writer(writer, "heatCapacity", block.getHeatCapacity());               
                IUtils.keyValue2Writer(writer, "transmissivity", block.transmissivity);
                IUtils.keyValue2Writer(writer, "calorific", block.getCalorific());
                IUtils.keyValue2Writer(writer, "burningPoint", block.getBurningPoint());
                IUtils.keyValue2Writer(writer, "meltingPoint", block.getMeltingPoint());
                IUtils.keyValue2Writer(writer, "maxPress", block.getSolidMaxPress());
                IUtils.keyValue2Writer(writer, "freezingPoint", block.getFreezingPoint());
                IUtils.keyValue2Writer(writer, "boilingPoint", block.getBoilingPoint());
                IUtils.keyValue2Writer(writer, "penetrationRate", block.getPenetrationRate());
                IUtils.keyValue2Writer(writer, GameSetting.lang == 0 ? "synt_eng" : "synt", IUtils.getSyntString(block.getSyntInfo(this), "<br>"));                

                writer.WriteObjectEnd();
            }
            writer.WriteArrayEnd();
            writer.WriteObjectEnd();
            IUtils.write2txt(GamePath.SDPATH + "blocksWiki.json", writer.ToString());
        }
    }
}
