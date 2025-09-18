using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTCommon;

namespace Scraft.DpartSpace
{

    public class DpartsEngine
    {

        public static int Max_Dpart_Count = 1000;
        public Dpart[] dpartArr;

        public GameObject dpartParentObject;

        public DpartsEngine(GameObject dpartParentObject)
        {      
            this.dpartParentObject = dpartParentObject;

            dpartArr = new Dpart[Max_Dpart_Count];
            clearDpartArr();
        }

        //==================
        //======Main========
        public Dpart createDPart(Dpart dpartStatic)
        {
            Dpart dpart = dpartStatic.clone(dpartParentObject);
            addDpartArr(dpart);
            return dpart;
        }

        public Dpart createCopyDPart(Dpart dpart)
        {
            Dpart copyDpart = createDPart(dpart);
            copyDpart.copyTransform(dpart);
            return copyDpart;
        }

        public Dpart createMirrorDPart(Dpart dpart)
        {
            Dpart mirrorDpart = createDPart(dpart);
            mirrorDpart.mirrorTransform(dpart);
            return mirrorDpart;
        }

        public void addDpartArr(Dpart dpart)
        {
            for (int i = 0; i < Max_Dpart_Count; i++)
            {
                if (dpartArr[i] == null)
                {
                    dpartArr[i] = dpart;
                    dpart.setIdentifyId(i);
                    return;
                }
            }
        }

        public void removeAllDpart()
        {
            for (int i = 0; i < Max_Dpart_Count; i++)
            {
                if (dpartArr[i] != null)
                {
                    dpartArr[i].clear();
                    dpartArr[i] = null;
                }
            }
            if (World.GameMode == World.GameMode_Assembler && IRT.Selection != null)
            {
                IRT.Selection.activeObject = null;
            }
        }

        public void clearDpartArr()
        {
            for (int i = 0; i < Max_Dpart_Count; i++)
            {
                dpartArr[i] = null;
            }
        }

        public Dpart getDpart(IPoint coor)
        {
            for (int i = 0; i < Max_Dpart_Count; i++)
            {
                if (dpartArr[i] != null)
                {
                    Dpart dpart = dpartArr[i];
                    if (dpart.isContainCoor(coor))
                    {
                        return dpart;
                    }
                }
            }
            return null;
        }

        public void deleteDpart(Dpart dpart)
        {
            deleteDpart(dpart.getIdentifyId());
        }

        public void deleteDpart(int identifyId)
        {
            Dpart depart = dpartArr[identifyId];
            dpartArr[identifyId] = null;
            depart.clear();
            depart = null;
        }

        public GameObject[] getDpartsGameObjectArray()
        {
            GameObject[] gameObjects = new GameObject[dpartArr.Length];
            int count = 0;
            for (int i = 0; i < Max_Dpart_Count; i++)
            {
                if (dpartArr[i] != null)
                {
                    gameObjects[count++] = dpartArr[i].getGameObject();
                }

            }
            return gameObjects;
        }

    }
}
