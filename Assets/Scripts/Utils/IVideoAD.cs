using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scraft
{
    public class IVideoAD : MonoBehaviour
    {
        static public IVideoAD instance;

        static public bool isScientific;

        UnityAction action;

        void Start()
        {
            instance = this;
        }

        public void showVideoAD(bool isSci, UnityAction listener)
        {
            isScientific = isSci;
            action = listener;
            UnityAndroidEnter.CallShowVideoAd(isScientific);
        }

        void onVideoAdCallBack(string isSci)
        {
            //IToast.instance.showWithoutILang("onVideoAdCallBack:" + isSci);
            isScientific = (isSci.Equals("1"));
            if(action != null)
            {
                action();
            }
        }
    }
}
