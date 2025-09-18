using Scraft.BlockSpace;
using UnityEngine;
using UnityEngine.EventSystems;
using Scraft.StationSpace;

namespace Scraft
{
    public class PoolerInput : MonoBehaviour
    {

        World world;
        BlocksEngine blocksEngine;

        Camera Camera3DWorld;
        Camera Camera2DWorld;

        Transform blocksParent;

        Vector3 lastPonitPos;
        Vector3 lastMapPos;

        static public bool isDebugMode;
        static public bool isOpenAcc;

        public bool openDebug;
        public float addTemperature = 500;
        public float addPress = 300;

        IChangeImageButton debugButton;
        IChangeImageButton accButton;

        PoolerSVSelector sVSelector;
        ScientificSelector scientificSelector;

        public Block selectBlock;

        static public int collider2DLayer;
        static public int collider3DLayer;
        static public int colliderTerrainLayer;


        static public string stationTag;
        static public string preloadShipTag;

        float clickTime;

        void Start()
        {
            world = World.instance;
            blocksEngine = world.blocksEngine;

            sVSelector = new PoolerSVSelector(this);
            scientificSelector = new ScientificSelector(this);

            Camera3DWorld = Camera.main;
            Camera2DWorld = GameObject.Find("2D Camera").GetComponent<Camera>();

            blocksParent = GameObject.Find("2D Builder Map").transform;

            lastMapPos = Camera2DWorld.transform.localPosition;
            lastPonitPos = Vector3.zero;

            debugButton = GameObject.Find("Canvas/debug").GetComponent<IChangeImageButton>();
            accButton = GameObject.Find("Canvas/radar rect/radar text rect/acc").GetComponent<IChangeImageButton>();
            debugButton.addListener(onDebugButtonClick);
            accButton.addListener(onAccButtonClick);

            isOpenAcc = false;
            isDebugMode = false;

            collider2DLayer = 1 << 13;
            collider3DLayer = 1 << 8;
            colliderTerrainLayer = 1 << 10;

            stationTag = "station";
            preloadShipTag = "preload ship";

        }

        void onDebugButtonClick()
        {
            isDebugMode = debugButton.value;
        }

        void onAccButtonClick()
        {
            isOpenAcc = accButton.value;
        }

        void LateUpdate()
        {
            view3D();
            view2D();
            keypad();
            acc();
        }

        void view3D()
        {
            if (World.activeCamera > 1 && !SubCamera.isBuildStationMode)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    clickTime = 0;
                }
                clickTime += Time.deltaTime;

                if (Input.GetMouseButtonUp(0) && clickTime < 0.2f && !IUtils.isPointGUI())
                {
                    Ray ray = Camera3DWorld.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, collider3DLayer))
                    {
                        if (hit.collider.CompareTag(stationTag))
                        {
                            StationComponent stationComponent = hit.transform.GetComponent<StationComponent>();
                            if (stationComponent != null)
                            {
                                stationComponent.onPoolerClick();
                                return;
                            }

                            Station station = hit.transform.GetComponent<Station>();
                            if (station != null)
                            {
                                station.onPoolerClick();
                                return;
                            }
                        }
                        else if (hit.collider.CompareTag(preloadShipTag))
                        {
                            ShipPreload shipPreload = hit.transform.GetComponent<ShipPreload>();
                            shipPreload.onPoolerClick();
                            return;
                        }
                    }
                }
            }
        }

        void view2D()
        {         
            if (selectBlock != null && selectBlock.isNeedDelete())
            {               
                selectBlock = null;
            }            
            

            if (World.activeCamera < 2)
            {
                Vector3 v;
                if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && (!GameSetting.isAndroid || Input.touchCount == 1))
                {
                    if (getMouseVectorOn2D(out v))
                    {
                        IPoint coor = IPoint.createMapIPointByWordVector(v, blocksEngine.mapSize);
                        if (isAtMap(coor))
                        {
                            Block block = blocksEngine.getBlock(coor);
                            if (block != null)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    block.onWorldModeClick();
                                }                               
                                SolidBlock selecSolidBlock = selectBlock as SolidBlock;
                                if (selecSolidBlock != null)
                                {
                                    selecSolidBlock.setIsSelecting(false);
                                }
                                selectBlock = block;
                                SolidBlock solidBlock = block as SolidBlock;
                                if (solidBlock != null)
                                {
                                    solidBlock.setIsSelecting(true);
                                    sVSelector.show(selectBlock.isCanSettingValue() != -1);
                                    scientificSelector.show(solidBlock.isCanCollectScientific(), solidBlock);
                                    BlockProgress.instance.setValue(solidBlock.getProgress());
                                }
                                else
                                {
                                    BlockProgress.instance.setValue(0);
                                }

                                if (isDebugMode || openDebug)
                                {
                                    block.addTemperature(addTemperature);
                                    block.addPress(addPress);
                                    Debug.Log(string.Format("{0}{1}t:{2},p:{3},hq:{4},ot:{5},ca:{6}", block.getName(), block.getCoor().toString(), block.getTemperature(), block.getPress(), block.getHeatQuantity(), block.getOutsideTag(), block.getCargoAreaTag()));
                                }
                            }
                        }
                    }
                }
            }
        }

        void keypad()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Pooler.isRunThread = false;
                Debug.Log("Stop Thread");
            }
        }

        void acc()
        {
            if (isOpenAcc)
            {
                float x = Input.acceleration.x;
                if (x < -0.25f)
                {
                    x = -0.25f;
                }
                else if (x > 0.25)
                {
                    x = 0.25f;
                }
                else if (Mathf.Abs(x) < 0.08f)
                {
                    x = 0;
                }
                PoolerUI.instance.rudder.setValue(x * 2 + 0.5f);
                PoolerUI.instance.onRudderPush();
            }
        }

        bool getMouseVectorOn2D(out Vector3 point)
        {
            point = Vector3.zero;

            if (World.activeCamera >= 2)
            {
                return false;
            }

            Ray ray = Camera2DWorld.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 21, collider2DLayer))
            {
                bool isPointGUI = GameSetting.isAndroid ? Input.touchCount > 0 ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : false : EventSystem.current.IsPointerOverGameObject();
                if (isPointGUI)
                {
                    return false;
                }
                point = hit.point;
                return true;
            }

            return false;
        }        

        bool isAtMap(IPoint coor)
        {

            return (new IRect(0, 0, blocksEngine.mapSize.x - 1, blocksEngine.mapSize.y - 1).containsPoint(coor));
        }
    }
}