using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft{ public class IFontCheck : MonoBehaviour
    {
        public Font orgFont;
        public Font toFont;
        bool isChange;

        [ContextMenu("Check Font")]
        void Start()
        {
            if (GameSetting.isAndroid)
            {
                return;
            }

            //Debug.Log("start check font!");

            Text[] texts = FindObjectsOfType<Text>();
            foreach (Text text in texts)
            {
                if (text.font == null)
                {
                    Debug.Log("[Warning!]Miss Font:" + text.text + ",go name:" + text.gameObject.name + ",parent:" + text.transform.parent.gameObject.name);
                }

                if (text.font.name.Equals("Arial") || text.font.Equals(orgFont))
                {
                    if (text.transform.parent != null)
                    {
                        Debug.Log("[Warning!]" + text.text + ",font:" + text.font.name + ",go name:" + text.gameObject.name + ",parent:" + text.transform.parent.gameObject.name);
                    }
                    else
                    {
                        Debug.Log("[Warning!]" + text.text + ",font:" + text.font.name + ",go name:" + text.gameObject.name + ",parent:null");
                    }
                }

            }


        }

        [ContextMenu("Change Font")]
        void changeFont()
        {
            Debug.Log("start change font!");
            Text[] texts = FindObjectsOfType<Text>();
            foreach (Text text in texts)
            {
                if (text.font == null || text.font.Equals(orgFont))
                {
                    text.font = toFont;
                    Debug.Log("change font[" + text.text + "][" + orgFont.name + "]=>[" + toFont + "]");
                }
            }
        }

        [ContextMenu("Change Bold To Normal")]
        void changeTextBold2Normal()
        {
            Debug.Log("start change Bold to Normal!");
            Text[] texts = FindObjectsOfType<Text>();
            foreach (Text text in texts)
            {
                if (text.fontStyle.Equals(FontStyle.Bold))
                {
                    text.fontStyle = FontStyle.Normal;
                    Debug.Log("change font[" + text.text + "][Bold]=>[Normal]");
                }
            }
        }
    }
}
