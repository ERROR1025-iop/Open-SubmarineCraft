using LitJson;
using Scraft.AchievementSpace;
using Scraft.BlockSpace;
using Scraft.DpartSpace;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Scraft.StationSpace;

namespace Scraft
{
    public class Pooler : MonoBehaviour
    {
        public static string curScenceName = "Pooler";
        public static int POOLER_MODE = 0;

        static public bool IS_Form_StationCenter = false;
        public static bool Is_From_Builder = true;
        public static string FromShip_ShipName;
        public static string FromShip_AssName;
        public static Vector3 FromShip_Position;
        public static Vector3 FromShip_EulerAngle;

        public bool frameDebug = false;
        bool frameKeyDown = false;



        public static Pooler instance;
        BlocksEngine blocksEngine;
        BlocksManager blocksManager;
        DpartsManager dpartsManager;
        DpartsEngine dpartsEngine;
        public ACManager achManager;
        public PoolerUI poolerUI;


        static Text modeText;
        public string subName;    

        static public string assData;    
        static public bool IS_MainSubmarine_initialized_Finish;

        WaveMono waveMono;
        static public int wave;
        static public bool isStartBattle;

        static public bool isGlobalShowIcon;

        public int fireWeapon;

        public float collectedScientific;

        /// <summary>
        ///outsideArea:1(outside),4(border),3(water),2(cancel),5(soild),0(inside)
        /// </summary>
        public static int[,] outsideArea;
        /// <summary>
        ///outsideArea:1(outside),0(cargo)
        /// </summary>
        public static int[,] cargoArea;
        public static int[] mainStationRemaindeCargoCounts;
        /// <summary>
        ///HeatMap_Mode:0(close),0(tempture),1(press)
        /// </summary>       
        public static int HeatMap_Mode;
        public static int terrainLayer;
        public static float terrainPosY;
        public static float shotPredictorData;
        public static bool isOpen_TerrainSonar;
        int m_Open_TerrainAdvSonar;
        int[] sonar_ShaderPropertyToIDs;
        public static Material[] shareMaterials;
        IRect shipRect1;
        IRect shipRect2;
        GameObject waterBackGroundObject;
        public static float waterBackGroundY;
        public static int tonnage;
        public static int orgTonnage;
        public static float mass;
        public static int wirelessCount;
        public static int searchlightCount;
        public static float shipOffsetY;
        public static float shipPlaneY;
        public static float massOffsetY;
        public static float wbgOffsetY;
        public static Quaternion blocksMapQuaternion;
        public static IPoint blocksMoveVector;
        public static int underwaterCount;

        public static float stayTemperture = 25;

        List<Block> frameArr;
        public List<IPoint> cargoCoors;

        [HideInInspector] public float timer;
        float runSpeed;

        bool[] recordIsAddCount;

        AudioSource audioSource;
        AudioSource audioSourceBgm;
        AudioClip matelcrash01;
        AudioClip matelcrash02;
        AudioClip matelcrash03;

        [HideInInspector] public float startSecond;

        Thread updataThread;
        public static bool isRunThread;
        public bool isGameStart5Second;

        void Awake()
        {
            instance = this;
            blocksEngine = BlocksEngine.instance;
            blocksManager = BlocksManager.instance;
            dpartsManager = DpartsManager.instance;
            dpartsEngine = new DpartsEngine(World.dpartParentObject);
            achManager = ACManager.instance;
            subName = World.mapName;        
            modeText = GameObject.Find("Canvas/radar rect/radar text rect/mode").GetComponent<Text>();
            waveMono = GameObject.Find("Canvas/wave").GetComponent<WaveMono>();
            outsideArea = new int[World.mapSize.x, World.mapSize.y];
            poolerUI = new PoolerUI();


            runSpeed = 0.05f;
            wave = 1;
            isStartBattle = false;
            wirelessCount = 0;
            searchlightCount = 0;
            isGameStart5Second = false;
            AISubMono.AI_Count = 0;
            isGlobalShowIcon = true;
            AISubMono.nearestAiDistance = float.MaxValue;
            AISubMono.aiList = new List<AISubMono>();
            Station.stations = new List<Station>();
            ShipPreload.shipPreloads = new List<ShipPreload>();

            frameArr = new List<Block>();
            cargoCoors = new List<IPoint>();

            audioSource = GameObject.Find("pool_preb/assist/Sound").GetComponent<AudioSource>();
            audioSourceBgm = GameObject.Find("MainCamera").GetComponent<AudioSource>();
            matelcrash01 = Resources.Load("sounds/matelcrash01", typeof(AudioClip)) as AudioClip;
            matelcrash02 = Resources.Load("sounds/matelcrash02", typeof(AudioClip)) as AudioClip;
            matelcrash03 = Resources.Load("sounds/matelcrash03", typeof(AudioClip)) as AudioClip;

            achManager.setACBarMono(GameObject.Find("Canvas/ac").GetComponent<ACBarMono>());

            initShareMaterial();

            startSecond = Time.fixedTime;

            HeatMap_Mode = 0;
            terrainPosY = 0;
            terrainLayer = 1 << 10;
            isRunThread = false;

            recordIsAddCount = new bool[World.mapSize.y];

            sonar_ShaderPropertyToIDs = new int[1];
            sonar_ShaderPropertyToIDs[0] = Shader.PropertyToID("_OpenSonar");

            if (GameSetting.isAndroid)
            {
                Camera.main.renderingPath = RenderingPath.Forward;
            }
            else
            {
                Camera.main.renderingPath = GameSetting.renderMode == 2 ? RenderingPath.DeferredShading : RenderingPath.Forward;
            }
            
            if (GameSetting.waveMode > 0)
            {
                if (GameSetting.waveMode == 1)
                {
                    Instantiate(Resources.Load("Prefabs/Pooler/Ocean1"));
                }
                if (GameSetting.waveMode == 2)
                {
                    Instantiate(Resources.Load("Prefabs/Pooler/Ocean2"));
                }
                GameObject.Find("3D Water")?.SetActive(false);
                GameObject.Find("Directional Light Water").transform.localEulerAngles = new Vector3(21.5f, 27.34f, -57.5f);
            }  

            m_Open_TerrainAdvSonar = 1;
            setAdvTerrainSonarOpen(false);

            collectedScientific = 0;
        }

        void Start()
        {
            blocksManager.setBlocksOnRegister();
            loadShipMap();
            blocksMapOffsetInit();
            initOutsideArea();
            programShipArea();
            loadDpartsMap();
            initBackGroundPosition();
            fillShipArea();           
            orgTonnage = programShipTonnage();
            poolerUI.init();
            initMapBlocks();
            initCargoArea();
            loadPoolerData(); 
            startAiSequence();
            StartCoroutine(FirstFrameLoad());

            startUpdataThread();

            UnityAndroidEnter.CallCloseInterstitialAD();
        }

        System.Collections.IEnumerator FirstFrameLoad()
        {
            // 等待一帧
            yield return null;
            MainSubmarine.transform.position = ISecretLoad.spwans[0];
            MainSubmarine.transform.eulerAngles = ISecretLoad.spwans[1];
        }

        void Update()
        {
            if (World.stopUpdata == false)
            {
                if (frameDebug)
                {
                    if (!Input.GetKeyDown(KeyCode.L))
                    {
                        frameKeyDown = false;
                        return;
                    }
                    frameKeyDown = true;
                }

                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    mass = 0;
                    tonnage = programShipTonnage();

                    steadyborderWater(shipRect2);
                    chargeAirWhenOnSuffer();
                    blocksEngine.updata(shipRect2);
                    timer = runSpeed;

                    MainSubmarine.tonnage = (tonnage - orgTonnage) * 30;

                }
            }
            frameUpdata();
            poolerUI.updata();
        }

        private void FixedUpdate()
        {
            sendTerrainSonarShaderData();
            calculateTerrainPosY();
            calculateShotPredict();
        }

        void startUpdataThread()
        {
            isRunThread = true;
            updataThread = new Thread(threadUpdata);
            updataThread.Start();
        }

        public void threadUpdata()
        {
            while (isRunThread)
            {
                
                if (World.stopUpdata == false)
                {
                    if (timer <= 0)
                    {

                        if (frameDebug && !frameKeyDown)
                        {
                            Thread.Sleep(2);
                            continue;
                        }

                        blocksEngine.threadUpdata(shipRect2);
                        Thread.Sleep(2);
                    }
                }
            }
        }

        public void frameUpdata()
        {
            int count = frameArr.Count;
            for (int i = 0; i < count; i++)
            {
                Block block = frameArr[i];
                if (block != null)
                {
                    block.frameUpdate();
                }
            }
        }

        public void playMatelCrashSound()
        {
            int p = (int)(Random.value * 100) % 3;
            switch (p)
            {
                case 0:
                    playSound(matelcrash01);
                    break;
                case 1:
                    playSound(matelcrash02);
                    break;
                case 2:
                    playSound(matelcrash03);
                    break;
                default:
                    playSound(matelcrash01);
                    break;
            }
        }

        public void playSound(AudioClip clip, bool isMuteBgm = false)
        {
            if (audioSource.isPlaying == false)
            {
                audioSource.clip = clip;
                audioSource.Play();

                if (isMuteBgm)
                {
                    muteBackGroundSound(true);
                }
            }
        }

        public void stopSound()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                muteBackGroundSound(false);
            }
        }

        public void muteBackGroundSound(bool isMute)
        {
            audioSourceBgm.mute = isMute;
        }

        public void decWirelessCount()
        {
            wirelessCount--;
            if (wirelessCount <= 0)
            {
                float maxDeep = MainSubmarine.maxDeep;
                int attackAiCount = poolerUI.attackAiCount;
                float maxSpeed = MainSubmarine.maxSpeed;

                poolerUI.poolerGameOver.setInfo((int)(Time.fixedTime - startSecond), (int)maxDeep, wave, attackAiCount);
                poolerUI.poolerGameOver.show(true);
            }
        }

        void calculateShotPredict()
        {
            Vector3 aiPlanePos = IUtils.vectorRoundInPlane(AISubMono.nearestAiPosition - MainSubmarine.transform.position, 0);
            float assistAimRotateAngle = IUtils.AngleSigned(aiPlanePos, -MainSubmarine.transform.right, Vector3.down);
            assistAimRotateAngle = IUtils.reviseAngleIn360(assistAimRotateAngle);
            shotPredictorData = ShotShellRS.assistAimPitchAngle + (int)assistAimRotateAngle * 100 + 0.99f;
            AISubMono.nearestAiDistance = float.MaxValue;
        }

        void calculateTerrainPosY()
        {
            if (isOpen_TerrainSonar)
            {
                Ray ray = new Ray(MainSubmarine.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 500, terrainLayer))
                {
                    terrainPosY = hit.point.y;
                }
            }
        }

        public void decSearchlightCount()
        {
            searchlightCount--;
        }

        public Color getHeatColor(float v)
        {
            //0.67-(1/(1+e^(-t/400))-0.5)*1.33
            float H = 0.68f - (IUtils.sigmoid(v / 400) - 0.5f) * 1.33f;
            Color color = Color.HSVToRGB(H, 1, 1);
            return color;
        }

        public void setAdvTerrainSonarOpen(bool open)
        {
            if (open != (m_Open_TerrainAdvSonar == 1))
            {
                m_Open_TerrainAdvSonar = open ? 1 : 0;
                Shader.SetGlobalVector(sonar_ShaderPropertyToIDs[0], new Vector4(m_Open_TerrainAdvSonar, 0, 0, 0));
            }
        }


        public bool isOpenAdvTerrainSonar()
        {
            return m_Open_TerrainAdvSonar == 1;
        }

        public void sendTerrainSonarShaderData()
        {
            if (m_Open_TerrainAdvSonar == 1)
            {         
                if ((Time.time * 10) % 2 == 1)
                {
                    playSound(MainSubmarine.bgms[0]);
                }
            }
        }

        void onUploadRecordResponse()
        {

        }

        public float getCanChargeElectric()
        {
            return poolerUI.electricMeter.getMaxElectric() - poolerUI.electricMeter.getElectric();
        }

        /// <summary>
        /// 充电
        /// </summary>
        public void chargeElectric(Block generator, float charge)
        {
            int count = Battery.battersArr.Count;
            Battery battery;
            for (int i = 0; i < count; i++)
            {
                battery = Battery.battersArr[i];
                if (battery != null && battery.getMaxStoreElectric() > 0)
                {
                    charge = battery.chargeElectric(generator, charge);
                    if (charge == 0)
                    {
                        poolerUI.updateElectric();
                        return;
                    }
                }
            }
            poolerUI.updateElectric();
        }

        /// <summary>
        /// 要电,返回获得成功的电能。
        /// </summary>
        public float requireElectric(Block taker, float require)
        {
            if (Application.isEditor)
            {
                return require;
            }
            float receive = 0;
            int count = Battery.battersArr.Count;
            Battery battery;
            for (int i = 0; i < count; i++)
            {
                battery = Battery.battersArr[i];
                if (battery != null && battery.getMaxStoreElectric() > 0)
                {
                    if (battery.getStroreElectric() > 0)
                    {
                        receive += battery.requireElectric(taker, require - receive);
                        if (receive == require)
                        {
                            poolerUI.updateElectric();
                            return receive;
                        }
                    }
                }
            }
            poolerUI.updateElectric();
            return receive;
        }

        /// <summary>
        /// 充入空气
        /// </summary>
        public void chargeAir(float charge)
        {           
            Block block;
            for (int x = shipRect2.x; x < shipRect2.getMaxX(); x++)
            {
                for (int y = shipRect2.y; y < shipRect2.getMaxY(); y++)
                {
                    if (outsideArea[x, y] == 0)
                    {
                        block = blocksEngine.getBlock(x, y);
                        if (block != null && block.getCanStoreAir() > 0)
                        {
                            charge = block.chargeAir(charge);
                            if (charge == 0)
                            {
                                poolerUI.updateAir();
                                return;
                            }
                        }
                    }
                }
            }
            poolerUI.updateAir();
        }

        /// <summary>
        /// 获取空气,返回获得成功的空气量。
        /// </summary>
        public float requireAir(float require)
        {
            if(require > 0)
            {
                float receive = 0;
                Block block;
                for (int x = shipRect2.x; x < shipRect2.getMaxX(); x++)
                {
                    for (int y = shipRect2.y; y < shipRect2.getMaxY(); y++)
                    {                        
                        if(outsideArea[x,y] == 0)
                        {
                            block = blocksEngine.getBlock(x, y);
                            if (block != null && block.getCanStoreAir() > 0)
                            {
                                if (block.getStoreAir() > 0)
                                {
                                    receive += block.requireAir(require - receive);
                                    if (receive == require)
                                    {
                                        poolerUI.updateAir();
                                        return receive;
                                    }
                                }
                            }
                        }                       
                    }
                }
                poolerUI.updateAir();
                return receive;
            }
            return 0;
        }

        public void changeFireWeapon(int fireWeapon)
        {
            this.fireWeapon = fireWeapon;
            poolerUI.changeFireImageWeapon(fireWeapon);
        }

        /// <summary>
        /// 获取一枚鱼雷
        /// </summary>
        public bool takeOneTorpedp(Block taker)
        {

            for (int x = shipRect2.getMinX() - 3; x < shipRect2.getMaxX() - 1; x++)
            {
                for (int y = shipRect2.getMinY() - 3; y < shipRect2.getMaxY() - 1; y++)
                {
                    Block block = blocksEngine.getBlock(x, y);
                    if (block != null && block.isNeedDelete() == false)
                    {
                        if (block.giveOneTorpedp(taker))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 获取一枚炮弹
        /// </summary>
        public bool takeOneShell(Block taker)
        {

            for (int x = shipRect2.getMinX() - 3; x < shipRect2.getMaxX() - 1; x++)
            {
                for (int y = shipRect2.getMinY() - 3; y < shipRect2.getMaxY() - 1; y++)
                {
                    Block block = blocksEngine.getBlock(x, y);
                    if (block != null && block.isNeedDelete() == false)
                    {
                        if (block.giveOneShell(blocksEngine, taker))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 获取一枚深水炸弹
        /// </summary>
        public bool takeOneDepthCharge(Block taker)
        {

            for (int x = shipRect2.getMinX() - 3; x < shipRect2.getMaxX() - 1; x++)
            {
                for (int y = shipRect2.getMinY() - 3; y < shipRect2.getMaxY() - 1; y++)
                {
                    Block block = blocksEngine.getBlock(x, y);
                    if (block != null && block.isNeedDelete() == false)
                    {
                        if (block.giveOneDepthCharge(blocksEngine, taker))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 发射一枚鱼雷
        /// </summary>
        public void fireOneTorpedp(Block firer, float targetAngle, Vector3 firePos, float deep)
        {
            //2D画面鱼雷
            GameObject torpedp = Instantiate(Resources.Load("Prefabs/Pooler/torpedp")) as GameObject;
            torpedp.transform.localPosition = firePos;
            //3D画面
            MainSubmarine.addTorpedp(targetAngle, deep); 
        }

        /// <summary>
        /// 获取货物
        /// </summary>
        public bool getCargos(Block[] blocksStatic, int[] counts, out IPoint[] coors, bool remove)
        {
            //如果需求总数小于货舱大小，返回FALSE
            coors = null;
            int totalCount = 0;
            foreach (int c in counts)
            {
                totalCount += c;
            }
            if (cargoCoors.Count < totalCount)
            {
                return false;
            }

            List<IPoint> totalCoors = new List<IPoint>();

            for (int k = 0; k < blocksStatic.Length; k++)
            {
                Block blockStatic = blocksStatic[k];
                int count = counts[k];
                int i = 0;
                foreach (IPoint coor in cargoCoors)
                {
                    Block block = blocksEngine.getBlock(coor);
                    if (block != null && !block.isNeedDelete() && block.equalBlock(blockStatic))
                    {
                        totalCoors.Add(coor);
                        i++;
                    }
                    if (i >= count)
                    {
                        break;
                    }
                }

                if (i < count)
                {
                    return false;
                }
            }

            if (remove)
            {
                foreach (IPoint coor in totalCoors)
                {
                    blocksEngine.removeBlock(coor);
                }
            }
            coors = totalCoors.ToArray();
            return true;
        }

        //将新方块存入货舱
        public bool addCargo(Block blockStatic)
        {
            foreach (IPoint coor in cargoCoors)
            {
                Block block = blocksEngine.getBlock(coor);
                if (block.isAir())
                {
                    blocksEngine.createBlock(coor, blockStatic);
                    return true;
                }
            }
            return false;
        }

        //将已存在的方块存入货舱(使用moveTo方法)
        public bool depositCargo(Block cargoBlock)
        {
            foreach (IPoint coor in cargoCoors)
            {
                Block block = blocksEngine.getBlock(coor);
                if (block.isAir())
                {
                    cargoBlock.moveTo(coor);
                    return true;
                }
            }
            return false;
        }

        public int[] getRecoveryShipBlocks()
        {
            int[] recovertCargos = new int[blocksManager.getBlockCount()];
            IUtils.initializedArray(recovertCargos, 0);
            for (int x = shipRect2.getMinX() - 3; x < shipRect2.getMaxX() - 1; x++)
            {
                for (int y = shipRect2.getMinY() - 3; y < shipRect2.getMaxY() - 1; y++)
                {
                    Block block = blocksEngine.getBlock(x, y);
                    if (block != null && block.isNeedDelete() == false)
                    {
                        if(block.isCanStoreInWarehouse() == 1 || block.isCanStoreInWarehouse() == 2)
                        {
                            recovertCargos[block.getId()] += 1;
                        }
                    }
                }
            }
            return recovertCargos;
        }

        public float recoveryBlocksScientific()
        {
            float result = 0;
            for (int x = shipRect2.getMinX() - 3; x < shipRect2.getMaxX() - 1; x++)
            {
                for (int y = shipRect2.getMinY() - 3; y < shipRect2.getMaxY() - 1; y++)
                {
                    SolidBlock block = blocksEngine.getBlock(x, y) as SolidBlock;
                    if (block != null && block.isCanCollectScientific() && block.isNeedDelete() == false)
                    {
                        result += block.getCollectedScientific();
                        block.resetCollectedScientific();
                        
                    }
                }
            }            
            collectedScientific += result;
            ScientificSelector.instance.updateBlockValue();
            return result;           
        }

        public void savePoolerData(bool isRecoveryMainShip)
        {          
            if (isRecoveryMainShip)
            {
                PoolerStationMenu.instance.onRecoveryPowerButtonClick();
                recoveryBlocksScientific();
                ISecretLoad.setScientific(ISecretLoad.getScientific() + collectedScientific);
            }
            else
            {
                saveMainShip();                
            }
            Station.saveStations();
            ISecretLoad.saveWorldAreaInfo();
            ISecretLoad.saveWorldShipsInfo(!isRecoveryMainShip);
        }        

        void loadPoolerData()
        {
            Station.loadStations();
            ISecretLoad.loadWorldShipInfo();
            ISecretLoad.loadWorldAreaInfo();
        }       

        public void startAiSequence()
        {
            waveMono.setShowFinishedListener(onWaveMoveEnd);
            waveMono.startShow(wave);
        }

        void onWaveMoveEnd()
        {
            AISubMono.createAi(wave);
            isStartBattle = true;
        }

        public void entrnNextWave()
        {
            isStartBattle = false;
            wave++;
            waveMono.startShow(wave);
        }



        void loadDpartsMap()
        {
            IS_MainSubmarine_initialized_Finish = false;
            dpartsEngine.removeAllDpart();

            JsonData jsonData;
            if (!IS_Form_StationCenter)
            {
                string path = GamePath.dpartFolder + "Last Ship.ass";
                if (!File.Exists(path))
                {
                    TextAsset textAsset = Resources.Load("Pooler/submarine") as TextAsset;
                    assData = textAsset.text;
                }
                else
                {
                    assData = IUtils.readFromTxt(path);
                }
                jsonData = JsonMapper.ToObject(assData);
            }
            else
            {
                assData = ISecretLoad.readWithVerifyMd5(FromShip_AssName);
                jsonData = JsonMapper.ToObject(assData);
            }

           
            int blocksCount = IUtils.getJsonValue2Int(jsonData, "count");
            shipPlaneY = IUtils.getJsonValue2Float(jsonData, "plane");
            massOffsetY = IUtils.getJsonValue2Float(jsonData, "mass");

            if (blocksCount == 0)
            {
                TextAsset textAsset = Resources.Load("Pooler/submarine") as TextAsset;
                jsonData = JsonMapper.ToObject(textAsset.text);
                blocksCount = IUtils.getJsonValue2Int(jsonData, "count");
            }

            JsonData blocksArrData = jsonData["dparts"];

            JsonData dpartData;
            Dpart dpart;
            Transform parentTrans = dpartsEngine.dpartParentObject.transform;

            for (int i = 0; i < blocksCount; i++)
            {
                dpartData = blocksArrData[i];

                int dpartId = IUtils.getJsonValue2Int(dpartData, "id");
                if (dpartId == -1)
                {
                    GroupDpart groupDpart = new GroupDpart(-1, dpartsEngine.dpartParentObject);
                    groupDpart.initGroupDpart("group_" + i);
                    dpartsEngine.addDpartArr(groupDpart);
                    dpart = groupDpart;
                }
                else
                {
                    dpart = dpartsEngine.createDPart(dpartsManager.getDPartById(dpartId));
                }
                dpart.onBuilderModeLoad(dpartData, dpartsEngine);
                dpart.onPoolerModeCreate();
            }

            wbgOffsetY = IUtils.centerOfGameObjects(dpartsEngine.dpartParentObject).y;
            IUtils.centerOnChildrens(dpartsEngine.dpartParentObject, Vector3.zero);            
            MainSubmarine.instance.onDpartMapLoadFinish(dpartsEngine.dpartParentObject);
            //combineColliderMeshes(dpartsEngine.dpartParentObject, MainSubmarine.transform.GetComponent<MeshCollider>());
            //combineRenderMeshes(dpartsEngine.dpartParentObject);

            shipOffsetY = -(wbgOffsetY - shipPlaneY);


            DpartParent[] dpartParents = dpartsEngine.dpartParentObject.GetComponentsInChildren<DpartParent>();
            foreach (var dp in dpartParents)
            {
                Dpart dpart2 = dp.getDpart();
                IPoint Map2DCoor = IPoint.createMapIPointByWordVector(dpart2.getTransform().localPosition, World.mapSize);
                dpart2.set2DMapCoor(Map2DCoor - blocksMoveVector);
            }

            if (IS_Form_StationCenter)
            {
                MainSubmarine.transform.position = FromShip_Position;
                MainSubmarine.transform.eulerAngles = FromShip_EulerAngle;
                
            }

            IS_MainSubmarine_initialized_Finish = true;
        }

        static public void combineColliderMeshes(GameObject gos, MeshCollider meshCollider)
        {

            DpartChild[] dpartChilds = gos.GetComponentsInChildren<DpartChild>();
            DpartChild child;

            int colliderCount = 0;
            int combineCount = 0;
            for (int i = 0; i < dpartChilds.Length; i++)
            {
                child = dpartChilds[i];
                if (child.isCombineCollider && child.isContainCollider)
                {
                    float dy = MainSubmarine.transform.TransformPoint(MainSubmarine.weightCenter).y - MainSubmarine.bounds.min.y;
                    if (child.transform.TransformPoint(child.meshFilter.mesh.bounds.max).y < dy * 2)
                    {
                        combineCount++;
                    }
                    else
                    {
                        child.isCombineCollider = false;
                    }
                    colliderCount++;
                }
                else if (child.isContainCollider)
                {
                    Destroy(child.GetComponent<Collider>());
                }
            }

            if (colliderCount == 1)
            {
                return;
            }

            CombineInstance[] combine = new CombineInstance[combineCount];

            int combineStack = 0;
            for (int i = 0; i < dpartChilds.Length; i++)
            {
                child = dpartChilds[i];
                if (child.isContainCollider)
                {
                    if (child.isCombineCollider)
                    {
                        MeshFilter meshFilters = child.transform.GetComponent<MeshFilter>();
                        combine[combineStack].mesh = meshFilters.sharedMesh;
                        combine[combineStack].transform = meshFilters.transform.localToWorldMatrix;
                        combineStack++;
                    }
                    //Destroy(child.gameObject);
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "Collider Mesh";
            mesh.CombineMeshes(combine);
            meshCollider.sharedMesh = mesh;

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
                if (child.isCombineRenderMesh && child.getDpart().getMaterialName().Equals("normal"))
                {

                    normalCount++;
                    combineCount++;
                }
                else
                {
                    child.isCombineRenderMesh = false;
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
                    combine[combineStack].mesh = meshFilters.sharedMesh;
                    combine[combineStack].transform = meshFilters.transform.localToWorldMatrix;
                    combineStack++;
                    Destroy(child.gameObject);
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "Render mesh";
            mesh.CombineMeshes(combine);

            gos.GetComponent<MeshFilter>().mesh = mesh;
            gos.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Dparts/ColorMat/normal", typeof(Material)) as Material;
        }

        protected void loadShipMap()
        {
            JsonData jsonData;
            if (!IS_Form_StationCenter)
            {
                string path = GamePath.shipsFolder + "Last Ship" + ".ship";
                jsonData = JsonMapper.ToObject(IUtils.readFromTxt(path));
            }
            else
            {
                string data = ISecretLoad.readWithVerifyMd5(FromShip_ShipName);                
                jsonData = JsonMapper.ToObject(data);
            }      
           
            int blocksCount = IUtils.getJsonValue2Int(jsonData, "count");
            JsonData blocksArrData = jsonData["blocks"];

            for (int i = 0; i < blocksCount; i++)
            {
                JsonData blockData = blocksArrData[i];

                int blockId = IUtils.getJsonValue2Int(blockData, "id");
                int x = IUtils.getJsonValue2Int(blockData, "x");
                int y = IUtils.getJsonValue2Int(blockData, "y");
                IPoint coor = new IPoint(x, y);
                Block blockStatic = blocksManager.getBlockById(blockId);
                if (blockStatic == null)
                {
                    continue;
                }
                Block block = blocksEngine.createBlock(coor, blockStatic);
                block.onWorldModeLoad(blockData, coor);
                block.setPosition(coor);
                block.setTemperature(25);
                blocksEngine.setBlock(coor, block);
            }            
        }

        void blocksMapOffsetInit()
        {            

            IPoint center = IUtils.getCenterOfBlocks(blocksEngine.blocksMap);
            blocksMoveVector = center - new IPoint(World.mapSize.x * 0.5f, World.mapSize.y * 0.5f);
            Block[,] blocks = new Block[World.mapSize.x, World.mapSize.y];
            for (int x = 0; x < World.mapSize.x; x++)
            {
                for (int y = 0; y < World.mapSize.y; y++)
                {
                    IPoint coor = new IPoint(x, y);
                    Block block = blocksEngine.getBlock(coor);
                    if (block != null)
                    {
                        IPoint newCoor = coor - blocksMoveVector;
                        block.setPosition(newCoor);
                        blocks[newCoor.x, newCoor.y] = block;
                    }
                }
            }

            blocksEngine.blocksMap = blocks;    
        }

        /// <summary>
        /// 保存船舶
        /// </summary>
        public void saveMainShip()
        {
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();
            IUtils.keyValue2Writer(writer, "sw", shipRect2.x);
            IUtils.keyValue2Writer(writer, "sh", shipRect2.y);
            IUtils.keyValue2Writer(writer, "w", shipRect2.with + 1);
            IUtils.keyValue2Writer(writer, "h", shipRect2.height + 1);
            writer.WritePropertyName("blocks");
            writer.WriteObjectStart();
            int blockCount = 0;

            for (int x = shipRect2.x; x < shipRect2.getMaxX(); x++)
                for (int y = shipRect2.y; y < shipRect2.getMaxY(); y++)
                {
                    Block block = blocksEngine.getBlock(new IPoint(x, y));
                    if (block != null && !block.isAir() && (outsideArea[x, y] == 0 || outsideArea[x, y] == 5))
                    {
                        writer.WritePropertyName("" + blockCount);
                        writer.WriteObjectStart();
                        writer = block.onWorldModeSave(writer);
                        writer.WriteObjectEnd();

                        blockCount++;
                    }
                }
            writer.WriteObjectEnd();
            IUtils.keyValue2Writer(writer, "count", blockCount);
            writer.WriteObjectEnd();
           ISecretLoad.saveMainShip(writer.ToString());
        }

        public static void changeMode(int mode)
        {
            POOLER_MODE = mode;
            modeText.text = ILang.get("mode", "menu") + ILang.get("mode" + mode, "menu");
        }

        void initOutsideArea()
        {

            for (int x = 0; x < World.mapSize.x; x++)
            {
                for (int y = 0; y < World.mapSize.y; y++)
                {
                    if (x == 0 || y == 0 || x == World.mapSize.x || y == World.mapSize.y)
                    {
                        outsideArea[x, y] = 1;
                    }
                    else
                    {
                        outsideArea[x, y] = 0;
                    }
                }
            }

            int forStack = 0;
            for (int inEnd = 0; inEnd == 0;)
            {
                inEnd = 1;
                for (int x = 0; x < World.mapSize.x; x++)
                {
                    for (int y = 0; y < World.mapSize.y; y++)
                    {
                        if (outsideArea[x, y] == 1)
                        {

                            if (x + 1 < World.mapSize.x && isOusideBlock(blocksEngine.getBlock(x + 1, y)))
                            {
                                outsideArea[x + 1, y] = 1;
                                inEnd = 0;
                            }
                            if (x - 1 > 0 && isOusideBlock(blocksEngine.getBlock(x - 1, y)))
                            {
                                outsideArea[x - 1, y] = 1;
                                inEnd = 0;
                            }
                            if (y + 1 < World.mapSize.y && isOusideBlock(blocksEngine.getBlock(x, y + 1)))
                            {
                                outsideArea[x, y + 1] = 1;
                                inEnd = 0;
                            }
                            if (y - 1 > 0 && isOusideBlock(blocksEngine.getBlock(x, y - 1)))
                            {
                                outsideArea[x, y - 1] = 1;
                                inEnd = 0;
                            }

                        }
                    }
                }
                forStack++;
                if (forStack > 50)
                {
                    break;
                }
            }

            //outsideArea:1(outside),4(border),3(water),2(cancel),5(soild),0(inside)
            seedAlgo(1, 0, 3);
            //seedAlgo(1, 2, 3);
            seedAlgo(1, 3, 4);
            seedAlgo(0, 3, 5);

        }

        bool isOusideBlock(Block block)
        {
            return block == null || block.isAir() || block.isBorder();
        }

        public void initCargoArea()
        {
            cargoArea = new int[World.mapSize.x, World.mapSize.y];

            for (int x = 0; x < World.mapSize.x; x++)
            {
                for (int y = 0; y < World.mapSize.y; y++)
                {
                    if (outsideArea[x, y] == 1)
                    {
                        cargoArea[x, y] = 0;
                    }
                    else
                    {
                        cargoArea[x, y] = 1;
                    }
                }
            }

            int forStack = 0;
            for (int inEnd = 0; inEnd == 0;)
            {
                inEnd = 1;
                for (int x = 1; x < World.mapSize.x - 1; x++)
                {
                    for (int y = 1; y < World.mapSize.y - 1; y++)
                    {
                        if (cargoArea[x, y] == 0)
                        {
                            if (isCargoOusideAreaBlock(blocksEngine.getBlock(x + 1, y)))
                            {
                                cargoArea[x + 1, y] = 0;
                                inEnd = 0;
                            }
                            if (isCargoOusideAreaBlock(blocksEngine.getBlock(x - 1, y)))
                            {
                                cargoArea[x - 1, y] = 0;
                                inEnd = 0;
                            }
                            if (isCargoOusideAreaBlock(blocksEngine.getBlock(x, y + 1)))
                            {
                                cargoArea[x, y + 1] = 0;
                                inEnd = 0;
                            }
                            if (isCargoOusideAreaBlock(blocksEngine.getBlock(x, y - 1)))
                            {
                                cargoArea[x, y - 1] = 0;
                                inEnd = 0;
                            }
                        }
                    }
                }
                forStack++;
                if (forStack > 50)
                {
                    break;
                }
            }

            Block block;
            for (int x = shipRect2.getMinX(); x < shipRect2.getMaxX(); x++)
            {
                for (int y = shipRect2.getMinY(); y < shipRect2.getMaxY(); y++)
                {                    
                    if(cargoArea[x, y] == 1)
                    {
                        block = blocksEngine.getBlock(x, y);
                        if (block != null && block.isCargohold())
                        {
                            cargoArea[x, y] = 0;
                        }
                        else
                        {
                            cargoCoors.Add(new IPoint(x, y));
                        }
                    }
                }
            }
        }


        bool isCargoOusideAreaBlock(Block block)
        {
            return block != null && !block.isCargohold();
        }

        void seedAlgo(int self, int adjoin, int set)
        {
            for (int x = 0; x < World.mapSize.x; x++)
            {
                for (int y = 0; y < World.mapSize.y; y++)
                {
                    if (outsideArea[x, y] != self)
                    {
                        continue;
                    }
                    if (x + 1 < World.mapSize.x && outsideArea[x + 1, y] == adjoin)
                    {
                        outsideArea[x, y] = set;
                        continue;
                    }
                    if (x - 1 > 0 && outsideArea[x - 1, y] == adjoin)
                    {
                        outsideArea[x, y] = set;
                        continue;
                    }
                    if (y + 1 < World.mapSize.y && outsideArea[x, y + 1] == adjoin)
                    {
                        outsideArea[x, y] = set;
                        continue;
                    }
                    if (y - 1 > 0 && outsideArea[x, y - 1] == adjoin)
                    {
                        outsideArea[x, y] = set;
                        continue;
                    }
                }
            }
        }

        void programShipArea()
        {

            shipRect1 = new IRect();
            shipRect2 = new IRect();
            for (int x = 0; x < World.mapSize.x; x++)
            {
                for (int y = 0; y < World.mapSize.y; y++)
                {
                    if (outsideArea[x, y] == 0 || outsideArea[x, y] == 5)
                    {
                        shipRect1.setMinX(x);
                        goto next1;
                    }
                }
            }

            next1:

            for (int y = 0; y < World.mapSize.y; y++)
            {
                for (int x = 0; x < World.mapSize.x; x++)
                {
                    if (outsideArea[x, y] == 0 || outsideArea[x, y] == 5)
                    {
                        shipRect1.setMinY(y);
                        goto next2;
                    }
                }
            }

            next2:

            for (int x = World.mapSize.x - 1; x > 0; x--)
            {
                for (int y = World.mapSize.y - 1; y > 0; y--)
                {
                    if (outsideArea[x, y] == 0 || outsideArea[x, y] == 5)
                    {
                        shipRect1.setMaxX(x);
                        goto next3;
                    }
                }
            }

            next3:

            for (int y = World.mapSize.y - 1; y > 0; y--)
            {
                for (int x = World.mapSize.x - 1; x > 0; x--)
                {
                    if (outsideArea[x, y] == 0 || outsideArea[x, y] == 5)
                    {
                        shipRect1.setMaxY(y);

                        shipRect2.setMinX(shipRect1.getMinX() - 4);
                        shipRect2.setMinY(shipRect1.getMinY() - 4);
                        shipRect2.setMaxX(shipRect1.getMaxX() + 5);
                        shipRect2.setMaxY(shipRect1.getMaxY() + 5);
                        return;
                    }
                }
            }
        }

        void initBackGroundPosition()
        {                                
            //waterBackGroundObject = GameObject.Find("2D WaterBackGround");
            //waterBackGroundObject.transform.localPosition = new Vector3(0, -50f + shipOffsetY, 10);
        }

        void fillShipArea()
        {
            for (int y = 0; y < World.mapSize.y; y++)
            {
                for (int x = 0; x < World.mapSize.x; x++)
                {
                    int tag = outsideArea[x, y];
                    //create border
                    if (tag == 4)
                    {
                        Block border = blocksEngine.getBlock(new IPoint(x, y));
                        if(border == null)
                        {
                            border = blocksEngine.createBlock(new IPoint(x, y), blocksManager.border);
                        }                        
                        border.getBlockObject().transform.localPosition = new Vector3(0, 9999, 0);
                    }//create water
                    else if (tag > 1 && tag < 4)
                    {

                        float blocky = new IPoint(0, y).mapIPoint2WordVector(World.mapSize).y;
                        float bgy = waterBackGroundY;
                        if (blocky > bgy)
                        {
                            blocksEngine.createBlock(new IPoint(x, y), blocksManager.air);
                        }
                        else
                        {
                            blocksEngine.createBlock(new IPoint(x, y), blocksManager.water);
                        }

                    }
                    else if ((tag == 0 || tag == 5) && blocksEngine.getBlock(x, y) == null)
                    {
                        blocksEngine.createBlock(new IPoint(x, y), blocksManager.air);
                    }
                    else if (tag == 1 && blocksEngine.getBlock(x, y) != null)
                    {
                        blocksEngine.getBlock(x, y).getBlockObject().transform.localPosition = new Vector3(0, 9999, 0);
                    }

                    if (tag == 5)
                    {
                        /*if (shipBoradPointsStack < MAX_SHIP_BORDER){
                            shipBorderPoints[shipBoradPointsStack++] = IPoint(x, y);
                        }*/
                    }
                }
            }
        }

        public int programShipTonnage()
        {

            int min_x = shipRect2.getMinX();
            int min_y = shipRect2.getMinY();
            int max_x = shipRect2.getMaxX();
            int max_y = shipRect2.getMaxY();

            int t = 0;
            for (int x = min_x; x < max_x; x++)
            {
                for (int y = min_y; y < max_y; y++)
                {

                    if (outsideArea[x, y] != 0)
                        continue;

                    Block block = blocksEngine.getBlock(x, y);
                    if (block != null && (!block.isWater()))
                    {
                        t++;
                    }
                }
            }
            return t;
        }

        void chargeAirWhenOnSuffer()
        {
            float chargeCount = (shipRect2.height - underwaterCount - 6) * 5000;
            if(chargeCount > 0)
            {
                chargeAir(chargeCount);                
            }

        }

        void steadyborderWater(IRect updataRect)
        {
            if (!isGameStart5Second)
            {
                isGameStart5Second = (int)(Time.fixedTime - startSecond) > 5;
            }

            if(!isGameStart5Second && (MainSubmarine.transform.position.y < -10000 || MainSubmarine.transform.position.y > 10000 || Mathf.Abs( MainSubmarine.rigidbody.velocity.y) > 10000))
            {
                Debug.Log("MainSubmarine.deep > 10000");
                MainSubmarine.transform.position = ISecretLoad.spwans[0];
                MainSubmarine.transform.eulerAngles = ISecretLoad.spwans[1];
                MainSubmarine.rigidbody.velocity = Vector3.zero;
                return;
            }

            underwaterCount = 0;
            IUtils.initializedArray(recordIsAddCount, false);

            blocksMapQuaternion = Quaternion.AngleAxis(MainSubmarine.transform.localEulerAngles.z, Vector3.forward);
            for (int x = updataRect.x; x < updataRect.getMaxX(); x++)
            {
                for (int y = updataRect.y; y < updataRect.getMaxY(); y++)
                {
                    if (outsideArea[x, y] == 3)
                    {
                        Block block = blocksEngine.getBlock(x, y);
                        float waterCoorPosY = (blocksMapQuaternion * block.getBlockObject().transform.position).y;
                        bool isUpWater = waterCoorPosY > WaterBackGround.waterLevelY;
                        if (isUpWater)
                        {
                            if (block.isAir())
                            {
                                
                                block.setTemperature(25);
                                block.setPress(100);
                            }
                            else
                            {
                                blocksEngine.removeBlock(new IPoint(x, y));
                                continue;
                            }
                        }
                        else
                        {
                            if(stayTemperture > 100)
                            {
                                if (block.isWaterGas())
                                {
                                    block.setTemperature(stayTemperture);
                                    block.setPress(MainSubmarine.deep + 100);                                    
                                }
                                else
                                {
                                    GasBlock gasBlock = blocksEngine.createBlock(new IPoint(x, y), BlocksManager.instance.waterGas) as GasBlock;
                                    gasBlock.setAtChildrensIndex(0);
                                }
                            }
                            else
                            {
                                if (block.isWater())
                                {                                    
                                    block.setTemperature(stayTemperture);
                                    block.setPress(MainSubmarine.deep + 100);
                                    (block as Water).realseAllCompressChild();
                                }
                                else
                                {
                                    blocksEngine.createBlock(new IPoint(x, y), BlocksManager.instance.water);                                    
                                }
                            }                           

                            if (!recordIsAddCount[y])
                            {
                                underwaterCount++;
                                recordIsAddCount[y] = true;
                            }                            
                        }

                    }
                    continue;
                }
            }          
        }

        void initMapBlocks()
        {
            for (int x = shipRect2.x; x < shipRect2.getMaxX(); x++)
            {
                for (int y = shipRect2.y; y < shipRect2.getMaxY(); y++)
                {
                    Block block = blocksEngine.getBlock(x, y);
                    if (block != null)
                    {
                        if (block.frameUpdate())
                        {
                            frameArr.Add(block);
                        }                       
                        block.onPoolerModeInitFinish();
                    }
                }
            }
        }

        

        public static void initShareMaterial()
        {
            if (shareMaterials == null)
            {
                shareMaterials = new Material[2];
                shareMaterials[0] = Resources.Load("Mats/Materials/SpriteDefault") as Material;
                shareMaterials[1] = Resources.Load("Mats/Materials/Heatmap") as Material;
            }
        }      

       

        public int getOutsideAreaTag(IPoint coor)
        {
            return outsideArea[coor.x, coor.y];
        }

        public IRect getShipRect()
        {
            return shipRect2;
        }
    }
}