using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class SDDetailPage : MonoBehaviour
    {
        public SDMain sDMain;
        public TMPro.TMP_Text titleText;
        public TMPro.TMP_Text diaText;
        public TMPro.TMP_Text authorText;
        public TMPro.TMP_Text saleText;
        public TMPro.TMP_Text descText;
        public TMPro.TMP_Text favText;
        public TMPro.TMP_Text updateText;
        public Button backButton;
        public Button downloadButton;
        public Button delButton;
        public IChangeImageButton favButton;
        public IChangeImageButton priButton;
        public Image cover;
        public SDData sDData;
        private RectTransform rectTransform;
        static public SDDetailPage instance;

        private int isFavour = -1;
        private int isPri = -1;

        void Start()
        {
            instance = this;
            rectTransform = GetComponent<RectTransform>();
            backButton.onClick.AddListener(() =>
            {
                Hide();
            });
            downloadButton.onClick.AddListener(() =>
            {
                DoDownload();
            });
            delButton.onClick.AddListener(async () =>
            {
                await DoDelete();
            });
            favButton.addListener(async () =>
            {
                await DoFavor();
            });
            priButton.addListener(async () =>
            {
                await DoPrivate();
            });           
        }

        public void SetSDData(SDData sd)
        {
            sDData = sd;
            UpdateSelfButtonActivate();
            if (LoginHandle.userData == null || LoginHandle.userData.token1 == null)
            {
                AlertBox.instance.Show("请先登录\nPlease log in first");
                return;
            }
            rectTransform.anchoredPosition = new Vector2(0, 0);

            titleText.text = sd.title;
            diaText.text = sd.sale.ToString();
            authorText.text = sd.nickname.ToString();
            saleText.text = sd.sale.ToString();
            descText.text = sd.describe.ToString();
            updateText.text = sd.update_time.ToString();
            Base64Helper.SetImageFromBase64(sd.bitmap, cover, new Vector2(137, 129));
            _ = GetFavor(); // 触发任务，不等待
            _ = GetPrivate();
        }

        void UpdateSelfButtonActivate()
        {
            if (LoginHandle.userData != null || LoginHandle.userData.token1 != null)
            {
                if (LoginHandle.userData.user_id == sDData.user_id)
                {
                    delButton.gameObject.SetActive(true);
                    priButton.gameObject.SetActive(true);
                    return;
                }
            }
            delButton.gameObject.SetActive(false);
            priButton.gameObject.SetActive(false);
        }

        public void Hide()
        {
            rectTransform.anchoredPosition = new Vector2(0, 1000);
        }

        async void DoDownload()
        {
            if (LoginHandle.userData == null || LoginHandle.userData.token1 == null)
            {
                AlertBox.instance.Show("请先登录\nPlease log in first");
                return;
            }
            if (sDData == null)
                return;
            if (LoginHandle.userData.user_id != sDData.user_id && sDData.sale > 0)
            {
                if (GameSetting.lang == 0)
                {
                    AlertBox.instance.Show("Are you sure you want to spend " + sDData.sale.ToString() + " diamonds to download?", DoDownloadReal);
                }
                if (GameSetting.lang == 2)
                {
                    AlertBox.instance.Show("确定消耗 " + sDData.sale.ToString() + " 钻石下载？", DoDownloadReal);
                }
            }
            else
            {                
                DoDownloadReal();
            }
        }

        async void DoDownloadReal()
        {
            downloadButton.enabled = false;
            var request = new HttpRequest(NetworkFactory.SCRAFT_HOST + "/ship/download");
            request.addFormData("id", sDData.ID.ToString());
            request.addFormData("token1", LoginHandle.userData.token1);
            IToast.instance.show("Downloading...");
            HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
            IToast.instance.hide();
            if (response.code == 200)
            {
                string filepath = sDData.isAssembler == "1" ?
                    (GamePath.dpartFolder + sDData.title + ".ass") : (GamePath.shipsFolder + sDData.title + ".ship");
                Base64Helper.SaveBase64StringToFile(response.body, filepath);
                IToast.instance.show("下载成功 Download Success", 200);
                if (DiamondView.instance != null)
                {
                    DiamondView.instance.UpdateDiamondCount();
                }
            }
            else
            {
                AlertBox.instance.Show("下载失败 Download Failed:" + response.body);
            }
            downloadButton.enabled = true;
        }

        async Task GetFavor()
        {
            favButton.enabled = false;
            var request = new HttpRequest(NetworkFactory.SCRAFT_HOST + "/ship/favour");
            request.addFormData("ship_id", sDData.ID.ToString());
            request.addFormData("token1", LoginHandle.userData.token1);
            HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
            if (response.code == 200)
            {
                isFavour = int.Parse(response.body);
                favButton.setValue(isFavour == 1);
            }
            else
            {
                AlertBox.instance.Show("GetFavor Failed:" + response.body);
            }
            favButton.enabled = true;
        }

        async Task GetPrivate()
        {
            priButton.enabled = false;
            var request = new HttpRequest(NetworkFactory.SCRAFT_HOST + "/ship/pri");
            request.addFormData("ship_id", sDData.ID.ToString());
            request.addFormData("token1", LoginHandle.userData.token1);
            HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
            if (response.code == 200)
            {
                isPri = int.Parse(response.body);
                priButton.setValue(isPri == 1);
            }
            else
            {
                AlertBox.instance.Show("GetPrivate Failed:" + response.body);
            }
            priButton.enabled = true;
        }

        async Task DoFavor()
        {
            if(isFavour == -1)
                return;
            favButton.enabled = false;
            var request = new HttpRequest(NetworkFactory.SCRAFT_HOST + "/ship/favour/set");
            request.addFormData("ship_id", sDData.ID.ToString());
            request.addFormData("token1", LoginHandle.userData.token1);
            request.addFormData("action", isFavour == 1 ? "0" : "1");
            IToast.instance.show("Loading...");
            HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
            IToast.instance.hide();
            if (response.code == 200)
            {
                isFavour = int.Parse(response.body);
                favButton.setValue(isFavour == 1);
            }
            else
            {
                AlertBox.instance.Show("DoFavor Failed:" + response.body);
            }
            favButton.enabled = true;
        }
        
        async Task DoDelete()
        {
            AlertBox.instance.Show("确定删除？Confirm deletion?" , RealDelete);
        }

        async void RealDelete()
        {
            priButton.enabled = false;
            var request = new HttpRequest(NetworkFactory.SCRAFT_HOST + "/ship/del/set");
            request.addFormData("ship_id", sDData.ID.ToString());
            request.addFormData("token1", LoginHandle.userData.token1);
            IToast.instance.show("Loading...");
            HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
            IToast.instance.hide();
            if (response.code == 200)
            {
                Hide();
                await sDMain.GetShipList();
                AlertBox.instance.Show("删除成功");
            }
            else
            {
                AlertBox.instance.Show("DoDelete Failed:" + response.body);
            }
            priButton.enabled = true;
        }

        async Task DoPrivate()
        {
            if(isPri == -1)
                return;
            priButton.enabled = false;
            var request = new HttpRequest(NetworkFactory.SCRAFT_HOST + "/ship/pri/set");
            request.addFormData("ship_id", sDData.ID.ToString());
            request.addFormData("token1", LoginHandle.userData.token1);
            request.addFormData("action", isPri == 1 ? "0" : "1");
            IToast.instance.show("Loading...");
            HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
            IToast.instance.hide();
            if (response.code == 200)
            {
                isPri = int.Parse(response.body);
                priButton.setValue(isPri == 1);
            }
            else
            {
                AlertBox.instance.Show("DoPrivate Failed:" + response.body);
            }
            priButton.enabled = true;
        }          
    } 
}
