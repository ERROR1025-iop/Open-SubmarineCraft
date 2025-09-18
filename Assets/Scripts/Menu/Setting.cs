using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace Scraft {
    public class Setting : MonoBehaviour
    {
        public ISettingButton languageButton;
        public ISettingButton aiButton;
        public ISettingButton careerButton;
        public ISettingButton channel100Button;
        public ISettingButton musicButton;
        public ISettingButton renderUnderwaterEffectButton;
        public ISettingButton renderLightbeamButton;

        public Button resetCareerButton;

        void Start()
        {
            languageButton.init();
            languageButton.setValue(GameSetting.lang == 2);
            languageButton.setClickListener(onLanguageButtonClick);

            aiButton.init();
            aiButton.setValue(GameSetting.isCreateAi);
            aiButton.setClickListener(onAiButtonClick);

            careerButton.init();
            careerButton.setValue(GameSetting.isCareer);
            careerButton.setClickListener(onCareerButtonClick);

            channel100Button.init();
            channel100Button.setValue(GameSetting.isChannel100Activity);
            channel100Button.setClickListener(onChannel100ButtonClick);

            musicButton.init();
            musicButton.setValue(GameSetting.isMusicOpen);
            musicButton.setClickListener(onMusicButtonClick);

            renderUnderwaterEffectButton.init();
            renderUnderwaterEffectButton.setValue(GameSetting.renderUnderwaterEffect);
            renderUnderwaterEffectButton.setClickListener(onRenderUnderwaterEffectButtonClick);

            renderLightbeamButton.init();
            renderLightbeamButton.setValue(GameSetting.renderLightbeam);
            renderLightbeamButton.setClickListener(onRenderLightbeamButtonClick);

            resetCareerButton.onClick.AddListener(onResetCareerButtonClick);

            GameObject.Find("Canvas/Back").GetComponent<Button>().onClick.AddListener(onBackButtonClick);
        }

        void onLanguageButtonClick()
        {
            languageButton.setValue(!languageButton.getValue());
            GameSetting.lang = languageButton.getValue() ? 2 : 0;
            GameSetting.save();
            ILang.isLoaded = false;
        }

        void onAiButtonClick()
        {
            aiButton.setValue(!aiButton.getValue());
            GameSetting.isCreateAi = aiButton.getValue();
            GameSetting.save();
        }

        void onCareerButtonClick()
        {
            careerButton.setValue(!careerButton.getValue());
            GameSetting.isCareer = careerButton.getValue();
            GameSetting.save();            
        }

        void onChannel100ButtonClick()
        {
            channel100Button.setValue(!channel100Button.getValue());
            GameSetting.isChannel100Activity = channel100Button.getValue();
            GameSetting.save();
        }

        void onMusicButtonClick()
        {
            musicButton.setValue(!musicButton.getValue());
            GameSetting.isMusicOpen = musicButton.getValue();
            GameSetting.save();
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

        void onBackButtonClick()
        {
            Application.LoadLevel("Menu");
        }

        void onResetCareerButtonClick()
        {
            IConfigBox.instance.show(ILang.get( "setting.reset career tap"), onResetCareerConfirm, null);
        }

        void onResetCareerConfirm()
        {
            Directory.Delete(GamePath.SDPATH + "world", true);
            //IUtils.createFolder(GamePath.worldFolder);
            IToast.instance.show("setting.reset successed", 100);
        }
    }
}
