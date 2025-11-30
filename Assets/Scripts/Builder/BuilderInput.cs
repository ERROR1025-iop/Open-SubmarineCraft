using Scraft.BlockSpace;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scraft
{
    public class BuilderInput : MonoBehaviour
    {
        public float addTemperature = 300;
        public float addPress = 0;
        public Camera camera2D; 
        BlocksEngine blocksEngine;
        World world;
        

        Vector3 decVector;

        Vector3 lastPonitPos;
        Vector3 lastMapPos;

        IPoint lastPointCoor;
        IPoint lastPartsCoor;

        LargeBlockTap largeBlockTap;

        void Start()
        {

            world = World.instance;
            blocksEngine = world.blocksEngine;

            decVector = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
            lastMapPos = Camera.main.transform.localPosition;
            lastPonitPos = Vector3.zero;
            lastPartsCoor = IPoint.zero;
            lastPointCoor = IPoint.zero;


            largeBlockTap = new LargeBlockTap();
        }


        void LateUpdate()
        {
            Vector3 v;
            bool result = getMouseVector(out v);
            IPoint coor = IPoint.createMapIPointByWordVector(v, blocksEngine.mapSize);
            if (result && isAtMap(coor))
            {
              
                if (Input.touchCount == 2)
                {
                    moveMap(v);
                }
                else
                {
                    if (Builder.BUILDER_MODE == 0)
                    {
                        placeBlock(coor);
                    }
                    else if (Builder.BUILDER_MODE == 3)
                    {
                        delBlock(coor);
                    }
                    else if (Builder.BUILDER_MODE == 4)
                    {
                        bindBlock(coor);
                    }
                    else if (Builder.BUILDER_MODE == 5)
                    {
                        movePartsMap(coor);
                    }
                }
            }
        }

        void placeBlock(IPoint coor)
        {
            Block block = blocksEngine.getBlock(coor);
            if (Input.GetMouseButton(0))
            {
                if (CardManager.selectBlockStatic.isLargerBlock())
                {
                    largeBlockTap.placeLargeBlockTap(coor, CardManager.selectBlockStatic as LargeBlock);
                }
                else
                {
                    if (Builder.IS_Can_Cover)
                    {
                        if (block != null)
                        {
                            if (!block.isLargerBlock())
                            {
                                Builder.instance.delBlock(coor);
                                Block nblock = blocksEngine.createBlock(coor, CardManager.selectBlockStatic);
                                Builder.instance.onBlockCreate(nblock, coor);
                            }
                        }
                        else
                        {
                            Block nblock = blocksEngine.createBlock(coor, CardManager.selectBlockStatic);
                            Builder.instance.onBlockCreate(nblock, coor);
                        }
                    }
                    else
                    {
                        if (block == null)
                        {
                            Block nblock = blocksEngine.createBlock(coor, CardManager.selectBlockStatic);
                            Builder.instance.onBlockCreate(nblock, coor);
                        }
                        else
                        {
                            Builder.instance.onBlockClick(block, coor);
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (largeBlockTap.isShowTap())
                {
                    IPoint placeCoor = largeBlockTap.getPlaceCoor();
                    largeBlockTap.onLargeBlockPlace();
                    Block tblock = blocksEngine.getBlock(placeCoor);

                    if (Builder.IS_Can_Cover)//可以覆盖
                    {
                        if (tblock != null && tblock.isLargerBlock())//大方块
                        {
                            Block nblock = blocksEngine.createBlock(placeCoor, CardManager.selectBlockStatic);
                            Builder.instance.onBlockCreate(nblock, placeCoor);
                        }
                        else //小方块
                        {
                            Builder.instance.delBlock(placeCoor);
                            Block nblock = blocksEngine.createBlock(placeCoor, CardManager.selectBlockStatic);
                            Builder.instance.onBlockCreate(nblock, placeCoor);
                        }
                    }
                    else//不能覆盖
                    {
                        if (tblock == null)
                        {
                            Block nblock = blocksEngine.createBlock(placeCoor, CardManager.selectBlockStatic);
                            Builder.instance.onBlockCreate(nblock, placeCoor);
                        }
                        else
                        {
                            Builder.instance.onBlockClick(block, coor);
                        }
                    }
                }
            }
            if (Input.GetMouseButton(1))
            {
                Builder.instance.delBlock(coor);
            }

        }

        void delBlock(IPoint coor)
        {
            if (Input.GetMouseButton(0))
            {
                Builder.instance.delBlock(coor);
            }
        }

        void bindBlock(IPoint coor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Block block = blocksEngine.getBlock(coor);
                if (block != null)
                {
                    //Debug.Log(coor.toString());
                    Builder.instance.onBlockClick(block, coor);
                }
            }
        }

        void moveMap(Vector3 v)
        {
            if (Input.GetMouseButton(0))
            {
                Builder.changeMode(1);
                if (lastPonitPos.Equals(Vector3.zero))
                {
                    lastPonitPos = Input.mousePosition;
                }
                Vector3 dv = (Input.mousePosition - lastPonitPos) * 0.01f;
                if (dv.magnitude < 0.2f)
                {
                    Camera.main.transform.localPosition = lastMapPos - dv;
                }
                lastPonitPos = Input.mousePosition;
                lastMapPos = Camera.main.transform.localPosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                lastPonitPos = Vector3.zero;
            }

            if (Builder.isLoadParts)
            {
                Builder.changeMode(5);
            }
        }

        void movePartsMap(IPoint coor)
        {
            if (Input.GetMouseButton(0))
            {
                if (lastPointCoor.Equals(IPoint.zero))
                {
                    lastPointCoor = coor;
                }
                IPoint dv = (lastPointCoor - coor);
                IPoint partsParentCoor = lastPartsCoor - dv;
                Builder.instance.partsParentObject.transform.localPosition = new Vector3(partsParentCoor.x * 0.16f, partsParentCoor.y * 0.16f, 0);
                lastPartsCoor = partsParentCoor;
                lastPointCoor = coor;
            }
            if (Input.GetMouseButtonUp(0))
            {
                lastPointCoor = IPoint.zero;
            }
        }

        bool getMouseVector(out Vector3 vector)
        {
            vector = Vector3.zero;
            Camera gameCamera = Camera.main;
            Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                bool isPointGUI = IUtils.isPointGUI();
                if (isPointGUI)
                {
                    return false;
                }

                if (hit.transform.name == "Click Plane")
                {
                    vector = hit.point;
                    return true;
                }
            }
            return false;
        }

        bool isAtMap(IPoint coor)
        {
            return (new IRect(7, 7, blocksEngine.mapSize.x - 14, blocksEngine.mapSize.y - 14).containsPoint(coor));
        }
    }
}