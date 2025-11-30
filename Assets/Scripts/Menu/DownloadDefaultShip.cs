using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Scraft {
    public class DownloadDefaultShip : MonoBehaviour
    {
        private bool login = false;
        void Start()
        {            
            StartCoroutine(PollLoginStatus());
        }
       

        IEnumerator PollLoginStatus()
        {
            while (!login)
            {
                login = DoDownload();
                if (!login)
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            yield return null;
        }

        bool DoDownload()
        {
            if (LoginHandle.userData == null || LoginHandle.userData.token1 == null)
            {
                return false;
            }
            if (GameSetting.isAndroid)
            {
                DoDownloadReal(3354, "默认潜艇.ass");
                DoDownloadReal(3353, "小型采矿船.ass");
                DoDownloadReal(3350, "二战潜艇.ass");
                DoDownloadReal(3359, "教程潜艇.ship");
                DoDownloadReal(3375, "常规潜艇.ship");
                //DoDownloadReal(3360, "水下机器人.ship");
                //DoDownloadReal(3361, "战舰.ship");
                //DoDownloadReal(3362, "二战潜艇.ship");
            }
            else
            {
                DoDownloadReal(3354, "Default Sub.ass");
                DoDownloadReal(3353, "Small Mining Ship.ass");
                DoDownloadReal(3350, "WWII Sub.ass");
                DoDownloadReal(3359, "Tutorial Submarine.ship");
                DoDownloadReal(3375, "Conventional Submarine.ship");
                //DoDownloadReal(3360, "Underwater Robot.ship");
                //DoDownloadReal(3361, "Warship.ship");
                //DoDownloadReal(3362, "WWII Submarine.ship");
            }
            return true;
        }

        async void DoDownloadReal(int id, string title)
        {            
            string filepath = (title.EndsWith(".ass")?GamePath.dpartFolder:GamePath.shipsFolder) + title;
            if (File.Exists(filepath))
            {
                return;
            }
            var request = new HttpRequest(NetworkFactory.SCRAFT_HOST + "/ship/download");
            request.addFormData("id", id.ToString());
            request.addFormData("token1", LoginHandle.userData.token1);
            request.addFormData("m", "1");
            HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
            if (response.code == 200)
            {
                Base64Helper.SaveBase64StringToFile(response.body, filepath);
                Debug.Log("下载成功 Download Success");
            }
            else
            {
                Debug.Log("下载失败 Download Failed:" + response.body);
            }
        }
    }
}
