using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TapSDK.Core;
using TapSDK.Login;
using TapSDK.Compliance;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;
using Scraft;

public class TapSdkLogin : MonoBehaviour
{
    public Button TapTapLoginButton;

    public bool hasCheckedCompliance;

    async void Awake()
    {
        var preferredLanguage = TapTapLanguageType.en;
        if (GameSetting.lang == 2 || GameSetting.isAndroid)
        {
            preferredLanguage = TapTapLanguageType.zh_Hans;
        }
        // 核心配置
        TapTapSdkOptions coreOptions = new TapTapSdkOptions
        {
            // 客户端 ID，开发者后台获取
            clientId = Account.taptapClientId,
            // 客户端令牌，开发者后台获取
            clientToken = Account.taptapClientToken,
            // 地区，CN 为国内，Overseas 为海外
            region = TapTapRegionType.CN,
            // 语言，默认为 Auto，默认情况下，国内为 zh_Hans，海外为 en
            preferredLanguage = preferredLanguage,
            // 是否开启日志，Release 版本请设置为 false
            enableLog = false
        };
        // TapSDK 初始化
        TapTapSDK.Init(coreOptions);

        TapTapComplianceOption complianceOption = new TapTapComplianceOption
        {
            showSwitchAccount = true,  // 是否显示切换账号按钮
            useAgeRange = false  // 游戏是否需要获取真实年龄段信息
        };
        TapTapCompliance.RegisterComplianceCallback(ComplianceCallback);

        // 当需要添加其他模块的初始化配置项，例如合规认证、成就等， 请使用如下 API
        TapTapSdkBaseOptions[] otherOptions = new TapTapSdkBaseOptions[]
        {
            // 其他模块配置项
        };
        TapTapSDK.Init(coreOptions, otherOptions);

        TapTapLoginButton.gameObject.SetActive(false);
        TapTapLoginButton.onClick.AddListener(() =>
        {
            OnTapLoginButtonClick();
        });

        await PreCheckLoginAsync();
    }

    // 声明合规认证回调
    private void ComplianceCallback(int code, string errorMsg)
    {
        Debug.Log("ComplianceCallback");
        Debug.Log(code);
        Debug.Log(errorMsg);
        // 根据回调返回的参数 code 添加不同情况的处理
        switch (code)
        {

            case 500: // 玩家未受限制，可正常进入
                hasCheckedCompliance = true;
                Debug.Log("Can enter game");
                LoginHandle.via = "tap";
                SceneManager.LoadScene("Menu");
                // TODO: 显示开始游戏按钮
                break;
            default:
                TapTapLogin.Instance.Logout(); // 如果游戏有其他账户系统，此时也应执行退出
                AlertBox.instance.Show("防沉迷认证失败",
                () =>
                {
                    TapTapLoginButton.gameObject.SetActive(true);
                }, "退出游戏", () =>
                {
                    TapTapLoginButton.gameObject.SetActive(true);
                }, null);
                TapTapLoginButton.gameObject.SetActive(true);
                break;
        }

    }

    public void StartCheckCompliance(string userIdentifier)
    {
        hasCheckedCompliance = false;
        TapTapCompliance.Startup(userIdentifier);
    }

    /// <summary>
    /// 开启合规认证检查
    /// </summary>
    public async Task StartCheckCompliance()
    {
        // 获取当前已登录用户的 account 信息
        TapTapAccount account = null;
        try
        {
            account = await TapTapLogin.Instance.GetCurrentTapAccount();
        }
        catch (Exception exception)
        {
            Debug.Log($"获取用户信息出现异常：{exception}");
        }
        if (account == null)
        {
            // 无法获取用户信息时，登出并显示登录按钮
            TapTapLogin.Instance.Logout();
            TapTapLoginButton.gameObject.SetActive(true);
            // TODO: 显示登录按钮
            return;
        }

        // 使用当前 Tap 用户的 unionid 作为用户标识进行合规认证检查
        string userIdentifier = account.unionId;
        StartCheckCompliance(userIdentifier);
    }

    async Task PreCheckLoginAsync()
    {
        TapTapAccount account = null;
        try
        {
            // 检查本地是否已存在 account 信息
            account = await TapTapLogin.Instance.GetCurrentTapAccount();
        }
        catch (Exception e)
        {
            Debug.Log("本地无有效用户信息");
        }

        if (account == null)
        {
            // TODO: 显示登录按钮
            TapTapLoginButton.gameObject.SetActive(true);
        }
        else
        {
            // 如果当前还未通过合规认证检查，开始认证
            if (!hasCheckedCompliance)
            {
                // 开始合规认证检查
                await StartCheckCompliance();
            }
        }
    }

    public async void OnTapLoginButtonClick()
    {
        try
        {
            List<string> scopes = new List<string>
            {
                TapTapLogin.TAP_LOGIN_SCOPE_PUBLIC_PROFILE
            };
            // 发起 Tap 登录并获取用户信息
            var account = await TapTapLogin.Instance.LoginWithScopes(scopes.ToArray());

            // 开始合规认证检查
            StartCheckCompliance();
        }
        catch (Exception e)
        {
            // 登录取消或错误，提示用户重新登录
            Debug.Log("用户登录取消或错误");
        }
    }
}
