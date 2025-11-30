#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

using UnityEngine;
#if !DISABLESTEAMWORKS
using System.Collections;
using Steamworks;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ISteamUsername : MonoBehaviour
{
#if !DISABLESTEAMWORKS    
    static public string steamUsername;

    void Start()
    {
        bool isSteamInit = SteamAPI.Init();
        if (isSteamInit)
        {
            steamUsername = SteamFriends.GetPersonaName();
            if (steamUsername != null)
            {
                Text text = GetComponent<Text>();
                if (text != null)
                {
                    text.text = steamUsername;
                }
            }
        }
    }

#endif
}
