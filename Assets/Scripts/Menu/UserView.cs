using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scraft;
using TapSDK.Login;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserView : MonoBehaviour
{
    public TMPro.TextMeshProUGUI userNameText;
    public Button userInfoButton;
    private bool login = false;
    
    async void Start()
    {
        userInfoButton.onClick.AddListener(() => AlertBox.instance.Show("退出登录?", () =>
        {
            LoginHandle.userData = null;
            if(TapTapLogin.Instance != null)
            {
                TapTapLogin.Instance.Logout();
            }            
            SceneManager.LoadScene("Login");
        }));
        
        await PollLoginStatusAsync();
    }

    async Task PollLoginStatusAsync()
    {
        while (!login)
        {
            login = await UpdateUserInfoAsync();
            if (!login)
            {
                // 等待1秒后继续尝试
                await Task.Delay(1000);
            }
        }
    }

    async Task<bool> UpdateUserInfoAsync()
    {
        if (LoginHandle.userData != null)
        {
            userNameText.text = LoginHandle.userData.nickname;
            return true;
        }
        if (LoginHandle.via == "null")
        {
            userNameText.text = "";
            return true;
        }
        return false;
    }
}