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
            text.text = ILang.get(text.text, type);

            //Debug.Log(text.font.name + ":" + gameObject.name + ",p:" + transform.parent.gameObject.name);        

        }
    }
	
}
