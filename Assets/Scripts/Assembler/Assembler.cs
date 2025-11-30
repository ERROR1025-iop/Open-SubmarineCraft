using LitJson;
using Scraft.DpartSpace;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class Assembler : MonoBehaviour
    {
        public static Assembler instance;

        public static bool IS_LOAD_LAST = false;
        public static int ASSEMBLER_PLACE_MODE = 0;
        public static int ASSEMBLER_GROUP_MODE = 0;
        public static bool IS_UnitSnapping = false;
        public static bool IS_FORM_POOLER = false;
        public static bool IS_Show_WeighCenter = true;
       

        DpartsManager dpartsManager;
        DpartCardManager dpartCardManager;
        static public DpartsEngine dpartsEngine;
        public DpartAttribute dpartAttribute;
        public AssemblerMenu assemblerMenu;
        public AssemblerSetting assemblerSetting;
        public AssemblerRuntimeBander assemblerRuntimeBander;
        public AssemblerPlaceMode assemblerPlaceMode;
        AssemblerLoader loadSelector;
        GroupDpartsSelector groupDpartsSelector;
        CustomDpartsSelector customDpartsSelector;
        public AssemblerSenceSelector assemblerSenceSelector;
        AssemblerAddCustomBox assemblerAddCustomBox;
        AssemblerAddCustomCardBox assemblerAddCustomCardBox;

        IChangeImageButton changeTextButton;
        IChangeImageButton hierarchyButton;
        IChangeImageButton weightCenterButton;

        public Camera dpart3DCamera;
        public Camera shotCamera;
        static public Camera preShotCamera;
        static public GameObject preShotParent;
        public Transform dpartPlane;
        InputField inputField;
        Text dpartNameText;

        static Text modeText;
        public static bool isLoadParts;
        int dpartsCount;
        Texture2D dpartsScrenshotTexture;
        float planeOffset;
        public static float massOffset;

        void Awake()
        {
            instance = this;

            dpartsManager = DpartsManager.instance;
            dpartsEngine = new DpartsEngine(World.dpartParentObject);         
           

            new IRT();

            GameObject.Find("Canvas/bander/save").GetComponent<Button>().onClick.AddListener(onSaveButtonClick);
            GameObject.Find("Canvas/bander/load").GetComponent<Button>().onClick.AddListener(onLoadButtonClick);
            GameObject.Find("Canvas/bander/parts").GetComponent<Button>().onClick.AddListener(onPartsButtonClick);
            GameObject.Find("Canvas/bander/run").GetComponent<Button>().onClick.AddListener(onRunButtonClick);           
            GameObject.Find("Canvas/bander/menu").GetComponent<Button>().onClick.AddListener(onMenuButtonClick);

            changeTextButton = GameObject.Find("Canvas/bander/change").GetComponent<IChangeImageButton>();
            hierarchyButton = GameObject.Find("Canvas/bander/showHierarchy").GetComponent<IChangeImageButton>();
            weightCenterButton = GameObject.Find("Canvas/bander/showWeightCenter").GetComponent<IChangeImageButton>();

            changeTextButton.addListener(onChangeTextButtonClick);
            hierarchyButton.addListener(onHierarchyButtonClick);
            weightCenterButton.addListener(onWeightCenterButtonClick);           
            IS_Show_WeighCenter = weightCenterButton.value;

            Transform dpartsSelectorBottom = GameObject.Find("Canvas/dparts selector bottom").transform;
            dpartsSelectorBottom.GetChild(0).GetComponent<Button>().onClick.AddListener(onSelectorBaseButtonClick);
            dpartsSelectorBottom.GetChild(1).GetComponent<Button>().onClick.AddListener(onSelectorGroupButtonClick);
            dpartsSelectorBottom.GetChild(2).GetComponent<Button>().onClick.AddListener(onSelectorCustomButtonClick);
            dpartsSelectorBottom.GetChild(3).GetComponent<Button>().onClick.AddListener(onSelectorAddButtonClick);

            modeText = GameObject.Find("Canvas/mode").GetComponent<Text>();
            dpart3DCamera = Camera.main;
            shotCamera = GameObject.Find("ShotCamera").GetComponent<Camera>();
            preShotCamera = GameObject.Find("PreShotCamera").GetComponent<Camera>();
            preShotParent = GameObject.Find("PreShotParent");
            inputField = GameObject.Find("Canvas/InputField").GetComponent<InputField>();
            dpartNameText = GameObject.Find("Canvas/InputField/Text").GetComponent<Text>();

            dpartCardManager = new DpartCardManager(dpartsManager, dpartsEngine);
            dpartAttribute = new DpartAttribute();
            assemblerMenu = new AssemblerMenu();
            assemblerSetting = new AssemblerSetting();
            assemblerAddCustomBox = new AssemblerAddCustomBox();
            assemblerAddCustomCardBox = new AssemblerAddCustomCardBox();
            assemblerRuntimeBander = new AssemblerRuntimeBander();
            assemblerPlaceMode = new AssemblerPlaceMode();
            loadSelector = new AssemblerLoader();
            groupDpartsSelector = new GroupDpartsSelector();
            customDpartsSelector = new CustomDpartsSelector();
            assemblerSenceSelector = new AssemblerSenceSelector();

            shotCamera.enabled = false;
            preShotCamera.enabled = false;

            Camera.main.renderingPath = GameSetting.renderMode == 2 ? RenderingPath.DeferredShading : RenderingPath.Forward;

            Time.timeScale = 1;
            World.activeCamera = 0;
            Pooler.HeatMap_Mode = 0;

            planeOffset = 0;
        }

        void Start()
        {
            changeTextButton.setValue(GameSetting.isAssemblerShowText);

            if (IS_LOAD_LAST)
            {
                inputField.text = World.dpartName;
                loadDparts("Last Ship");
            }           
        }

        void onSelectorBaseButtonClick()
        {
            groupDpartsSelector.show(false);
            customDpartsSelector.show(false);
            ASSEMBLER_GROUP_MODE = 0;
        }

        void onSelectorGroupButtonClick()
        {
            groupDpartsSelector.show(true);
            customDpartsSelector.show(false);
            ASSEMBLER_GROUP_MODE = 1;
        }

        void onSelectorCustomButtonClick()
        {
            groupDpartsSelector.show(false);
            customDpartsSelector.show(true);
            ASSEMBLER_GROUP_MODE = 2;
        }

        void onSelectorAddButtonClick()
        {
            GameObject[] gos = IRT.Selection.gameObjects;
            if (gos != null && gos.Length > 0)
            {
                assemblerAddCustomBox.show(true);
                return;
            }
            return;
        }

        void onSaveButtonClick()
        {

            World.dpartName = (dpartNameText.text.Equals("") ? "unname" : dpartNameText.text);
            IToast.instance.show("Saving");
            string shipData = saveDpart(World.dpartName);
            createThumbnailFile(World.dpartName, GamePath.assemblerThumbnailFolder);
            IToast.instance.show("Saved", 100);
        }



        public string saveDpart(string folder, string fileName)
        {
            string path = folder + fileName + ".ass";
            string shipData = serializeDpart().ToString();
            IUtils.write2txt(path, shipData);
            return shipData;
        }

        public string saveDpart(string fileName)
        {
            return saveDpart(GamePath.dpartFolder, fileName);
        }


        public JsonWriter serializeDpart()
        {
            JsonWriter writerDparts = new JsonWriter();
            Dpart dpart;
            dpartsCount = 0;

            writerDparts.WriteObjectStart();
            writerDparts.WritePropertyName("dparts");
            writerDparts.WriteObjectStart();

            for (int i = 0; i < DpartsEngine.Max_Dpart_Count; i++)
            {
                if (dpartsEngine.dpartArr[i] != null)
                {
                    dpart = dpartsEngine.dpartArr[i];
                    GameObject go = dpart.getGameObject();
                    if (go != null && dpart.getGameObject().activeSelf)
                    {
                        if (dpart.mirrorDpart != null && i > dpart.mirrorDpart.getIdentifyId())
                        {
                            continue;
                        }
                        writerDparts.WritePropertyName("" + dpartsCount);
                        writerDparts.WriteObjectStart();
                        writerDparts = dpart.onBuilderModeSave(writerDparts);
                        writerDparts.WriteObjectEnd();
                        dpartsCount++;
                    }
                }
            }

            writerDparts.WriteObjectEnd();
            IUtils.keyValue2Writer(writerDparts, "count", dpartsCount);
            IUtils.keyValue2Writer(writerDparts, "plane", planeOffset);
            IUtils.keyValue2Writer(writerDparts, "mass", massOffset);
            writerDparts.WriteObjectEnd();

            return writerDparts;
        }

        public void createThumbnailFile(string filename, string saveFolder)
        {
            AssemblerUtils.createGameObjectThumbnailImage(dpartsEngine.dpartParentObject, shotCamera, saveFolder + filename + ".thu", new Rect(0, 0, 800, 480));
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
                DirectoryInfo direction = new DirectoryInfo(GamePath.dpartFolder);
                FileInfo[] folders = direction.GetFiles("*.ass", SearchOption.TopDirectoryOnly);

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
                DirectoryInfo direction = new DirectoryInfo(GamePath.dpartFolder);
                FileInfo[] folders = direction.GetFiles("*.ass", SearchOption.TopDirectoryOnly);

                loadSelector.isLoadPart = true;
                loadSelector.setFile(folders);
                loadSelector.show();
            }
            IToast.instance.hide();
        }

        void onChangeTextButtonClick()
        {
            GameSetting.isAssemblerShowText = changeTextButton.value;
            GameSetting.save();
            ISwitchImageTextButton.setGlobalShow(GameSetting.isAssemblerShowText);
        }

        void onHierarchyButtonClick()
        {
            AssemblerHierarchy.instance.setShow(hierarchyButton.value);
        }

        void onWeightCenterButtonClick()
        {
            IS_Show_WeighCenter = weightCenterButton.value;
        }

        public void onRunButtonClick()
        {
            if (IS_FORM_POOLER)
            {
                assemblerSenceSelector.show(true);
            }
            else
            {
                assemblerSenceSelector.onBuilderButtonClick();
            }

        }

        void onMenuButtonClick()
        {
            assemblerMenu.show(true);
        }

        public void onLoadAssemblerMap(string dpartName)
        {
            World.dpartName = dpartName;
            inputField.text = dpartName;
            IToast.instance.show("Loading");
            loadDparts(dpartName);
            IToast.instance.hide();
        }

        void loadDparts(string dpartName)
        {
            setPlaneOffset(0);
            dpartsEngine.removeAllDpart();
            string path = GamePath.dpartFolder + dpartName + ".ass";

            if (!File.Exists(path))
            {
                return;
            }

            JsonData jsonData = JsonMapper.ToObject(IUtils.readFromTxt(path));

            int blocksCount = IUtils.getJsonValue2Int(jsonData, "count");          
            setPlaneOffset(IUtils.getJsonValue2Float(jsonData, "plane", 0));
            setMassOffset(IUtils.getJsonValue2Float(jsonData, "mass", 0));
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
        }



        public void loadParts(string dpartName)
        {
            DpartCardManager.selectDPartStatic = null;
            string path = GamePath.dpartFolder + dpartName + ".ass";
            JsonData jsonData = JsonMapper.ToObject(IUtils.readFromTxt(path));

            int blocksCount = IUtils.getJsonValue2Int(jsonData, "count");
            JsonData blocksArrData = jsonData["dparts"];

            JsonData dpartData;
            Dpart dpart;

            GameObject[] loadObject = new GameObject[blocksCount];

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
                loadObject[i] = dpart.getGameObject();
                dpartAttribute.show(true);
            }

            IRT.Selection.objects = loadObject;
        }

        public static void changePlaceMode(int mode)
        {
            changePlaceMode(mode, ILang.get("assmode" + mode));
        }

        public static void changePlaceMode(int mode, string str)
        {
            ASSEMBLER_PLACE_MODE = mode;
            modeText.text = str;
        }

        public void setPlaneOffset(float y)
        {
            planeOffset = y;
            dpartPlane.localPosition = new Vector3(0, planeOffset, 0);
            AssemblerInput.dragPlane = new Plane(Vector3.up, dpartPlane.localPosition);
        }

        public void setMassOffset(float y)
        {
            massOffset = y;            
        }

        void Update()
        {           
            loadSelector.updata();
            assemblerRuntimeBander.initRTHandle();
            groupDpartsSelector.updata();
            customDpartsSelector.updata();
        }
    }

}