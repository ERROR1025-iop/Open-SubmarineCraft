using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    [Serializable]
    public class SDData
    {
        public int ID;
        public string title;
        public string user_id;
        public string nickname;
        public string describe;
        public string isAssembler;
        public string bitmap;
        public int sale;
        public string version;
        public string create_time;
        public string update_time;
        public int download;
        public int favour;
        public int reply;
    }
    public class SDDataCell : MonoBehaviour
    {
        public TMPro.TMP_Text titleText;
        public TMPro.TMP_Text saleText;
        public TMPro.TMP_Text favText;
        public TMPro.TMP_Text dowText;
        public TMPro.TMP_Text authorText;
        public Image cover;
        public Button button;
        public SDData sdData;
        public void SetSDData(SDData sd)
        {
            sdData = sd;
            if (sdData == null)
            {
                gameObject.SetActive(false); 
                return;
            }
            else
            {
                gameObject.SetActive(true);
                titleText.text = sdData.title;
                saleText.text = sdData.sale.ToString();
                favText.text = sdData.favour.ToString();
                dowText.text = sdData.download.ToString();
                authorText.text = sdData.nickname.ToString();
                Base64Helper.SetImageFromBase64(sdData.bitmap, cover, new Vector2(137, 60));
                button.onClick.AddListener(() =>
                {
                    if(SDDetailPage.instance != null)
                        SDDetailPage.instance.SetSDData(sdData);
                });
            }
        }
    }
}


