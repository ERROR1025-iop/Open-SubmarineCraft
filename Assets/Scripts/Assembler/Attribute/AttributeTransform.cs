using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.DpartSpace;

namespace Scraft
{

    public class AttributeTransform : MonoBehaviour
    {



        public static AttributeTransform instance;
        DpartsEngine dpartsEngine;

        void Start()
        {
            instance = this;
            dpartsEngine = Assembler.dpartsEngine;

            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(onRotationXButtonClick);
            transform.GetChild(2).GetComponent<Button>().onClick.AddListener(onRotationYButtonClick);
            transform.GetChild(3).GetComponent<Button>().onClick.AddListener(onRotationZButtonClick);
            transform.GetChild(4).GetComponent<Button>().onClick.AddListener(onCenterXButtonClick);
            transform.GetChild(5).GetComponent<Button>().onClick.AddListener(onCenterYButtonClick);
            transform.GetChild(6).GetComponent<Button>().onClick.AddListener(onCenterZButtonClick);
            transform.GetChild(7).GetComponent<Button>().onClick.AddListener(onCopyButtonClick);
            transform.GetChild(8).GetComponent<Button>().onClick.AddListener(onMirrorButtonClick);
            transform.GetChild(9).GetComponent<Button>().onClick.AddListener(onAlignButtonClick);
        }

        public void onRotationXButtonClick()
        {
            rotation(Vector3.right);
        }

        public void onRotationYButtonClick()
        {
            rotation(Vector3.up);
        }

        public void onRotationZButtonClick()
        {
            rotation(Vector3.forward);
        }

        void rotation(Vector3 axis)
        {
            GameObject[] selection = IRT.Selection.gameObjects;
            if (selection == null)
            {
                return;
            }

            if (selection.Length == 1)
            {
                IRT.Undo.BeginRecordTransform(selection[0].transform);
                selection[0].transform.Rotate(axis, 90);
                return;
            }

            Vector3 centerPoint = Vector3.zero;
            Vector3 position;
            int count = selection.Length;
            for (int i = 0; i < count; i++)
            {
                position = selection[i].transform.localPosition;
                centerPoint.x += position.x;
                centerPoint.y += position.y;
                centerPoint.z += position.z;
            }
            centerPoint = centerPoint / selection.Length;

            IRT.Undo.BeginRecord();
            for (int i = 0; i < count; i++)
            {
                IRT.Undo.BeginRecordTransform(selection[i].transform);
                selection[i].transform.RotateAround(centerPoint, axis, 90);
            }
            IRT.Undo.EndRecord();
            IRT.Selection.activeObject = null;
            IRT.Undo.Undo();
        }

        public void onAlignmentXButtonClick()
        {
            anlignment((go1, go2) => { IRT.Undo.BeginRecordTransform(go2.transform); go2.transform.localPosition = new Vector3(go1.transform.localPosition.x, go2.transform.localPosition.y, go2.transform.localPosition.z); });
        }

        public void onAlignmentYButtonClick()
        {
            anlignment((go1, go2) => { IRT.Undo.BeginRecordTransform(go2.transform); go2.transform.localPosition = new Vector3(go2.transform.localPosition.x, go1.transform.localPosition.y, go2.transform.localPosition.z); });
        }

        public void onAlignmentZButtonClick()
        {
            anlignment((go1, go2) => { IRT.Undo.BeginRecordTransform(go2.transform); go2.transform.localPosition = new Vector3(go2.transform.localPosition.x, go2.transform.localPosition.y, go1.transform.localPosition.z); });
        }

        void anlignment(System.Action<GameObject, GameObject> execute)
        {
            GameObject[] selection = IRT.Selection.gameObjects;
            if (selection == null || selection.Length < 2)
            {
                return;
            }
            GameObject go1 = selection[0];
            GameObject go2 = selection[1];

            execute(go1, go2);
        }

        public void onCenterXButtonClick()
        {
            center(Vector3.forward);
        }

        public void onCenterYButtonClick()
        {
            center(Vector3.right);
        }

        public void onCenterZButtonClick()
        {
            center(Vector3.up);
        }

        private void center(Vector3 axis)
        {
            GameObject[] selection = IRT.Selection.gameObjects;
            if (selection == null)
            {
                return;
            }

            Vector3 centerPoint = Vector3.zero;
            Vector3 position;
            int count = selection.Length;
            for (int i = 0; i < count; i++)
            {
                position = selection[i].transform.localPosition;
                centerPoint.x += position.x;
                centerPoint.y += position.y;
                centerPoint.z += position.z;
            }
            centerPoint.y += AssemblerInput.dragPlane.distance;

            centerPoint.x *= axis.x;
            centerPoint.y *= axis.y;
            centerPoint.z *= axis.z;

            centerPoint = centerPoint / selection.Length;

            IRT.Undo.BeginRecord();
            for (int i = 0; i < count; i++)
            {
                IRT.Undo.BeginRecordTransform(selection[i].transform);
                selection[i].transform.localPosition = selection[i].transform.localPosition - centerPoint;
            }
            IRT.Undo.EndRecord();
            IRT.Selection.activeObject = null;
            IRT.Undo.Undo();
        }

        public void onCopyButtonClick()
        {
            List<GameObject> gos = new List<GameObject>();
            AssemblerInput.changeSelectDpart(go =>
            {
                Dpart orgDpart = go.GetComponent<DpartParent>().getDpart();
                Dpart dpart = dpartsEngine.createCopyDPart(orgDpart);
                dpart.onBuilderModeCreate();
                gos.Add(dpart.getGameObject());
                dpart.setColor(orgDpart.getColor(1), 1);
                dpart.setColor(orgDpart.getColor(2), 2);

                if (orgDpart.mirrorDpart != null)
                {
                    Dpart mdpart = dpartsEngine.createCopyDPart(orgDpart.mirrorDpart);
                    mdpart.onBuilderModeCreate();
                    gos.Add(mdpart.getGameObject());
                    mdpart.setColor(orgDpart.getColor(1), 1);
                    mdpart.setColor(orgDpart.getColor(2), 2);
                    dpart.setMirrorDpart(mdpart);
                }
            });

            IRT.createUndoGameObjects(gos);
        }

        public void onMirrorButtonClick()
        {
            List<GameObject> gos = new List<GameObject>();
            AssemblerInput.changeSelectDpart(go =>
            {
                DpartParent dpartParent = go.GetComponent<DpartParent>();
                if (dpartParent.getDpart().mirrorDpart == null)
                {
                    Dpart dpart = dpartsEngine.createMirrorDPart(dpartParent.getDpart());
                    dpart.onBuilderModeCreate();
                    gos.Add(dpart.getGameObject());
                    dpartParent.OnSelectionChanged(null);
                }
            });

            IRT.createUndoGameObjects(gos, false);
        }

        public void onSetAlignButtonClick()
        {

        }

        public void onAlignButtonClick()
        {
            AttributeAlign.instance.initialized();
        }

        public void onSetPlaneButtonClick()
        {
            if (IRT.Selection.activeTransform != null)
            {
                Vector3 apos = IRT.Selection.activeTransform.position;
                Assembler.instance.setPlaneOffset(apos.y);
            }
        }

        public void onMassCenterUpClick()
        {
            Assembler.instance.setMassOffset(Assembler.massOffset + 0.01f);
        }

        public void onMassCenterDownClick()
        {
            Assembler.instance.setMassOffset(Assembler.massOffset - 0.01f);
        }

        public void onMassCenterResetClick()
        {
            Assembler.instance.setMassOffset(0);
        }
    }
}
