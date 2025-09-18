using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Scraft
{
    public class IConfigBox : MonoBehaviour
    {
        static public IConfigBox instance;
        GameObject boxObject;
        Text nameText;
        Button yesButton;
        Button noButton;

        UnityAction yesCall;
        UnityAction noCall;

        void Awake()
        {
            instance = this;
            boxObject = gameObject;
            nameText = transform.GetChild(0).GetComponent<Text>();
            yesButton = transform.GetChild(1).GetComponent<Button>();
            noButton = transform.GetChild(2).GetComponent<Button>();

            yesButton.onClick.AddListener(onYesButtonClick);
            noButton.onClick.AddListener(onNoButtonClick);

            hide();
        }

        void onYesButtonClick()
        {
            if (yesCall != null)
            {
                yesCall();
            }
            hide();
        }

        void onNoButtonClick()
        {
            if (noCall != null)
            {
                noCall();
            }
            hide();
        }

        public void show(string str, UnityAction yesListener, UnityAction noListener)
        {
            nameText.text = str;
            yesCall = yesListener;
            noCall = noListener;
            boxObject.SetActive(true);

        }

        public void hide()
        {
            boxObject.SetActive(false);
            yesCall = null;
            noCall = null;
        }
    }
}
