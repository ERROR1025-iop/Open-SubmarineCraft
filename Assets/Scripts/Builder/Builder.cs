using LitJson;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;
using Scraft.DpartSpace;

namespace Scraft
{
    public class Builder : MonoBehaviour
    {

        public static Builder instance;

        public static string curScenceName = "Builder";
        public static int BUILDER_MODE = 0;
        public static bool IS_Can_Cover = false;
        public static bool IS_LOAD_LAST = false;

        CardManager cardManager;
        BlocksEngine blocksEngine;
        BlocksManager blocksManager;
        LoadSelector loadSelector;
        RotateSelector rotateSelector;
        BindSelector bindSelector;
        BuilderMenu builderMenu;
        IChangeImageButton coverButton;
        IChangeImageButton dpartViewButton;
        public BuilderSetting builderSetting;
        SVSelector sVSelector;
        BuilderPartsSelector bpSelector;

        AssemblerLoader dpartsLoaderSelector;
        DpartsManager dpartsManager;
        DpartsEngine dpartsEngine;

        InputField inputField;
        [HideInInspector] public GameObject partsParentObject;
        Text subNameText;
        static Text modeText;
        static Text coverText;
        public static bool isLoadParts;
        string subName;
        IRect shipRect;

        Text selectBlockText;
        public Block selectBlock;

        void OnEnable()
        {
            instance = this;
            blocksEngine = BlocksEngine.instance;
            blocksManager = BlocksManager.instance;
            dpartsManager = DpartsManager.instance;
            dpartsEngine = new DpartsEngine(World.dpartParentObject);

            blocksManager.loadStationInfos();

            GameObject.Find("Canvas/bander/save").GetComponent<Button>().onClick.AddListener(onSaveButtonClick);
            GameObject.Find("Canvas/bander/load").GetComponent<Button>().onClick.AddListener(onLoadButtonClick);
            GameObject.Find("Canvas/bander/parts").GetComponent<Button>().onClick.AddListener(onPartsButtonClick);
            GameObject.Find("Canvas/bander/dparts").GetComponent<Button>().onClick.AddListener(onDpartsButtonClick);
            GameObject.Find("Canvas/btns gorup/del").GetComponent<Button>().onClick.AddListener(onDelButtonClick);
            GameObject.Find("Canvas/bander/run").GetComponent<Button>().onClick.AddListener(onRunButtonClick);
            GameObject.Find("Canvas/btns gorup/bind").GetComponent<Button>().onClick.AddListener(onBindButtonClick);
            GameObject.Find("Canvas/bander/menu").GetComponent<Button>().onClick.AddListener(onMenuButtonClick);

            coverButton = GameObject.Find("Canvas/btns gorup/cover").GetComponent<IChangeImageButton>();
            coverText = GameObject.Find("Canvas/cover text").GetComponent<Text>();
            dpartViewButton = GameObject.Find("Canvas/bander/dpart-view").GetComponent<IChangeImageButton>();
            inputField = GameObject.Find("Canvas/InputField").GetComponent<InputField>();
            subNameText = GameObject.Find("Canvas/InputField/Text").GetComponent<Text>();
            modeText = GameObject.Find("Canvas/mode").GetComponent<Text>();
            selectBlockText = GameObject.Find("Canvas/select block text").GetComponent<Text>();
            partsParentObject = GameObject.Find("Parts Map");

            coverButton.addListener(onCoverButtonClick);
            coverText.text = ILang.get(IS_Can_Cover ? "can cover" : "can not cover");
            dpartViewButton.addListener(onDpartViewButtonClick);

            cardManager = new CardManager(blocksManager);
            loadSelector = new LoadSelector();
            rotateSelector = new RotateSelector();
            bindSelector = new BindSelector();
            builderMenu = new BuilderMenu();
            builderSetting = new BuilderSetting();
            sVSelector = new SVSelector();
            bpSelector = new BuilderPartsSelector();             
            dpartsLoaderSelector = new AssemblerLoader();

            setSelectBlock(null);

            if (IS_LOAD_LAST)
            {
                this.subName = World.mapName;
                inputField.text = subName;
                loadBlocks("Last Ship");
            }

            loadDparts("Last Ship");
            isLoadParts = false;

            World.activeCamera = 0;
            Pooler.HeatMap_Mode = 0;
            Pooler.initShareMaterial();
            Time.timeScale = 1;
            MainSubmarine.lightLevel = 5;
            MainSubmarine.lightColor = Color.white;

            UnityAndroidEnter.CallCloseInterstitialAD();
        }       

        public void Update()
        {           
            loadSelector.updata();
        }

        void onSaveButtonClick()
        {
            subName = (subNameText.text.Equals("") ? "unname" : subNameText.text);
            string path = GamePath.shipsFolder + subName + ".ship";
            if (File.Exists(path))
            {
                IConfigBox.instance.show(ILang.get("The save has already existed, is it covered?", "menu"), onSaveConfigButtonClick, null);
            }
            else
            {
                onSaveConfigButtonClick();
            }
        }

        void onSaveConfigButtonClick()
        {
            subName = (subNameText.text.Equals("") ? "unname" : subNameText.text);
            World.mapName = subName;
            IToast.instance.show("Saving");
            string shipData = saveBlocks(subName);
            createThumbnailFile(shipData, subName);
            IToast.instance.show("Saved", 100);
        }

        public string saveBlocks(string folder, string fileName)
        {
            string path = folder + fileName + ".ship";
            shipRect = programShipArea();
            string shipData = serializeShip().ToString();
            IUtils.write2txt(path, shipData);
            return shipData;
        }

        string saveBlocks(string fileName)
        {
            return saveBlocks(GamePath.shipsFolder, fileName);
        }

        public JsonWriter serializeShip()
        {

            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();

            IUtils.keyValue2Writer(writer, "sw", shipRect.x);
            IUtils.keyValue2Writer(writer, "sh", shipRect.y);
            IUtils.keyValue2Writer(writer, "w", shipRect.with + 1);
            IUtils.keyValue2Writer(writer, "h", shipRect.height + 1);
            writer.WritePropertyName("blocks");
            writer.WriteObjectStart();

            int blockCount = 0;

            for (int x = 0; x < World.mapSize.x; x++)
            {
                for (int y = 0; y < World.mapSize.y; y++)
                {
                    Block block = blocksEngine.getBlock(new IPoint(x, y));
                    if (block != null)
                    {
                        writer.WritePropertyName("" + blockCount);
                        writer.WriteObjectStart();
                        writer = block.onWorldModeSave(writer);
                        writer.WriteObjectEnd();

                        blockCount++;
                    }
                }
            }
            writer.WriteObjectEnd();
            IUtils.keyValue2Writer(writer, "count", blockCount);
            writer.WriteObjectEnd();

            return writer;
        }

        void createThumbnailFile(string shipData, string filename)
        {
            Texture2D texture2D = createThumbnailTexture2D(JsonMapper.ToObject(shipData));
            IUtils.saveTexture2D2SD(texture2D, GamePath.builderThumbnailFolder + filename + ".thu");
        }

        public Texture2D createThumbnailTexture2D(JsonData mapData)
        {
            int blocksCount = IUtils.getJsonValue2Int(mapData, "count");
            int sx = IUtils.getJsonValue2Int(mapData, "sw");
            int sy = IUtils.getJsonValue2Int(mapData, "sh");
            int w = IUtils.getJsonValue2Int(mapData, "w");
            int h = IUtils.getJsonValue2Int(mapData, "h");

            if (w == 0 || h == 0)
            {
                IToast.instance.show(ILang.get("createThumbnail error"), 50);
                return null;
            }

            IPoint sCoor = new IPoint(sx, sy);
            JsonData blocksArrData = mapData["blocks"];
            Color[] rgbdata = new Color[w * h];
            for (int i = 0; i < blocksCount; i++)
            {
                JsonData blockData = blocksArrData[i];
                int blockId = IUtils.getJsonValue2Int(blockData, "id");
                int x = IUtils.getJsonValue2Int(blockData, "x");
                int y = IUtils.getJsonValue2Int(blockData, "y");
                IPoint coor = new IPoint(x, y);
                IPoint bmpCoor = coor - sCoor;
                Block block = BlocksManager.instance.getBlockById(blockId);
                rgbdata[bmpCoor.y * w + bmpCoor.x] = block.getThumbnailColor(blockData);
            }
            Texture2D texture2D = IBmp.getDataPicture(w, h, rgbdata);
            texture2D.filterMode = FilterMode.Point;
            return texture2D;
        }

        IRect programShipArea()
        {
            IRect shipRect = new IRect();
            for (int x = 0; x < World.mapSize.x; x++)
            {
                for (int y = 0; y < World.mapSize.y; y++)
                {
                    if (blocksEngine.getBlock(new IPoint(x, y)) != null)
                    {
                        shipRect.setMinX(x);
                        goto next1;
                    }
                }
            }

            next1:

            for (int y = 0; y < World.mapSize.y; y++)
            {
                for (int x = 0; x < World.mapSize.x; x++)
                {
                    if (blocksEngine.getBlock(new IPoint(x, y)) != null)
                    {
                        shipRect.setMinY(y);
                        goto next2;
                    }
                }
            }

            next2:

            for (int x = World.mapSize.x - 1; x > 0; x--)
            {
                for (int y = World.mapSize.y - 1; y > 0; y--)
                {
                    if (blocksEngine.getBlock(new IPoint(x, y)) != null)
                    {
                        shipRect.setMaxX(x);
                        goto next3;
                    }
                }
            }

            next3:

            for (int y = World.mapSize.y - 1; y > 0; y--)
            {
                for (int x = World.mapSize.x - 1; x > 0; x--)
                {
                    if (blocksEngine.getBlock(new IPoint(x, y)) != null)
                    {
                        shipRect.setMaxY(y);
                        return shipRect;
                    }
                }
            }
            return new IRect(0, 0, 1, 1);
        }

        void onLoadButtonClick()
        {
            if (isLoadParts)
            {
                return;
            }

            IToast.instance.show("Loading");
            if (Directory.Exists(GamePath.shipsFolder))
            {
                DirectoryInfo direction = new DirectoryInfo(GamePath.shipsFolder);
                FileInfo[] folders = direction.GetFiles("*.ship", SearchOption.TopDirectoryOnly);

                loadSelector.isLoadPart = false;
                loadSelector.setFile(folders);
                loadSelector.show();
            }
            IToast.instance.hide();
        }

        void onPartsButtonClick()
        {
            if (isLoadParts)
            {
                return;
            }

            IToast.instance.show("Loading");
            if (Directory.Exists(GamePath.shipsFolder))
            {
                DirectoryInfo direction = new DirectoryInfo(GamePath.shipsFolder);
                FileInfo[] folders = direction.GetFiles("*.ship", SearchOption.TopDirectoryOnly);

                loadSelector.isLoadPart = true;
                loadSelector.setFile(folders);
                loadSelector.show();
            }
            IToast.instance.hide();
        }

        public void loadParts(string subName)
        {
            isLoadParts = true;
            bpSelector.show(true, subName);
            string path = GamePath.shipsFolder + subName + ".ship";
            JsonData jsonData = JsonMapper.ToObject(IUtils.readFromTxt(path));

            int blocksCount = IUtils.getJsonValue2Int(jsonData, "count");
            JsonData blocksArrData = jsonData["blocks"];

            bool isHasLockBlock = false;

            for (int i = 0; i < blocksCount; i++)
            {
                JsonData blockData = blocksArrData[i];

                int blockId = IUtils.getJsonValue2Int(blockData, "id");
                if (GameSetting.isCareer && !blocksManager.getIsUnlock(blockId))
                {
                    isHasLockBlock = true;
                    continue;
                }
                int x = IUtils.getJsonValue2Int(blockData, "x");
                int y = IUtils.getJsonValue2Int(blockData, "y");

                IPoint coor = new IPoint(x, y);

                Block blockStatic = blocksManager.getBlockById(blockId);
                if (blockStatic == null)
                {
                    continue;
                }
                Block block = blockStatic.clone(partsParentObject, blocksManager, null, World.mapSize);
                block.onWorldModeLoad(blockData, coor);
                block.setPosition(coor);

                if (isHasLockBlock)
                {
                    IToast.instance.show("Ununlocked blocks in the save have been automatically deleted", 100);
                }
            }
        }

        void onDpartsButtonClick()
        {
            if (isLoadParts)
            {
                return;
            }

            IToast.instance.show("Loading");
            if (Directory.Exists(GamePath.shipsFolder))
            {
                DirectoryInfo direction = new DirectoryInfo(GamePath.dpartFolder);
                FileInfo[] folders = direction.GetFiles("*.ass", SearchOption.TopDirectoryOnly);

                dpartsLoaderSelector.isLoadPart = false;
                dpartsLoaderSelector.setFile(folders);
                dpartsLoaderSelector.show();
            }
            IToast.instance.hide();
        }

        public void onLoadAssemblerMap(string dpartName)
        {
            IToast.instance.show("Loading");
            loadDparts(dpartName);
            IToast.instance.hide();
        }

        void loadDparts(string dpartName)
        {
            World.dpartName = dpartName;
            dpartsEngine.removeAllDpart();
            IUtils.deleteAllChildrens(dpartsEngine.dpartParentObject.transform);
            string path = GamePath.dpartFolder + dpartName + ".ass";

            if (!File.Exists(path))
            {
                Debug.Log(path + " is Not Exists");
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(IUtils.readFromTxt(path));
            int blocksCount = IUtils.getJsonValue2Int(jsonData, "count");
            JsonData blocksArrData = jsonData["dparts"];

            JsonData dpartData;
            Dpart dpart;

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
                dpart.onBuilderModeCreate();
            }

            IUtils.centerOnChildrens(dpartsEngine.dpartParentObject);
        }

        void onCoverButtonClick()
        {
            IS_Can_Cover = coverButton.value;
            coverText.text = ILang.get(IS_Can_Cover ? "can cover" : "can not cover");
        }

        public void loadBuilderMap(string subName)
        {
            this.subName = subName;
            World.mapName = subName;
            inputField.text = subName;
            IToast.instance.show("Loading");
            loadBlocks(subName);
            IToast.instance.hide();
        }


        public void drawPartsToMap(string subName)
        {

            Vector3 partsPos = partsParentObject.transform.localPosition;
            IPoint offset = new IPoint(partsPos.x * 6.25f, partsPos.y * 6.25f);

            string path = GamePath.shipsFolder + subName + ".ship";
            JsonData jsonData = JsonMapper.ToObject(IUtils.readFromTxt(path));

            int blocksCount = IUtils.getJsonValue2Int(jsonData, "count");
            JsonData blocksArrData = jsonData["blocks"];

            for (int i = 0; i < blocksCount; i++)
            {
                JsonData blockData = blocksArrData[i];

                int blockId = IUtils.getJsonValue2Int(blockData, "id");
                int x = IUtils.getJsonValue2Int(blockData, "x");
                int y = IUtils.getJsonValue2Int(blockData, "y");

                IPoint coor = new IPoint(x, y) + offset;
                if (!isAtMap(coor))
                {
                    continue;
                }
                Block blockStatic = blocksManager.getBlockById(blockId);
                if (blockStatic == null)
                {
                    continue;
                }
                Block block = blockStatic.clone(blocksEngine.blocksParentObject, blocksManager, null, World.mapSize);
                block.onWorldModeLoad(blockData, coor);
                block.setPosition(coor);
                Block orgBlock = blocksEngine.getBlock(coor);
                BlocksManager.instance.addConsumeCargoCount(block, 1);
                if (orgBlock != null)
                {
                    delBlock(coor);
                }
                blocksEngine.setBlock(coor, block);                
            }
            CardManager.instance.updatecargoCount();
        }

        public void clearPartsMap()
        {
            isLoadParts = false;
            changeMode(1);
            for (int i = partsParentObject.transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(partsParentObject.transform.GetChild(i).gameObject);
            }
        }

        void loadBlocks(string subName)
        {
            clearMap();
            string path = GamePath.shipsFolder + subName + ".ship";
            JsonData jsonData = JsonMapper.ToObject(IUtils.readFromTxt(path));

            int blocksCount = IUtils.getJsonValue2Int(jsonData, "count");
            JsonData blocksArrData = jsonData["blocks"];

            bool isHasLockBlock = false;

            for (int i = 0; i < blocksCount; i++)
            {
                JsonData blockData = blocksArrData[i];

                int blockId = IUtils.getJsonValue2Int(blockData, "id");
                if (GameSetting.isCareer && !blocksManager.getIsUnlock(blockId))
                {
                    isHasLockBlock = true;
                    continue;
                }
                int x = IUtils.getJsonValue2Int(blockData, "x");
                int y = IUtils.getJsonValue2Int(blockData, "y");

                IPoint coor = new IPoint(x, y);
                if (!isAtMap(coor))
                {
                    continue;
                }
                Block blockStatic = blocksManager.getBlockById(blockId);
                if (blockStatic == null)
                {
                    continue;
                }
                Block block = blockStatic.clone(blocksEngine.blocksParentObject, blocksManager, null, World.mapSize);
                block.onWorldModeLoad(blockData, coor);
                block.openHeatMapMode(false);
                block.setPosition(coor);
                blocksEngine.setBlock(coor, block);               
                BlocksManager.instance.addConsumeCargoCount(block, 1);                
            }
            if (isHasLockBlock)
            {
                IToast.instance.show("Ununlocked blocks in the save have been automatically deleted", 100);
            }
            CardManager.instance.updatecargoCount();
        }


        public void clearMap()
        {
            for (int x = 0; x < World.mapSize.x; x++)
            {
                for (int y = 0; y < World.mapSize.y; y++)
                {
                    IPoint coor = new IPoint(x, y);
                    delBlock(coor);
                }
            }
            blocksEngine.clearUnusedBlocksPool();
            setSelectBlock(null);

        }

        void onDelButtonClick()
        {
            changeMode(3);
        }

        void onBindButtonClick()
        {
            changeMode(4);
        }

        void onMenuButtonClick()
        {
            subName = (subNameText.text.Equals("") ? "unname" : subNameText.text);
            World.mapName = subName;
            builderMenu.show(true);
        }

        void onDpartViewButtonClick()
        {
            if (dpartViewButton.value)
            {
                Camera.main.cullingMask |= (1 << 8);
            }
            else
            {
                Camera.main.cullingMask &= ~(1 << 8);
            }
        }

        public void setDpartViewOpen(bool isOpen)
        {
            dpartViewButton.setValue(isOpen);
            onDpartViewButtonClick();
        }

        public void onBlockCreate(Block block, IPoint coor)
        {
            block.onBuilderModeCreated();
            sVSelector.returnLastSettingValue(block as SolidBlock);
            bindSelector.returnLastBindValue(block as SolidBlock);

            if (selectBlock != null)
            {
                selectBlock.setBuilderModeHightLight(false);
            }


        }

        public void delBlock(IPoint coor)
        {
            Block block = blocksEngine.getBlock(coor);
            if (block != null)
            {
                block.onBuilderModeDeleted();                
                block.clear(true);
                blocksEngine.setBlock(coor, null);
                setSelectBlock(null);           
            }
        }

        public static void changeMode(int mode, string str)
        {
            BUILDER_MODE = mode;
            modeText.text = ILang.get("mode", "menu") + ":" + str;
        }

        public static void changeMode(int mode)
        {
            BUILDER_MODE = mode;
            modeText.text = ILang.get("mode", "menu") + ":" + ILang.get("mode" + mode, "menu");
        }

        public void onBlockClick(Block block, IPoint coor)
        {
            setSelectBlock(block);
        }        

        void setSelectBlock(Block block)
        {
            if (block != null)
            {
                if (selectBlock != null && selectBlock.isNeedDelete() == false)
                {
                    selectBlock.setBuilderModeHightLight(false);
                }
                selectBlock = block;
                selectBlock.setBuilderModeHightLight(true);
                selectBlockText.text = ILang.get("current select", "menu") + ":" + block.getLangName();

                rotateSelector.show(selectBlock.isCanRotate());
                bindSelector.show(selectBlock.isCanBind());
                sVSelector.show(selectBlock.isCanSettingValue() != -1);
            }
            else
            {
                selectBlock = null;
                selectBlockText.text = "";
                rotateSelector.show(false);
                bindSelector.show(false);
                sVSelector.show(false);
            }

        }

        public bool isContainBlocks(IRect rect, IPoint exceptPoint)
        {
            for (int x = rect.x; x < rect.getMaxX(); x++)
            {
                for (int y = rect.y; y < rect.getMaxY(); y++)
                {
                    if (new IPoint(x, y).equal(exceptPoint))
                    {
                        continue;
                    }
                    if (blocksEngine.getBlock(x, y) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        int programWirelessCount()
        {
            int count = 0;
            for (int x = 0; x < World.mapSize.x; x++)
            {
                for (int y = 0; y < World.mapSize.y; y++)
                {
                    Block block = blocksEngine.getBlock(x, y);
                    if (block != null && block.equalBlock(blocksManager.wireless))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void onRunButtonClick()
        {
            int count = programWirelessCount();
            if (count <= 0)
            {
                IToast.instance.show("Please place at least one wirless", 100);
            }
            else
            {
                IToast.instance.show("Loading");
                string backgroundPath = string.Format("Menu/Loading/{0}{1}", GameSetting.isCreateAi ? "n" : "s", (int)(Random.value * 2.9f));
                AsyncLoadScene.sprite = Resources.Load(backgroundPath, typeof(Sprite)) as Sprite;
                UnityAndroidEnter.CallShowInterstitialAD();
                string shipData = saveBlocks("Last Ship");
                createThumbnailFile(shipData, "Last Ship");
                bool isGraphConnected = new DFSGraph().start(blocksEngine.blocksMap, shipRect);
                if (!isGraphConnected)
                {
                    IToast.instance.show("Please connect all the blocks.", 100);
                    return;
                }
                int negativeBlockId = checkHasNegativeCargoCounts();
                if (negativeBlockId != -1)
                {
                    IToast.instance.showWithoutILang(string.Format("{0}({1}){2}", ILang.get("Inadequate blocks"), blocksManager.getBlockById(negativeBlockId).getLangName(), ILang.get(", Unable to build.")), 100);
                    return;
                }               
                Pooler.mainStationRemaindeCargoCounts = blocksManager.getRemaindeCargoCounts();
                saveRemaindeCargoCounts();
                FileInfo dpartLastFile = new FileInfo(GamePath.dpartFolder + "Last Ship.ass");
                IUtils.renameFileByCopy(GamePath.dpartFolder, World.dpartName + ".ass", "Last Ship.ass", true);
                Pooler.IS_Form_StationCenter = false;
                if(ITutorial.tutorialStep >= 0)
                {
                    ITutorial.tutorialStep++;
                }
                if (GameSetting.isAndroid)
                {
                    AsyncLoadScene.asyncloadScene("Pooler");
                }
                else
                {
                    Application.LoadLevel("Pooler");
                }
                
            }
        }


        /// <summary>
        ///检查货舱是否有存量。有存量返回-1，否则返回缺货方块ID
        /// </summary>
        int checkHasNegativeCargoCounts()
        {
            if (GameSetting.isCareer)
            {
                bool isNegative = false;
                int count = blocksManager.getBlockCount();
                for (int i=0;i < count; i++)
                {
                    if(blocksManager.getMainStationCargoCount(blocksManager.getBlockById(i)) < 0)
                    {
                        return i;
                    }
                }                
            }
            return -1;
        }

        void saveRemaindeCargoCounts()
        {
            if (GameSetting.isCareer && !GameSetting.isAndroid)
            {
                JsonWriter writer = new JsonWriter();
                writer.WriteObjectStart();
                int count = Pooler.mainStationRemaindeCargoCounts.Length;
                IUtils.keyValue2Writer(writer, "count", count);
                for (int i = 0; i < count; i++)
                {
                    IUtils.keyValue2Writer(writer, i.ToString(), Pooler.mainStationRemaindeCargoCounts[i]);
                }
                writer.WriteObjectEnd();
                IUtils.write2txt(GamePath.cacheFolder + "remaindeCargo.txt", writer.ToString());
            }
        }

        bool isAtMap(IPoint coor)
        {
            return (new IRect(7, 7, blocksEngine.mapSize.x - 14, blocksEngine.mapSize.y - 14).containsPoint(coor));
        }
    }
}