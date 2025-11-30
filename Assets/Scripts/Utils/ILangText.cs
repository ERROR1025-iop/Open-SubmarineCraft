using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft{ public class ILangText : MonoBehaviour
    {

        public string type = "menu";

        void Start()
        {
            Text text = transform.GetComponent<Text>();
            if (text != null)
                text.text = ILang.get(text.text, type);
            TMPro.TMP_Text text2 = transform.GetComponent<TMPro.TextMeshProUGUI>();
            if (text2 != null)
                text2.text = ILang.get(text2.text, type);

            //Debug.Log(text.font.name + ":" + gameObject.name + ",p:" + transform.parent.gameObject.name);        

        }
    }
	
}
