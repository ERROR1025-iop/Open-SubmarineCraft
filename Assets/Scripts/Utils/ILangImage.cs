using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft {
    public class ILangImage : MonoBehaviour
    {

        public List<Sprite> langSprite;

        Image image;

        void Start()
        {           
            if(langSprite.Count > GameSetting.lang)
            {
                if(langSprite[GameSetting.lang] != null)
                {
                    image = GetComponent<Image>();
                    image.sprite = langSprite[GameSetting.lang]; 
                }                
            }   
        }
    }
}
