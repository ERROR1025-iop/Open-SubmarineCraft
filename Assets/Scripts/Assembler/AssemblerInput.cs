using Scraft.DpartSpace;
using UnityEngine;
using UnityEngine.EventSystems;



namespace Scraft
{
    public class AssemblerInput : MonoBehaviour
    {

        public static Plane dragPlane;
        public static Dpart mouseSelectUnplaceDPart;

        public GameObject SceneComponent;
        public Camera camera3D;

        Assembler assembler;
        DpartsEngine dpartsEngine;

        Vector3 point;



        void Start()
        {
            assembler = Assembler.instance;
            dpartsEngine = Assembler.dpartsEngine;

            dragPlane = new Plane(Vector3.up, 0);
        }


        void Update()
        {
            bool isPointGUI = IUtils.isPointGUI();
            if (!isPointGUI)
            {
                clickDown();
                movingMouse();
                clickUp();
            }
        }

        void clickDown()
        {
            if (mouseSelectUnplaceDPart == null &&
                (Assembler.ASSEMBLER_GROUP_MODE == 0 && DpartCardManager.selectDPartStatic != null)
                || (GroupDpartsSelector.selectDpart != null))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (Assembler.ASSEMBLER_GROUP_MODE == 0)
                    {
                        mouseSelectUnplaceDPart = dpartsEngine.createDPart(DpartCardManager.selectDPartStatic);
                        mouseSelectUnplaceDPart.onBuilderModeCreate();
                    }
                    else
                    {
                        mouseSelectUnplaceDPart = dpartsEngine.createDPart(GroupDpartsSelector.selectDpart);
                    }
                    IRT.createUndoGameObject(mouseSelectUnplaceDPart.getGameObject());
                    mouseSelectUnplaceDPart.closeCollider(true);
                    SceneComponent.SetActive(false);

                    if (GetPointOnPoint(out point))
                    {
                        Vector3 placePoint = point;

                        mouseSelectUnplaceDPart.getTransform().localPosition = placePoint;
                    }

                    if (Assembler.ASSEMBLER_PLACE_MODE == 1)
                    {
                        Dpart dpart = dpartsEngine.createMirrorDPart(mouseSelectUnplaceDPart);
                        dpart.onBuilderModeCreate();
                        dpart.closeCollider(true);
                        mouseSelectUnplaceDPart.isSelecting = true;
                    }
                }
            }
        }

        void movingMouse()
        {
            if (mouseSelectUnplaceDPart != null)
            {
                Ray ray = camera3D.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Vector3 placePoint = Vector3.zero;
                if (Physics.Raycast(ray, out hit))
                {
                    point = hit.point;
                    //Debug.Log(hit.transform.name);
                    if (Assembler.ASSEMBLER_PLACE_MODE == 2)
                    {
                        point = new Vector3(point.x, point.y, 0);
                    }
                    Vector3 textPoint = hit.transform.position;
                    Vector3 closesetPoint = mouseSelectUnplaceDPart.getColliderClosestPoint(textPoint);
                    Vector3 moveVector = closesetPoint - hit.point;
                    placePoint = hit.point - moveVector;
                    if (Assembler.IS_UnitSnapping)
                    {
                        placePoint = new Vector3(Mathf.RoundToInt(placePoint.x), Mathf.RoundToInt(placePoint.y), Mathf.RoundToInt(placePoint.z));
                    }
                }
                else if (GetPointOnDragPlane(out point))
                {
                    placePoint = point;
                }

                if (Assembler.ASSEMBLER_PLACE_MODE == 2)
                {
                    placePoint = new Vector3(placePoint.x, placePoint.y, 0);
                }
                mouseSelectUnplaceDPart.getTransform().position = placePoint;
            }
        }

        void clickUp()
        {
            if (mouseSelectUnplaceDPart != null && Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    dpartsEngine.deleteDpart(mouseSelectUnplaceDPart);
                }
                else
                {
                    GameObject gameObject = mouseSelectUnplaceDPart.getGameObject();
                    IRT.Undo.BeginRecord();
                    IRT.Selection.activeGameObject = gameObject;
                    IRT.Undo.EndRecord();
                    mouseSelectUnplaceDPart.closeCollider(false);
                    if (Assembler.ASSEMBLER_PLACE_MODE == 1)
                    {
                        mouseSelectUnplaceDPart.mirrorDpart.closeCollider(false);
                    }
                }
                mouseSelectUnplaceDPart = null;
                DpartCardManager.selectDPartStatic = null;
                GroupDpartsSelector.selectDpart = null;
                DpartAttribute.instance.show(true);
                SceneComponent.SetActive(true);
            }

        }

        public static void changeSelectDpart(System.Action<GameObject> execute)
        {
            GameObject[] selection = IRT.Selection.gameObjects;
            if (selection == null)
            {
                return;
            }

            for (int i = 0; i < selection.Length; ++i)
            {
                GameObject selectedObj = selection[i];
                if (selectedObj != null)
                {
                    execute(selectedObj);
                }
            }
        }

        bool GetPointOnDragPlane(out Vector3 point)
        {
            Ray ray = camera3D.ScreenPointToRay(Input.mousePosition);
            float distance;
            if (dragPlane.Raycast(ray, out distance))
            {
                point = ray.GetPoint(distance);
                if (isAt3DMap(point))
                {
                    if (Assembler.IS_UnitSnapping)
                    {
                        point = new Vector3(Mathf.RoundToInt(point.x), point.y, Mathf.RoundToInt(point.z));
                    }
                    return true;
                }
                return false;
            }
            point = Vector3.zero;
            return false;
        }

        bool GetPointOnPoint(out Vector3 point)
        {
            Ray ray = camera3D.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                point = hit.point;
                return true;
            }
            else if (GetPointOnDragPlane(out point))
            {
                return true;
            }
            point = Vector3.zero;
            return false;
        }

        bool isAt3DMap(Vector3 point)
        {
            if (Mathf.Abs(point.x) > 15)
            {
                return false;
            }
            if (Mathf.Abs(point.y) > 15)
            {
                return false;
            }
            if (Mathf.Abs(point.z) > 15)
            {
                return false;
            }
            return true;
        }
    }
}