using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class RenderSetting : MonoBehaviour
    {
        public static RenderSetting instance;

        RectTransform rectTrans;
        ISettingButton renderUnderwaterEffectButton;
        ISettingButton renderLightbeamButton;

        void Start()
        {
            instance = this;
            rectTrans = GetComponent<RectTransform>();

            renderUnderwaterEffectButton = rectTrans.GetChild(1).GetComponent<ISettingButton>();
            renderLightbeamButton = rectTrans.GetChild(2).GetComponent<ISettingButton>();
            rectTrans.GetChild(3).GetComponent<Button>().onClick.AddListener(onResumeGameButtonClick);

            renderUnderwaterEffectButton.init();
            renderUnderwaterEffectButton.setValue(GameSetting.renderUnderwaterEffect);
            renderUnderwaterEffectButton.setClickListener(onRenderUnderwaterEffectButtonClick);

            renderLightbeamButton.init();
            renderLightbeamButton.setValue(GameSetting.renderLightbeam);
            renderLightbeamButton.setClickListener(onRenderLightbeamButtonClick);

            show(false);
        }

        void onRenderUnderwaterEffectButtonClick()
        {
            renderUnderwaterEffectButton.setValue(!renderUnderwaterEffectButton.getValue());
            GameSetting.renderUnderwaterEffect = renderUnderwaterEffectButton.getValue();
            GameSetting.save();
        }

        void onRenderLightbeamButtonClick()
        {
            renderLightbeamButton.setValue(!renderLightbeamButton.getValue());
            GameSetting.renderLightbeam = renderLightbeamButton.getValue();
            GameSetting.save();
        }

        public void show(bool show)
        {
            if (show)
            {
                rectTrans.anchoredPosition = new Vector2(0, 0);
            }
            else
            {
                rectTrans.anchoredPosition = new Vector2(1000, 0);
            }
        }

        void onResumeGameButtonClick()
        {
            show(false);
        }
    }
}
