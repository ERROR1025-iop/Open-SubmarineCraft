using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Scraft {
    public class ITutorial : MonoBehaviour
    {
        static public ITutorial instance;
        static public int tutorialStep = -1;
        static public string tutorialName;
        static public bool orgIsCareer;       

        public string abbreviation;
        public bool isOpen3DView;        
        public bool debug;
        public int debugStep;

        List<ITutorialStep> steps;
        int stepCount;              
        SpriteRenderer sketchMap;

        static public void start(string sence, bool isCareer, string tutorialName)
        {
            orgIsCareer = GameSetting.isCareer;
            GameSetting.isCareer = isCareer;
            GameSetting.save();
            ITutorial.tutorialName = tutorialName;         
            tutorialStep = 0;
            IToast.instance.show("Loading");
            SceneManager.LoadScene(sence);
        }

        void Start()
        {
            if (debug)
            {
                tutorialStep = debugStep;
            }

            if (SceneManager.GetActiveScene().name.Equals("Builder") && Builder.instance != null)
            {
                Builder.instance.setDpartViewOpen(isOpen3DView);
                sketchMap = GameObject.Find("SketchMap").GetComponent<SpriteRenderer>();
            }

            instance = this;          
            steps = new List<ITutorialStep>();

            stepCount = transform.childCount;
            for (int i = 0; i < stepCount; i++)
            {
                steps.Add(transform.GetChild(i).GetComponent<ITutorialStep>());
                transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(onNextStepClick);
                var file = GameSetting.isAndroid ? "tutorial" : "tutorial_pc";
                transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text = ILang.get(string.Format("{0}.step{1}", abbreviation, i.ToString()), file);
                transform.GetChild(i).gameObject.SetActive(false);
            }

            steps[tutorialStep].SetActive(true);    
            
        }

        public void showSketchMap(string sketchMapName)
        {
            if (sketchMap != null)
            {
                if (sketchMapName == null || sketchMapName.Equals(""))
                {
                    sketchMapName = string.Format("{0}_{1}", abbreviation, tutorialStep);
                }
                var folder = GameSetting.isAndroid ? "Images" : "Images_pc";
                Sprite sprite = Resources.Load<Sprite>(string.Format("Tutorial/{0}/{1}", folder, sketchMapName));
                if (sprite != null)
                {
                    sketchMap.gameObject.SetActive(true);
                    sketchMap.sprite = sprite;
                }
            }                    
        }

        public void hideSketchMap()
        {
            if (sketchMap != null)
            {
                sketchMap.gameObject.SetActive(false);
            }
        }

        void onNextStepClick()
        {
            if (tutorialStep < stepCount - 1 && tutorialStep >= 0)
            {               
                if (steps[tutorialStep].onNextStepClick())
                {
                    ++tutorialStep;
                }
                else
                {
                    steps[tutorialStep].SetActive(false);
                    steps[++tutorialStep].SetActive(true);
                }
                
                              
            }
            else
            {
                tutorialStep = -1;
                tutorialName = null;
                IConfigBox.instance.show(ILang.get("This tutorial is finish"), onFinishClick, onFinishClick);
            }
        }        

        void onFinishClick()
        {
            tutorialStep = -1;
            tutorialName = null;
            GameSetting.isCareer = orgIsCareer;
            GameSetting.save();
            IToast.instance.show("Loading");
            SceneManager.LoadScene("Tutorial");
        }
    }
}
