using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class AssemblerSetting
    {
        RectTransform rectTrans;
        Assembler assembler;
        ISettingButton musicButton;

        public AssemblerSetting()
        {
            assembler = Assembler.instance;

            rectTrans = GameObject.Find("Canvas/setting").GetComponent<RectTransform>();
            GameObject.Find("Canvas/setting/Resume game").GetComponent<Button>().onClick.AddListener(onResumeGameButtonClick);
            GameObject.Find("Canvas/setting/render").GetComponent<Button>().onClick.AddListener(onRenderButtonClick);

            musicButton = rectTrans.GetChild(1).GetComponent<ISettingButton>();

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

        void onRenderButtonClick()
        {
            RenderSetting.instance.show(true);
            show(false);
        }

        void onResumeGameButtonClick()
        {
            show(false);
        }

        void onMusicButtonClick()
        {
            musicButton.setValue(!musicButton.getValue());
            GameSetting.isMusicOpen = musicButton.getValue();
            GameSetting.save();            
        }
    }
}