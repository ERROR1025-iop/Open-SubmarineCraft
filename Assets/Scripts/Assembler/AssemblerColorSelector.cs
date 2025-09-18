using RuntimeInspectorNamespace;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Scraft
{
    public class AssemblerColorSelector : MonoBehaviour
    {

        public static AssemblerColorSelector instance;
        public static Color color;

        public ColorWheelControl colorWheelControl;

        bool isShow;
        RectTransform mainTrans;
        UnityAction call;

        void Start()
        {
            instance = this;
            mainTrans = GetComponent<RectTransform>();
            mainTrans.GetChild(1).GetComponent<Button>().onClick.AddListener(onConfirmButtonClick);
            mainTrans.GetChild(2).GetComponent<Button>().onClick.AddListener(onCancelButtonClick);

            show(false);
        }

        void onConfirmButtonClick()
        {
            color = colorWheelControl.Color;
            if (call != null)
            {
                call();
            }
            show(false);
        }

        void onCancelButtonClick()
        {
            show(false);
        }

        public void show(UnityAction action)
        {
            call = action;
            show(true);
        }

        public void show(bool show)
        {
            isShow = show;
            if (show)
            {
                mainTrans.anchoredPosition = new Vector2(162f, -193f);
            }
            else
            {
                mainTrans.anchoredPosition = new Vector2(9999f, 0);
            }
        }
    }
}