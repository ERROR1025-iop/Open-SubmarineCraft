using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Policy : MonoBehaviour
{
    public Button agreeButton;
    public Button disagreeButton;
    public Button agreementButton;
    public Button policyButton;
    void Start()
    {
        bool agree = PlayerPrefs.GetInt("policy agree", 0) == 1;
        if (agree) {
            SceneManager.LoadScene("Login");
        }
        agreeButton.onClick.AddListener(() =>
        {
            // 保存整数
            PlayerPrefs.SetInt("policy agree", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Login");
        });
        disagreeButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        agreementButton.onClick.AddListener(() =>
        {
            Application.OpenURL("https://sdk.aipie.cool/policy/user_agreement?n=方块潜艇");
        });
        policyButton.onClick.AddListener(() =>
        {
            Application.OpenURL("https://sdk.aipie.cool/policy/privacy_agreement?n=方块潜艇");
        });
    }
}
