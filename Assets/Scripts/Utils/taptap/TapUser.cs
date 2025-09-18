using System.Collections;
using System.Collections.Generic;
using TapSDK.Login;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TapUser : MonoBehaviour
{
    public TMPro.TextMeshProUGUI userNameText;
    public Button userInfoButton;
    async void Start()
    {
        TapTapAccount taptapAccount = await TapTapLogin.Instance.GetCurrentTapAccount();
        if (taptapAccount != null)
        {
            userNameText.text = taptapAccount.name;

            userInfoButton.onClick.AddListener(() => AlertBox.instance.Show("退出登录?", () =>
            {
                TapTapLogin.Instance.Logout();
                SceneManager.LoadScene("Login");
            }));
        }
        else
        {
            userInfoButton.gameObject.SetActive(false);
        }        
    }
}
