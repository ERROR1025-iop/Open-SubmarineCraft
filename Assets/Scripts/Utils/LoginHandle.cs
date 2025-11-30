using System;
using System.Collections;
using UnityEngine;
using TapSDK.Login;
using System.Threading.Tasks;

namespace Scraft
{    

    [Serializable]
    public class UserData
    {
        public string token1;       
        public string user_id;     
        public string user_code;    
        public string nickname; 
        public string refresh_token;                            
    }

    [Serializable]
    public class TokensData
    {
        public string token1;
        public string refresh_token;
    }

    public class LoginHandle : MonoBehaviour
    {

        static public UserData userData;
        public static LoginHandle instance;
        public static string via;
        private const float INTERVAL = 20 * 60f; // 1200秒
        private bool isTimerRunning = false;

        public static string steamId;
        public static string steamNickname;
        public static string sessionTicket;

        void Awake()
        {
            // 实现单例，防止重复创建
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // 关键：不随场景销毁
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        IEnumerator Start()
        {
            if(via == "null")
            {
                yield break;
            }
            // 先等待登录完成
            yield return StartCoroutine(AwaitAsync(DoLoginAsync()));

            // 再启动定时器
            if (!isTimerRunning)
            {
                StartCoroutine(TimerCoroutine());
                isTimerRunning = true;
            }
        }

        private IEnumerator TimerCoroutine()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(INTERVAL);
                yield return StartCoroutine(AwaitAsync(RefToken()));
            }
        }

        // 包装 Task 为 IEnumerator
        private IEnumerator AwaitAsync(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                Debug.LogError($"Task failed: {task.Exception}");
            }
        }

        public async Task DoLoginAsync()
        {
            if (via == "tap")
            {
                TapTapAccount taptapAccount = await TapTapLogin.Instance.GetCurrentTapAccount();
                if (taptapAccount != null)
                {
                    string kid = taptapAccount.accessToken.kid;
                    string mac_key = taptapAccount.accessToken.macKey;
                    await DoLogin(kid, mac_key, "tap", GameSetting.appChannel);
                }
            }
            if (via == "steam" && sessionTicket != null)
            {
                await DoLogin(steamId, sessionTicket, "steam", GameSetting.appChannel, steamNickname);
            }
        }

        public async Task DoLogin(string userCode, string captcha, string entrance, string appChannel, string steamNickname = null)
        {
            var request = new HttpRequest(NetworkFactory.AUTH_HOST + "/user/login");
            request.addFormData("user_code", userCode);
            request.addFormData("captcha", captcha);
            if (steamNickname != null)
            {
                request.addFormData("nickname", steamNickname);
            }
            request.addFormData("entrance", entrance);
            request.addFormData("app_device", GameSetting.appDevice);
            request.addFormData("app_channel", appChannel);
            HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
            OnLoginCallback(response);
        }

        private void OnLoginCallback(HttpResponse response)
        {
            var jsonResponse = new HttpJsonResponse<UserData>(response);

            if (jsonResponse.code > 0)
            {
                // 登录/注册成功

                // 1. 保存用户数据（模拟 Cookies）
                userData = jsonResponse.data;
                PlayerPrefs.SetString("user", JsonUtility.ToJson(jsonResponse.data));
                PlayerPrefs.Save();

                // 3. 区分登录和注册
                if (jsonResponse.code == 2)
                {
                    Debug.Log("登录成功");
                }
                else if (jsonResponse.code == 1)
                {
                    Debug.Log("注册成功");
                }
                if (DiamondView.instance != null)
                {
                    DiamondView.instance.UpdateDiamondCount();
                }

            }
            else
            {
                // 登录失败，显示错误
                string errorMsg = !string.IsNullOrEmpty(jsonResponse.message)
                    ? jsonResponse.message
                    : "未知错误";

                Debug.LogError($"[{jsonResponse.code}]{errorMsg}");
                AlertBox.instance.Show($"[{jsonResponse.code}]{errorMsg}");
            }
        }

        private async Task RefToken()
        {
            if(userData == null || userData.token1 == null)
            {
                return;
            }
            try
            {
                var request = new HttpRequest(NetworkFactory.AUTH_HOST + "/verify/token1/refresh");
                request.addFormData("refresh_token", userData?.refresh_token);

                if (string.IsNullOrEmpty(userData?.refresh_token))
                {
                    Debug.LogError("refresh_token 为空，无法刷新");
                    return;
                }

                HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
                var jsonResponse = new HttpJsonResponse<TokensData>(response);

                if (jsonResponse.code > 0)
                {
                    userData.token1 = jsonResponse.data.token1;
                    userData.refresh_token = jsonResponse.data.refresh_token;
                    // 更新本地存储
                    PlayerPrefs.SetString("user", JsonUtility.ToJson(jsonResponse.data));
                    PlayerPrefs.Save();
                    Debug.Log("Token 刷新成功");
                }
                else
                {
                    Debug.LogError($"Token 刷新失败: [{jsonResponse.code}]{jsonResponse.message}");
                    // 可考虑登出或重新登录
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("刷新 Token 时发生异常: " + e.Message);
                // 可以尝试重试一次？或者标记状态？
            }
        }

        private void LoadHomePage()
        {
            // 在 Unity 中切换场景或打开主界面
            // 例如：
            // SceneManager.LoadScene("HomeScene");
            // 或
            // UIManager.Open<HomePanel>();

            Debug.Log("跳转到首页");
        }
    }
}