using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Scraft {

    public class ITutorialStep : MonoBehaviour
    {
        public bool sketchMap;
        public string sketchMapName;
        public bool jumpToPooler;
        public bool jumpToBuilder;
        public bool IGotIt;

        void Start()
        {
            transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = ILang.get(IGotIt ? "I got it" : "I'm done");
        }

        public bool onNextStepClick()
        {
            if (jumpToPooler)
            {
                ITutorial.tutorialStep--;
                Builder.instance.onRunButtonClick();
                return true;
            }
            if (jumpToBuilder)
            {
                ITutorial.tutorialStep--;
                if (SceneManager.GetActiveScene().name.Equals("Pooler"))
                {
                    Builder.IS_LOAD_LAST = true;
                    PoolerMenu.instance.onRevertBlueprintButtonClick();
                }
                else
                if (SceneManager.GetActiveScene().name.Equals("Assembler"))
                {
                    Assembler.instance.assemblerSenceSelector.onBuilderButtonClick();
                }
                return true;
            }
            return false;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if (active)
            {                
                if (sketchMap)
                {
                    ITutorial.instance.showSketchMap(sketchMapName);
                }
            }
            else
            {
                if (sketchMap)
                {
                    ITutorial.instance.hideSketchMap();
                }
            }
        }
    }   
    
}
