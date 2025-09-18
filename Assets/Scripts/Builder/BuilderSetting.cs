using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class BuilderSetting
    {

        RectTransform rectTrans;

        Builder builder;

        ISettingButton aiButton;
        ISettingButton careerButton;
        ISettingButton channel100Button;
        ISettingButton musicButton;

        public BuilderSetting()
        {
            builder = Builder.instance;

            rectTrans = GameObject.Find("Canvas/setting").GetComponent<RectTransform>();
            GameObject.Find("Canvas/setting/Resume game").GetComponent<Button>().onClick.AddListener(onResumeGameButtonClick);

            aiButton = rectTrans.GetChild(1).GetComponent<ISettingButton>();
            careerButton = rectTrans.GetChild(2).GetComponent<ISettingButton>();
            channel100Button = rectTrans.GetChild(3).GetComponent<ISettingButton>();
            musicButton = rectTrans.GetChild(4).GetComponent<ISettingButton>();

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

            show(false);
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
            IToast.instance.show("Return to the main menu and enter it again before it takes effect.", 100);
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
    }
}