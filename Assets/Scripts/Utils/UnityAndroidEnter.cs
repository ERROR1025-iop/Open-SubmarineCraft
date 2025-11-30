using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class UnityAndroidEnter
    {
        public static void CallCheckSDPermission()
        {
            if (GameSetting.isAndroid)
            {
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallCheckSDPermission", currentActivity);
            }
        }

        public static void CallSavesMain()
        {
            if (GameSetting.isAndroid)
            {
                CallSendIconCount();
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallSavesMain", currentActivity);
            }    
        }

        public static void CallWiki()
        {
            if (GameSetting.isAndroid)
            {
                CallSendIconCount();
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallWiki", currentActivity);
            }
        }

        public static void CallPersonal()
        {
            if (GameSetting.isAndroid)
            {
                CallSendIconCount();
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallPersonal", currentActivity);
            }
        }

        public static void CallUploadShip(string name, bool isAssembler)
        {
            if (GameSetting.isAndroid)
            {
                CallSendIconCount();
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallUploadShip", currentActivity, name, isAssembler);
            }
        }

        public static void CallShowVideoAd(bool isScientific)
        {
            if (GameSetting.isAndroid)
            {
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallShowVideoAd", currentActivity, isScientific);
            }
        }

        public static void CallStatisticsBuyPrice(float totalPrice)
        {
            if (GameSetting.isAndroid)
            {
                CallSendIconCount();
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallStatisticsBuyPrice", currentActivity, totalPrice);
            }
        }

        public static void CallShowInterstitialAD()
        {
            return;
            if (GameSetting.isAndroid)
            {
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallShowInterstitialAD", currentActivity);
            }
        }

        public static void CallCloseInterstitialAD()
        {
            return;
            if (GameSetting.isAndroid)
            {
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallCloseInterstitialAD", currentActivity);
            }
        }

        public static void CallGetUsername()
        {
            if (GameSetting.isAndroid)
            {
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallGetUsername", currentActivity);
            }
        }

        public static void CallGetServiceDiamonds()
        {
            if (GameSetting.isAndroid)
            {
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallGetServiceDiamonds", currentActivity);
            }
        }

        public static void CallSendIconCount()
        {
            if (GameSetting.isAndroid)
            {
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.miaoyue91.submarine.UnityEnterActivity");
                jc.CallStatic("CallSendIconCount", currentActivity, ISecretLoad.getDiamonds());
            }
        }
    }
}