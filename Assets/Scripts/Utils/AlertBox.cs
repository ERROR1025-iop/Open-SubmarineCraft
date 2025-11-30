using System.Collections;
using System.Collections.Generic;
using Scraft;
using UnityEngine;
using UnityEngine.UI;

public class AlertBox : MonoBehaviour
{
    public static AlertBox instance;
    public TMPro.TextMeshProUGUI msg;
    public TMPro.TextMeshProUGUI confirm;
    public TMPro.TextMeshProUGUI cancel;
    public Button confirmButton;
    public Button cancelButton;
    public RectTransform confirmRT;
    async void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        confirmRT = confirmButton.GetComponent<RectTransform>();
    }

    public void Show(string msg, System.Action confirmAction = null, string confirm = "确定", System.Action cancelAction = null, string cancel = "取消")
    {
        gameObject.SetActive(true);
        if(GameSetting.lang != 2)
        {
            if (confirm == "确定")
            {
                confirm = "Confirm";
            }
            if(cancel == "取消")
            {
                cancel = "Cancel";
            }
        }
        if (cancel == null || cancel == "")
        {
            confirmRT.anchoredPosition = new Vector2(0, confirmRT.anchoredPosition.y);
            cancelButton.gameObject.SetActive(false);
        }
        else
        {
            confirmRT.anchoredPosition = new Vector2(61, confirmRT.anchoredPosition.y);
            cancelButton.gameObject.SetActive(true);
        }

        this.msg.text = msg;
        this.confirm.text = confirm;
        this.cancel.text = cancel;
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (confirmAction != null)
                confirmAction();
            gameObject.SetActive(false);
        });
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            if (cancelAction != null)
                confirmAction();
            gameObject.SetActive(false);
        });
    }
    
    static public void ShowNotImplemented()
    {
        if(GameSetting.lang == 2)
        {
            instance.Show("功能尚未实现，敬请期待下个版本", null);
        }
        else
        {
            instance.Show("This function is not implemented yet, please wait for the next version", null);
        }
    }
}
