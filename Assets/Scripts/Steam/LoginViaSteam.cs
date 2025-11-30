#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS
using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Steamworks;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scraft
{

    public class LoginViaSteam : MonoBehaviour
    {
#if !DISABLESTEAMWORKS
        void Start()
        {
            // 在对象生命周期开始时就设置好回调监听
            m_AuthTicketForWebAPI = Callback<GetTicketForWebApiResponse_t>.Create(OnAuthTicketForWebAPI);
            GetComponent<Button>().onClick.AddListener(() =>
            {
                OnLoginClick();
            });
        }

        void OnLoginClick()
        {
            AuthenticateWithServer();
        }

        // 用于管理回调的成员变量
        protected Callback<GetTicketForWebApiResponse_t> m_AuthTicketForWebAPI;
        private HAuthTicket hAuthTicket;

        // 当需要验证时调用此方法
        public void AuthenticateWithServer()
        {
            Debug.Log("Requesting Steam Auth Ticket for Web API...");
            IToast.instance.show("Loading...");
            // 1. 调用 GetAuthTicketForWebAPI。
            // 这个函数会触发一个 AuthTicketForWebAPI_t 的回调，由上面的 OnAuthTicketForWebAPI 方法处理。
            // 第一个参数 "identity" 是可选的，可以传 null。
            hAuthTicket = SteamUser.GetAuthTicketForWebApi(null);            
        }

        // 2. 这是接收 GetAuthTicketForWebAPI 结果的回调函数
        private void OnAuthTicketForWebAPI(GetTicketForWebApiResponse_t callback)
        {
            // 检查回调结果是否成功
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                string errorMessage = "OnAuthTicketForWebAPI callback returned an error: " + callback.m_eResult;
                Debug.LogError(errorMessage);
                AlertBox.instance.Show(errorMessage);
                return;
            }

            // 检查回调的句柄是否与我们请求的句柄匹配
            if (callback.m_hAuthTicket != hAuthTicket)
            {
                // 这不是我们期望的回调，忽略它
                return;
            }

            Debug.Log("Successfully received Auth Ticket for Web API.");

            // 3. 直接从回调的参数中获取票据数据！
            // 无需再次调用任何 Get...Ticket 函数。
            byte[] ticketData = new byte[callback.m_cubTicket];
            Array.Copy(callback.m_rgubTicket, ticketData, callback.m_cubTicket);

            // 4. 将票据字节数组转换为十六进制字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ticketData)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            string sessionTicket = sb.ToString();

            // 5. 获取当前用户的 64 位 Steam ID
            string steamId = SteamUser.GetSteamID().m_SteamID.ToString();
            string steamNickname = SteamFriends.GetPersonaName();

            // 6. 将 Ticket 和 Steam ID 发送到服务器
            // StartCoroutine(SendAuthRequest(sessionTicket, steamId));
            LoginHandle.sessionTicket = sessionTicket;
            LoginHandle.steamId = steamId;
            LoginHandle.steamNickname = steamNickname;
            LoginHandle.via = "steam";
            IToast.instance.hide();
            SceneManager.LoadScene("Menu");
        }

#endif    
    }
}
