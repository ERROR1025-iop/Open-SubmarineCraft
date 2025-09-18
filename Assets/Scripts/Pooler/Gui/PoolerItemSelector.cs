using Scraft.DpartSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class PoolerItemSelector : MonoBehaviour
    {
        static public PoolerItemSelector instance;

        static public int item3DLayer;
        static public SelectorRS mainSelectorRS;
        static public bool isLocal;

        public delegate void Custom1ButtonClickDelegate();
        public event Custom1ButtonClickDelegate OnCustom1ButtonClick;
        public delegate void Custom2ButtonClickDelegate();
        public event Custom2ButtonClickDelegate OnCustom2ButtonClick;
        public delegate void SelectedDelegate();
        public event SelectedDelegate OnSelected;
        public delegate void CancelButtonClickDelegate();
        public event CancelButtonClickDelegate OnCancelButtonClick;
        public delegate void CancelByHitOtherDelegate();
        public event CancelByHitOtherDelegate OnCancelByHitOther;
               
        RectTransform mainTrans;
        bool isShow;

        public IJoystick joystick1;
        public IJoystick joystick2;
        public IJoystick joystick3;

        IChangeImageButton localButton;
        Text localText;

        GameObject custom1ButtonGo;
        Text custom1ButtonText;

        GameObject custom2ButtonGo;
        Text custom2ButtonText;

        Camera Camera3DWorld;

        float clickTime;

        void Start()
        {
            instance = this;

            mainTrans = GetComponent<RectTransform>();

            localButton = transform.GetChild(0).GetComponent<IChangeImageButton>();
            localText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
            localButton.addListener(onLocalButtonClick);

            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(onCancelButtonClick);

            custom1ButtonGo = transform.GetChild(2).gameObject;
            custom1ButtonGo.GetComponent<Button>().onClick.AddListener(onCustom1ButtonClick);
            custom1ButtonText = custom1ButtonGo.transform.GetChild(0).GetComponent<Text>();

            custom2ButtonGo = transform.GetChild(3).gameObject;
            custom2ButtonGo.GetComponent<Button>().onClick.AddListener(onCustom2ButtonClick);
            custom2ButtonText = custom2ButtonGo.transform.GetChild(0).GetComponent<Text>();



            show(false);

            isLocal = false;

            Camera3DWorld = Camera.main;

            item3DLayer = 1 << 12;
        }

        public IJoystick GetJoystick1()
        {
            return joystick1;
        }

        public IJoystick GetJoystick2()
        {
            return joystick2;
        }

        public IJoystick GetJoystick3()
        {
            return joystick3;
        }

        void onLocalButtonClick()
        {
            isLocal = localButton.value;
            localText.text = ILang.get(isLocal ? "Local" : "Global");
            if (mainSelectorRS != null)
            {
                mainSelectorRS.onLocalButtonClick();
            }
        }

        void onCancelButtonClick()
        {
            if (OnCancelButtonClick != null)
            {
                OnCancelButtonClick();
            }
            if (mainSelectorRS != null)
            {
                mainSelectorRS.cancel();
            }
            show(false);
        }

        void onCustom1ButtonClick()
        {
            if (OnCustom1ButtonClick != null)
            {
                OnCustom1ButtonClick();
            }
        }

        void onCustom2ButtonClick()
        {
            if (OnCustom2ButtonClick != null)
            {
                OnCustom2ButtonClick();
            }
        }

        public void setCustom1ButtonText(string text)
        {
            custom1ButtonText.text = text;
        }

        public void setCustom2ButtonText(string text)
        {
            custom2ButtonText.text = text;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickTime = 0;
            }
            clickTime += Time.deltaTime;

            select3DItem();
        }

        void select3DItem()
        {
            if (Input.GetMouseButtonUp(0) && clickTime < 0.2f && !IUtils.isPointGUI())
            {
                Ray ray = Camera3DWorld.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, item3DLayer))
                {
                    SelectorRS hitSelectorRS = hit.collider.transform.parent.GetComponent<SelectorRS>();
                    if (hitSelectorRS != null)
                    {                       
                        if (mainSelectorRS != null && !hitSelectorRS.Equals(mainSelectorRS))
                        {
                            if (OnCancelByHitOther != null)
                            {
                                OnCancelByHitOther();
                            }
                            mainSelectorRS.cancel();
                        }

                        mainSelectorRS = hitSelectorRS;
                        hitSelectorRS.onRaycastHit();

                        custom1ButtonGo.SetActive(mainSelectorRS.hasCustomButton1);
                        if (mainSelectorRS.hasCustomButton1)
                        {
                            custom1ButtonText.text = ILang.get(mainSelectorRS.custom1ButtonName);
                        }

                        custom2ButtonGo.SetActive(mainSelectorRS.hasCustomButton2);
                        if (mainSelectorRS.hasCustomButton2)
                        {
                            custom2ButtonText.text = ILang.get(mainSelectorRS.custom2ButtonName);
                        }

                        if (OnSelected != null)
                        {
                            OnSelected();
                        }
                        joystick1.gameObject.SetActive(mainSelectorRS.hasJoystick1);
                        joystick2.gameObject.SetActive(mainSelectorRS.hasJoystick2);
                        joystick3.gameObject.SetActive(mainSelectorRS.hasJoystick3);
                    }
                }
            }
        }

        public void show(bool show)
        {
            isShow = show;
            if (show)
            {
                mainTrans.anchoredPosition = new Vector2(88.32f, -165f);
            }
            else
            {
                mainTrans.anchoredPosition = new Vector2(9999f, 0);
                joystick1.gameObject.SetActive(false);
                joystick2.gameObject.SetActive(false);
                joystick3.gameObject.SetActive(false);
            }
        }
    }
}