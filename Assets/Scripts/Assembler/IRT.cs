using Battlehub.RTCommon;
using System.Collections.Generic;
using UnityEngine;
using Scraft.DpartSpace;

namespace Scraft
{
    public class IRT
    {

        static public IRTE RTEditor;
        static public IRuntimeSelectionInternal Selection;
        static public IRuntimeUndo Undo;
        static public RuntimeTools Tools;

        public IRT()
        {
            RTEditor = IOC.Resolve<IRTE>();
            Selection = RTEditor.Selection;
            Undo = RTEditor.Undo;
            Tools = RTEditor.Tools;
        }

        static public void createUndoGameObject(GameObject go)
        {
            ExposeToEditor exposeToEditor = go.GetComponent<ExposeToEditor>();
            RTEditor.Undo.BeginRecord();
            RTEditor.Undo.RegisterCreatedObjects(new[] { exposeToEditor });
            RTEditor.Selection.activeObject = go;
            RTEditor.Undo.EndRecord();
        }

        static public void createUndoGameObjects(List<GameObject> gos, bool select = true)
        {
            RTEditor.Undo.BeginRecord();
            int count = gos.Count;
            for (int i = 0; i < count; i++)
            {
                ExposeToEditor exposeToEditor = gos[i].GetComponent<ExposeToEditor>();
                RTEditor.Undo.RegisterCreatedObjects(new[] { exposeToEditor });
            }
            RTEditor.Undo.EndRecord();

            if (select)
            {
                RTEditor.Selection.objects = gos.ToArray();
            }
        }

        static public void deleteGameObjects(GameObject[] gos)
        {
            if (gos == null)
            {
                return;
            }

            IRT.Undo.BeginRecord();

            int count = gos.Length;
            for (int i = 0; i < count; i++)
            {
                Dpart mirror = gos[i].GetComponent<DpartParent>().getDpart().mirrorDpart;
                if (mirror != null)
                {
                    ExposeToEditor exposeToEditor = mirror.getGameObject().GetComponent<ExposeToEditor>();
                    IRT.Undo.DestroyObjects(new[] { exposeToEditor });
                }
            }

            for (int i = 0; i < count; i++)
            {
                ExposeToEditor exposeToEditor = gos[i].GetComponent<ExposeToEditor>();
                IRT.Undo.DestroyObjects(new[] { exposeToEditor });
            }
            IRT.Undo.EndRecord();
        }
    }
}
