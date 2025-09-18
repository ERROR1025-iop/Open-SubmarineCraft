using LitJson;
using Scraft.DpartSpace;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class AssemblerAddCustomBox
    {
        public static AssemblerAddCustomBox instance;

        Assembler assembler;
        RectTransform rectTrans;

        InputField inputField;
        Dropdown dropdown;

        List<Dropdown.OptionData> options;

        public AssemblerAddCustomBox()
        {
            instance = this;
            assembler = Assembler.instance;

            rectTrans = GameObject.Find("Canvas/Add custom box").GetComponent<RectTransform>();

            inputField = rectTrans.transform.GetChild(1).GetComponent<InputField>();
            rectTrans.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(onConfirmButtonClick);
            rectTrans.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(onResumeGameButtonClick);
            dropdown = rectTrans.transform.GetChild(5).GetComponent<Dropdown>();

            options = new List<Dropdown.OptionData>();

            show(false);
        }

        public void addDropDownOption(string name)
        {
            Dropdown.OptionData optionData = new Dropdown.OptionData(name);
            options.Add(optionData);
        }

        public void refreshDropDown()
        {
            dropdown.options = options;
        }

        void onConfirmButtonClick()
        {
            string name = inputField.text;
            CustomCard customCard = CustomDpartsSelector.instance.getCardById(dropdown.value);
            string group = customCard.getCardName();
            if (name.Equals(""))
            {
                IToast.instance.show("Name is empty", 100);
                return;
            }

            IToast.instance.show("Saving");

            GameObject[] gos = IRT.Selection.gameObjects;

            JsonWriter writerDparts = new JsonWriter();
            Dpart dpart;
            int dpartsCount = 0;

            writerDparts.WriteObjectStart();
            writerDparts.WritePropertyName("dparts");
            writerDparts.WriteObjectStart();
            int count = gos.Length;
            for (int i = 0; i < count; i++)
            {
                if (gos[i] != null)
                {
                    DpartParent dpartParent = gos[i].GetComponent<DpartParent>();
                    if (dpartParent != null)
                    {
                        dpart = dpartParent.getDpart();
                        if (gos[i].activeSelf)
                        {
                            writerDparts.WritePropertyName("" + i);
                            writerDparts.WriteObjectStart();
                            writerDparts = dpart.onBuilderModeSave(writerDparts);
                            writerDparts.WriteObjectEnd();
                            dpartsCount++;
                        }
                    }
                }
            }

            writerDparts.WriteObjectEnd();
            IUtils.keyValue2Writer(writerDparts, "count", dpartsCount);
            writerDparts.WriteObjectEnd();
            string shipData = writerDparts.ToString();
            string pathFullName = GamePath.customFolder + group + "/" + name + ".ass";
            IUtils.write2txt(pathFullName, shipData);
            AssemblerUtils.createDpartThumbnailImage(Assembler.dpartsEngine, pathFullName, GamePath.customThumbnailFolder + group + "/", name);
            customCard.refreshDrawer();

            IToast.instance.show("Saved", 100);
            show(false);
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