using System.Collections;
using System.Collections.Generic;
using Scraft;
using UnityEngine;

public class PlatformAdaptor : MonoBehaviour
{
    public bool onlyMobile;
    public bool onlyDesktop;
    public bool onlyEditor;
    void Awake()
    {
        if (GameSetting.isAndroid && onlyDesktop)
        {
            gameObject.SetActive(false);
        }
        if (!GameSetting.isAndroid && onlyMobile)
        {
            gameObject.SetActive(false);
        }
        if (onlyEditor && !Application.isEditor)
        {
            gameObject.SetActive(false);
        }
    }
}
