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

namespace Scraft
{
    public class SteamInGameScene : MonoBehaviour
    {
    #if !DISABLESTEAMWORKS    

        protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
        

        private void OnEnable()
        {
            if (SteamManager.Initialized)
                m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }

        private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
        {
            if (pCallback.m_bActive != 0)
            {
                World.stopUpdata = true;
                Time.timeScale = 0;
            }
            else
            {
                World.stopUpdata = false;
                Time.timeScale = 1;
            }
        }
    #endif
    }
}
