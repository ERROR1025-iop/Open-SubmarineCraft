using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    [Serializable]
    public class CoinData
    {
        public string coin;
    }

    public class DiamondView : MonoBehaviour
    {

        public static DiamondView instance;
        public Text diamondCountText;
        public Button addButton;

        async void Start()
        {
            instance = this;
            UpdateDiamondCount();
            addButton.onClick.AddListener(() =>
            {
                Application.OpenURL("https://docs.qq.com/doc/DVEpZaGJac01idVBp");
            });
        }

        public async void UpdateDiamondCount()
        {
            var coin = await GetUserCoin();
            diamondCountText.text = coin.ToString("0");
        }


        public static async Task<float> GetUserCoin()
        {
            if (LoginHandle.userData == null || LoginHandle.userData.token1 == null)
            {
                return 0;
            }
            var request = new HttpRequest(NetworkFactory.AUTH_HOST + "/coin/count");
            request.addFormData("user_id", LoginHandle.userData.user_id);
            request.addFormData("user_code", LoginHandle.userData.user_code);
            HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
            if (response.code == 200)
            {
                var jsonResponse = new HttpJsonResponse<CoinData>(response);
                if (jsonResponse.code > 0)
                {
                    GameSetting.diamonds = float.Parse(jsonResponse.data.coin);
                    return GameSetting.diamonds;
                }
                else
                {
                    Debug.Log("获取钻石失败:" + jsonResponse.data);
                }

            }
            else
            {
                Debug.Log("获取钻石失败:" + response.body);
            }
            return 0;
        }
    }
}
