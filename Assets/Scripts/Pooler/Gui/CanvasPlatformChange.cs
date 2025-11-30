using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class CanvasPlatformChange : MonoBehaviour
    {        
        void Awake()
        {
            if (GameSetting.isAndroid)
            {
                var canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
                canvasScaler.referenceResolution = new Vector2(1080, 480);
            }
        }
    }
}
