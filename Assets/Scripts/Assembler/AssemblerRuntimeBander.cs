using Battlehub.RTCommon;
using Battlehub.RTHandles;
using Scraft.DpartSpace;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class AssemblerRuntimeBander
    {
        Assembler assembler;
        DpartsEngine dpartsEngine;

        bool isInitRTHandle;
        GameObject GLRanderObject;
        //BoxSelection boxSelection;
        Transform RuntimeSelectionTrans;
        RuntimeSelectionComponent runtimeSelectionComponent;
        PositionHandle positionHandle;
        RotationHandle rotationHandle;
        ScaleHandle scaleHandle;
        GameObject runtimeBtnRectObject;
        RectTransform runtimeBtnRectRectTrans;
        public static ISwitchImageTextButton TxtCurrentControl;
        public static ISwitchImageTextButton GirdCurrentControl;
        public static Sprite[] runtimeBtnSprites;

        public AssemblerRuntimeBander()
        {
            assembler = Assembler.instance;
            dpartsEngine = Assembler.dpartsEngine;
        }

        public void initRTHandle()
        {
            if (!isInitRTHandle)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GLRanderObject = GameObject.Find("GLRenderer");
                    //RuntimeSelectionTrans = GameObject.Find("RTHandles").transform;
                    //runtimeSelectionComponent = RuntimeSelectionTrans.GetComponent<RuntimeSelectionComponent>();


                    if (true)
                    {
                        //positionHandle = RuntimeSelectionTrans.GetChild(0).GetComponent<PositionHandle>();
                        //rotationHandle = RuntimeSelectionTrans.GetChild(1).GetComponent<RotationHandle>();
                        //scaleHandle = RuntimeSelectionTrans.GetChild(2).GetComponent<ScaleHandle>();
                        //boxSelection = boxSelectionCanvasObject.transform.GetChild(0).GetComponent<BoxSelection>();

                        runtimeBtnRectObject = GameObject.Find("RTBtn");
                        runtimeBtnRectRectTrans = runtimeBtnRectObject.GetComponent<RectTransform>();
                        runtimeBtnRectObject.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(onRuntimeHandleSwitchButtonClick);
                        runtimeBtnRectObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(onRuntimeHandleUndoButtonClick);
                        runtimeBtnRectObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(onRuntimeHandleRedoButtonClick);
                        runtimeBtnRectObject.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(onRuntimeHandleResetButtonClick);
                        runtimeBtnRectObject.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(onRuntimeHandleUnitButtonClick);
                        runtimeBtnRectObject.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(onRuntimeHandleSelectAllButtonClick);
                        runtimeBtnRectObject.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(onRuntimeHandleSelectInverseButtonClick);

                        TxtCurrentControl = runtimeBtnRectObject.transform.GetChild(0).GetComponent<ISwitchImageTextButton>();
                        GirdCurrentControl = runtimeBtnRectObject.transform.GetChild(4).GetComponent<ISwitchImageTextButton>();

                        //boxSelectionCanvasObject.SetActive(false);
                        isInitRTHandle = true;
                        //RuntimeTools.UnitSnapping = Assembler.IS_UnitSnapping;
                        runtimeBtnSprites = Resources.LoadAll<Sprite>("assembler/runtime");
                    }
                }
            }
        }

        void onRuntimeHandleSwitchButtonClick()
        {

            if (IRT.Tools.Current == RuntimeTool.None || IRT.Tools.Current == RuntimeTool.View)
            {
                IRT.Tools.Current = RuntimeTool.Move;
                TxtCurrentControl.change(runtimeBtnSprites[0], "move");
            }

            else if (IRT.Tools.Current == RuntimeTool.Move)
            {
                IRT.Tools.Current = RuntimeTool.Rotate;
                TxtCurrentControl.change(runtimeBtnSprites[1], "rotate");
            }

            else if (IRT.Tools.Current == RuntimeTool.Rotate)
            {
                IRT.Tools.Current = RuntimeTool.Scale;
                TxtCurrentControl.change(runtimeBtnSprites[2], "scale");
            }

            else if (IRT.Tools.Current == RuntimeTool.Scale)
            {
                IRT.Tools.Current = RuntimeTool.Move;
                TxtCurrentControl.change(runtimeBtnSprites[0], "move");
            }

        }

        void onRuntimeHandleUndoButtonClick()
        {
            IRT.Undo.Undo();
        }

        void onRuntimeHandleRedoButtonClick()
        {
            IRT.Undo.Redo();
        }

        void onRuntimeHandleResetButtonClick()
        {

            if (IRT.Tools.Current == RuntimeTool.Move)
            {
                AssemblerInput.changeSelectDpart(go => go.transform.localPosition = Vector3.zero);
            }
            else if (IRT.Tools.Current == RuntimeTool.Rotate)
            {
                AssemblerInput.changeSelectDpart(go => go.transform.rotation = Quaternion.identity);
            }
            else if (IRT.Tools.Current == RuntimeTool.Scale)
            {
                AssemblerInput.changeSelectDpart(go => go.transform.localScale = Vector3.one);
            }

        }

        public void onRuntimeHandleUnitButtonClick()
        {
            Assembler.IS_UnitSnapping = !Assembler.IS_UnitSnapping;
            //RuntimeTools.UnitSnapping = Assembler.IS_UnitSnapping;
            if (Assembler.IS_UnitSnapping)
            {
                GirdCurrentControl.change(runtimeBtnSprites[6], "grid");
            }
            else
            {
                GirdCurrentControl.change(runtimeBtnSprites[7], "continuity");
            }
        }

        void onRuntimeHandleSelectAllButtonClick()
        {
            IRT.Selection.objects = dpartsEngine.getDpartsGameObjectArray();
        }

        void onRuntimeHandleSelectInverseButtonClick()
        {
            List<GameObject> gosList = new List<GameObject>(dpartsEngine.getDpartsGameObjectArray());
            AssemblerInput.changeSelectDpart(go => gosList.Remove(go));
            IRT.Selection.objects = gosList.ToArray();
        }
    }
}