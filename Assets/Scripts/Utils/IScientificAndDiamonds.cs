using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace Scraft
{
    public class IScientificAndDiamonds : MonoBehaviour
    {
        static public IScientificAndDiamonds instance;
        static public IScientificAndDiamonds instance_Scientific;
        static public IScientificAndDiamonds instance_Diamonds;
        public bool isScientific;

        Text numberText;

        float m_value;

        float viewAdCanGet = 300;
        float viewAdIntervalHour = 3;

        TpHttpManager tpHttpManager1;
        string viewADTime;       


        void Start()
        {
            instance = this;

            if (isScientific)
            {
                instance_Scientific = this;
            }
            else
            {
                instance_Diamonds = this;
            }

            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(onAddButtonClick);
            numberText = transform.GetChild(2).GetComponent<Text>();

            ISecretLoad.init();
            UpdateNumberText();
        }

        void onAddButtonClick()
        {
            checkViewADtime();
        }

        void checkViewADtime()
        {
            tpHttpManager1 = new TpHttpManager(IConfigBox.instance, IToast.instance);
            tpHttpManager1.setListener(onGetTimeResponse);
            tpHttpManager1.setTpPost("ApiSaves", "getADConfigCon");
            tpHttpManager1.addVersion();
            tpHttpManager1.send();
        }

        void Update()
        {
            if(tpHttpManager1 != null)
            {
                tpHttpManager1.updata();
            }            
        }

        void onGetTimeResponse()
        {
            JsonData jsonData = JsonMapper.ToObject(tpHttpManager1.result);
            int result = IUtils.getJsonValue2Int(jsonData, "result");
            string msg = IUtils.getJsonValue2String(jsonData, "msg");
            if (result == 1)
            {
                viewADTime = msg;
                viewAdCanGet = IUtils.getJsonValue2Int(jsonData, "icon");
                checkTime();
            }
            else
            {                
                IToast.instance.show(msg, 100);
            }

        }

        void checkTime()
        {            
            if (GameSetting.viewAdTime == null)
            {
                pushConfigBox();
            }
            else
            {
                System.DateTime pauseT = System.Convert.ToDateTime(GameSetting.viewAdTime);
                System.DateTime resumeT = System.Convert.ToDateTime(viewADTime);
                System.TimeSpan ts1 = new System.TimeSpan(pauseT.Ticks);
                System.TimeSpan ts2 = new System.TimeSpan(resumeT.Ticks);
                System.TimeSpan tsSub = ts1.Subtract(ts2).Duration();
                //Debug.Log("resume  List  " + tsSub.Days + "   " + tsSub.Hours + "  " + tsSub.Minutes);
                if (tsSub.Hours < viewAdIntervalHour)
                {
                    int againHours = 2 - tsSub.Hours;
                    int againMinutes = 59 - tsSub.Minutes;
                    IToast.instance.showWithoutILang(string.Format(ILang.get("view hour"), againHours, againMinutes), 100);
                }
                else
                {
                    pushConfigBox();
                }
            }
        }

        void pushConfigBox()
        {            
            string title = string.Format("{0}{1}{2}{3}", ILang.get("view ad1"), viewAdCanGet, ILang.get(isScientific ? "scientific points" : "diamonds"), ILang.get("view ad2"));
            IConfigBox.instance.show(title, onConfirmButtonClick, null);
        }

        void onConfirmButtonClick()
        {
            //IVideoAD.instance.showVideoAD(isScientific, onVideoAdCallBack);
            onVideoAdCallBack();
        }

        void onVideoAdCallBack()
        {
            //if (isScientific == IVideoAD.isScientific)
            if (true)
            {
                if (isScientific)
                {
                    ISecretLoad.setScientific(m_value + viewAdCanGet);
                    IToast.instance.showWithoutILang(string.Format(ILang.get("You earn income scientific points {0}"), viewAdCanGet.ToString()), 100);
                }
                else
                {
                    ISecretLoad.setDiamonds(m_value + viewAdCanGet);
                    IToast.instance.showWithoutILang(string.Format(ILang.get("You earn income diamonds {0}"), viewAdCanGet.ToString()), 100);
                }
                GameSetting.viewAdTime = viewADTime;
                GameSetting.save();
                UpdateNumberText();
            }
        }

        public void UpdateNumberText()
        {
            m_value = isScientific ? ISecretLoad.getScientific() : ISecretLoad.getDiamonds();
            numberText.text = m_value.ToString("f1");
            UnityAndroidEnter.CallSendIconCount();
        }
    }
}
