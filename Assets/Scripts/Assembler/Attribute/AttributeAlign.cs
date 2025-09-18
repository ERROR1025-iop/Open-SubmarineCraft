using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Scraft
{

    public class AttributeAlign : MonoBehaviour
    {
        static public AttributeAlign instance;

        RectTransform rectTransform;
        Vector2 position;

        public Text needAlignGoText;
        public Text targetAlignGoText;

        public IChangeImageButton XButton;
        public IChangeImageButton YButton;
        public IChangeImageButton ZButton;

        public IChangeImageButton NMaxButton;
        public IChangeImageButton NCenterButton;
        public IChangeImageButton NMinButton;

        public IChangeImageButton TMaxButton;
        public IChangeImageButton TCenterButton;
        public IChangeImageButton TMinButton;

        bool isShow;
        bool isInit;

        UnityAction partAction;
        UnityAction targetAction;

        GameObject[] partsGos;
        GameObject[] targetsGos;

        Bounds NBounds;
        Bounds TBounds;

        Vector3 NPos;
        Vector3 TPos;

        Vector3[] NOrgPos;

        Vector3 alignAxis = Vector3.zero;

        void Start()
        {
            instance = this;

            rectTransform = GetComponent<RectTransform>();
            position = rectTransform.anchoredPosition;

            XButton.addListener(onXYZButtonClick);
            YButton.addListener(onXYZButtonClick);
            ZButton.addListener(onXYZButtonClick);

            NMaxButton.addListener(onNMaxButtonClick);
            NCenterButton.addListener(onNCenterButtonClick);
            NMinButton.addListener(onNMinButtonClick);

            TMaxButton.addListener(onTMaxButtonClick);
            TCenterButton.addListener(onTCenterButtonClick);
            TMinButton.addListener(onTMinButtonClick);

            show(false);
            isInit = false;
            IRT.Selection.SelectionChanged += OnSelectionChanged;

        }

        private void initializedAttribute()
        {
            if (!isInit)
            {
                onNMaxButtonClick();
                onTMaxButtonClick();
                isInit = true;
            }
        }

        public void initialized()
        {
            initializedAttribute();

            partsGos = IRT.Selection.gameObjects;
            if (partsGos == null || partsGos.Length < 1)
            {
                return;
            }

            string partsName = "";
            NOrgPos = new Vector3[partsGos.Length];
            for (int i = 0; i < partsGos.Length; i++)
            {
                string name = partsGos[i].name;
                name = ILang.get(name.Replace("(Clone)", ""), "dpart");
                partsName += name + ",";
                NOrgPos[i] = partsGos[i].transform.position;
            }
            needAlignGoText.text = ILang.get("part") + ":" + partsName;
            AttributeSelector.instance.cancelMultSelect();
            show(true);
        }

        public void OnSelectionChanged(Object[] unselectedObjects)
        {
            if (isShow)
            {
                if (IRT.Selection.gameObjects == null)
                {
                    return;
                }

                targetsGos = IRT.Selection.gameObjects;
                if (targetsGos.Length < 1)
                {
                    return;
                }

                string targetsName = "";
                for (int i = 0; i < targetsGos.Length; i++)
                {
                    string name = targetsGos[i].name;
                    name = ILang.get(name.Replace("(Clone)", ""), "dpart");
                    targetsName += name + ",";
                }
                targetAlignGoText.text = ILang.get("target") + ":" + targetsName;

                executeAction();
            }
        }

        void executeAction()
        {
            if (partAction != null)
            {
                partAction();
            }
            if (targetAction != null)
            {
                targetAction();
            }
        }

        void onXYZButtonClick()
        {
            alignAxis = new Vector3(XButton.value ? 1 : 0, YButton.value ? 1 : 0, ZButton.value ? 1 : 0);
            executeAction();
        }

        private void OnDrawGizmos()
        {
            if (NBounds != null)
            {
                Gizmos.DrawWireCube(NBounds.center, NBounds.size);
            }

            if (TBounds != null)
            {
                Gizmos.DrawWireCube(TBounds.center, TBounds.size);
            }
        }

        void startAlign(System.Action execute)
        {
            if (partsGos == null || targetsGos == null || partsGos.Length < 0 || targetsGos.Length < 0)
            {
                return;
            }

            resetPartsPos();
            execute();

            if (NPos == null || TPos == null || NOrgPos == null)
            {
                return;
            }

            //Debug.Log("NPos:" + NPos + ",TPos:" + TPos + ",NOrgPos:" + NOrgPos[0]);

            Vector3 moveVector = new Vector3((TPos.x - NPos.x) * alignAxis.x, (TPos.y - NPos.y) * alignAxis.y, (TPos.z - NPos.z) * alignAxis.z);
            IRT.Undo.BeginRecord();
            for (int i = 0; i < partsGos.Length; i++)
            {
                IRT.Undo.BeginRecordTransform(partsGos[i].transform);
                partsGos[i].transform.position = NOrgPos[i] + moveVector;
            }
            IRT.Undo.EndRecord();
            NBounds = IUtils.GetBounds(partsGos);
            TBounds = IUtils.GetBounds(targetsGos);
        }

        void resetPartsPos()
        {
            if (partsGos == null || NOrgPos == null)
            {
                return;
            }

            for (int i = 0; i < partsGos.Length; i++)
            {
                partsGos[i].transform.position = NOrgPos[i];
            }
        }

        void onNMaxButtonClick()
        {

            NMaxButton.setValue(true);
            NCenterButton.setValue(false);
            NMinButton.setValue(false);
            partAction = onNMaxButtonClick;

            startAlign(() => { NBounds = IUtils.GetBounds(partsGos); NPos = NBounds.max; });
        }


        void onNCenterButtonClick()
        {

            NMaxButton.setValue(false);
            NCenterButton.setValue(true);
            NMinButton.setValue(false);
            partAction = onNCenterButtonClick;

            startAlign(() => { NBounds = IUtils.GetBounds(partsGos); NPos = NBounds.center; });
        }

        void onNMinButtonClick()
        {

            NMaxButton.setValue(false);
            NCenterButton.setValue(false);
            NMinButton.setValue(true);
            partAction = onNMinButtonClick;

            startAlign(() => { NBounds = IUtils.GetBounds(partsGos); NPos = NBounds.min; });
        }

        void onTMaxButtonClick()
        {

            TMaxButton.setValue(true);
            TCenterButton.setValue(false);
            TMinButton.setValue(false);
            targetAction = onTMaxButtonClick;

            startAlign(() => { TBounds = IUtils.GetBounds(targetsGos); TPos = TBounds.max; });
        }

        void onTCenterButtonClick()
        {

            TMaxButton.setValue(false);
            TCenterButton.setValue(true);
            TMinButton.setValue(false);
            targetAction = onTCenterButtonClick;

            startAlign(() => { TBounds = IUtils.GetBounds(targetsGos); TPos = TBounds.center; });
        }

        void onTMinButtonClick()
        {

            TMaxButton.setValue(false);
            TCenterButton.setValue(false);
            TMinButton.setValue(true);
            targetAction = onTMinButtonClick;

            startAlign(() => { TBounds = IUtils.GetBounds(targetsGos); TPos = TBounds.min; });
        }

        public void onApplyButtonClick()
        {
            for (int i = 0; i < partsGos.Length; i++)
            {
                NOrgPos[i] = partsGos[i].transform.position;
            }
        }

        public void onCancelButtonClick()
        {
            for (int i = 0; i < partsGos.Length; i++)
            {
                partsGos[i].transform.position = NOrgPos[i];
            }
            show(false);
        }

        public void onConfirmButtonClick()
        {
            show(false);
        }

        public void show(bool show)
        {
            isShow = show;

            if (!isShow)
            {
                targetsGos = null;
                targetAlignGoText.text = ILang.get("target") + ":" + ILang.get("bind0");
                rectTransform.anchoredPosition = new Vector2(0, 9999);
            }
            else
            {
                rectTransform.anchoredPosition = position;
            }
        }

    }
}
