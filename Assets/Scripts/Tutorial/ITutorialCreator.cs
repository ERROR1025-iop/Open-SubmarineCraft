using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft {
    public class ITutorialCreator : MonoBehaviour
    {

        void Start()
        {
            if (ITutorial.tutorialStep != -1 && ITutorial.tutorialName != null)
            {
                GameObject prefabs = Resources.Load("Tutorial/Prefabs/" + ITutorial.tutorialName) as GameObject;
                if (prefabs != null)
                {
                    GameObject gameObject = Instantiate(prefabs);
                    gameObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
                }             
               
            }
    }
    }
}
