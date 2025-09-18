using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class AssemblerAddCustomCardBox
    {
        public static AssemblerAddCustomCardBox instance;

        RectTransform rectTrans;

        InputField inputField;

        public AssemblerAddCustomCardBox()
        {
            instance = this;

            rectTrans = GameObject.Find("Canvas/Add custom card box").GetComponent<RectTransform>();

            inputField = rectTrans.transform.GetChild(1).GetComponent<InputField>();
            rectTrans.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(onConfirmButtonClick);
            rectTrans.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(onResumeGameButtonClick);

            show(false);
        }

        void onConfirmButtonClick()
        {
            string name = inputField.text;
            if (name.Equals(""))
            {
                IToast.instance.show("Name is empty", 100);
                return;
            }

            string folder = GamePath.customFolder + name;
            IUtils.createFolder(folder);

            if (Directory.Exists(folder))
            {
                if (!CustomDpartsSelector.instance.registerNewCustomCard(name, false))
                {
                    IToast.instance.show("Has same name", 100);
                    return;
                }
                IToast.instance.show("Create successed!", 100);
                inputField.text = "";
                show(false);
            }
        }

        void onResumeGameButtonClick()
        {
            show(false);
        }

        public void show(bool show)
        {
            if (show)
            {
                rectTrans.anchoredPosition = new Vector2(-70, 0);
            }
            else
            {
                rectTrans.anchoredPosition = new Vector2(1000, 0);
            }
        }
    }
}