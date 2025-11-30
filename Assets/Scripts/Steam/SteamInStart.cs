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

public class SteamInStart : MonoBehaviour {
#if !DISABLESTEAMWORKS
	// Use this for initialization
	void Awake () {
		if (SteamAPI.Init())
		{
			if (!SteamApps.BIsSubscribed())
			{
				Application.Quit();
			}
		}
	}
#endif
}
