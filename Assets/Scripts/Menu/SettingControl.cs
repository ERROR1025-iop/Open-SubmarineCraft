using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InsaneSystems.InputManager;
using InsaneSystems.InputManager.UI;
using UnityEngine.SceneManagement;
namespace Scraft
{
    public class SettingControl : MonoBehaviour
    {

        public Settings setting;


        void Start()
        {
            GameObject.Find("Confirm").GetComponent<Button>().onClick.AddListener(onConfirmButtonClick);
            GameObject.Find("Reset").GetComponent<Button>().onClick.AddListener(onResetButtonClick);
        }

        void onConfirmButtonClick()
        {
            setting.SaveSettings();
            SceneManager.LoadScene("Setting");
        }

        void onResetButtonClick()
        {
            setting.ResetSettings();
        }

        void Update()
        {

        }
    }
}