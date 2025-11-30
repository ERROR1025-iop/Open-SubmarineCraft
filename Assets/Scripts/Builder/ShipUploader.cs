using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class ShipUploader : MonoBehaviour
    {

        public Button uploadButton;
        public Button cancelButton;
        public ISettingButton priButton;
        public bool isAssembler;
        public TMPro.TMP_InputField titleInput;
        public TMPro.TMP_InputField diamond_input;
        public TMPro.TMP_InputField description_input;
        public Image cover;
        public static ShipUploader instance;
        public Sprite nullSprite;
        private RectTransform rectTransform;

        void Start()
        {
            instance = this;
            rectTransform = GetComponent<RectTransform>();
            cancelButton.onClick.AddListener(() =>
            {
                Hide();
            });
            uploadButton.onClick.AddListener(async () =>
            {
                await UploadShip();
            });
            priButton.init();
            priButton.setValue(false);
            priButton.setClickListener(() =>
            {
                priButton.setValue(!priButton.getValue());
            });
        }

        public void Show()
        {
            if (LoginHandle.userData == null || LoginHandle.userData.token1 == null)
            {
                AlertBox.instance.Show("请先登录\nPlease log in first");
                return;
            }
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }

        public void Hide()
        {
            rectTransform.anchoredPosition = new Vector2(0, 1000);
        }

        public void SetCover(string filepath)
        {
            cover.sprite = nullSprite;
            Texture2D texture2D;
            try
            {
                texture2D = IUtils.loadTexture2DFromSD(filepath);
                texture2D.filterMode = FilterMode.Point;
            }
            catch
            {
                return;
            }
            cover.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            IUtils.resetImageSize(cover, new Vector2(120, 80));
        }

        public async Task UploadShip()
        {
            if (LoginHandle.userData == null || LoginHandle.userData.token1 == null)
            {
                AlertBox.instance.Show("请先登录\nPlease log in first");
                return;
            }
            uploadButton.enabled = false;
            string cover_path = GamePath.cacheFolder + "thumbnail.cache";
            string ship_data_path = Path.Combine(GamePath.cacheFolder, "upload.cache" +(isAssembler? ".ass": ".ship"));
            var request = new HttpRequest(NetworkFactory.SCRAFT_HOST + "/ship/upload");
            string pri = priButton.getValue()?"1":"0";
            request.addFormData("title", titleInput.text);
            request.addFormData("diamond", diamond_input.text);
            request.addFormData("description", description_input.text);
            request.addFormData("cover", Base64Helper.FileToBase64String(cover_path));
            request.addFormData("ship_data", Base64Helper.FileToBase64String(ship_data_path));
            request.addFormData("app_device", GameSetting.appDevice);
            request.addFormData("app_channel", GameSetting.appChannel);
            request.addFormData("app_version", Application.version);
            request.addFormData("token1", LoginHandle.userData.token1);
            request.addFormData("is_assembler", isAssembler ? "1" : "0");
            request.addFormData("is_pri", pri);
            IToast.instance.show("上传中...");
            HttpResponse response = await NetworkFactory.getHttpNet().PostAsync(request);
            IToast.instance.hide();
            if (response.code == 200)
            {
                Hide();
                AlertBox.instance.Show("上传成功");
                uploadButton.enabled = true;
                if (DiamondView.instance != null)
                {
                    DiamondView.instance.UpdateDiamondCount();
                }
            }
            else
            {
                AlertBox.instance.Show("上传失败:" + response.body);
                uploadButton.enabled = true;
            }
        }
    } 
}
