using Scraft.BlockSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class SVSelector
    {

        Transform objectTrans;
        Builder builder;

        IMultValueButton displayButton;
        Text valueText;
        Text nameText;
        int[] lastSettingValue;
        int[] lastDisplayValue;

        public SVSelector()
        {
            builder = Builder.instance;
            objectTrans = GameObject.Find("Canvas/SV selector").transform;
            displayButton = objectTrans.GetChild(1).GetComponent<IMultValueButton>();
            nameText = objectTrans.GetChild(2).GetComponent<Text>();
            objectTrans.GetChild(3).GetComponent<Button>().onClick.AddListener(onUpButtonClick);
            objectTrans.GetChild(4).GetComponent<Button>().onClick.AddListener(onDownButtonClick);
            valueText = objectTrans.GetChild(5).GetComponent<Text>();

            displayButton.setClickListener(onDisplayButtonClick);
            displayButton.init(4);
            displayButton.addValue("display0");
            displayButton.addValue("display1");
            displayButton.addValue("display2");
            displayButton.addValue("display3");

            initLastSettingValueArr();
        }

        void initLastSettingValueArr()
        {
            lastSettingValue = new int[BlocksManager.MAX_BLOCK_ID];
            lastDisplayValue = new int[BlocksManager.MAX_BLOCK_ID];
            for (int i = 0; i < BlocksManager.MAX_BLOCK_ID; i++)
            {
                lastSettingValue[i] = -95979;
                lastDisplayValue[i] = -95979;
            }
        }

        public void show(bool isShow)
        {
            objectTrans.gameObject.SetActive(isShow);
            if (isShow)
            {
                updataText();
            }
        }

        public void returnLastSettingValue(SolidBlock sblock)
        {

            if (sblock != null && sblock.isCanSettingValue() != -1)
            {
                if (sblock.equalBlock(BlocksManager.instance.numericalDisplay))
                {
                    return;
                }

                if (lastSettingValue[sblock.getId()] != -95979)
                {
                    sblock.setSettingValue(lastSettingValue[sblock.getId()]);
                }

                if (lastDisplayValue[sblock.getId()] != -95979)
                {
                    displayButton.selectValue(lastDisplayValue[sblock.getId()]);
                }
                else
                {
                    int display = sblock.isCanSettingValue();
                    displayButton.selectValue(display);
                }
                updataText();
            }
        }

        void updataText()
        {
            SolidBlock block = builder.selectBlock as SolidBlock;
            if (block != null)
            {
                nameText.text = ILang.get(block.getSettingValueName(), "menu");
                valueText.text = block.getCurrentSettingValue().ToString();
            }
        }

        void onDisplayButtonClick()
        {
            displayButton.moveToNextValue();
            lastDisplayValue[builder.selectBlock.getId()] = displayButton.getSelectIndex();
        }

        void onUpButtonClick()
        {
            SolidBlock block = builder.selectBlock as SolidBlock;
            if (block != null)
            {
                int value = 0;
                int display = displayButton.getSelectIndex();
                if (display == 0)
                {
                    value = block.getCurrentSettingValue() + 1;
                }
                else if (display == 1)
                {
                    value = block.getCurrentSettingValue() + 10;
                }
                else if (display == 2)
                {
                    value = block.getCurrentSettingValue() + 100;
                }
                else if (display == 3)
                {
                    value = block.getCurrentSettingValue() + 1000;
                }

                int[] rank = block.getSettingValueRank();
                int max = rank[1];
                if (value > max)
                {
                    block.setSettingValue(max);
                }
                else
                {
                    block.setSettingValue(value);
                }
                lastSettingValue[block.getId()] = block.getCurrentSettingValue();
                updataText();
            }
        }

        void onDownButtonClick()
        {
            SolidBlock block = builder.selectBlock as SolidBlock;
            if (block != null)
            {
                int value = 0;
                int display = displayButton.getSelectIndex();
                if (display == 0)
                {
                    value = block.getCurrentSettingValue() - 1;
                }
                else if (display == 1)
                {
                    value = block.getCurrentSettingValue() - 10;
                }
                else if (display == 2)
                {
                    value = block.getCurrentSettingValue() - 100;
                }
                else if (display == 3)
                {
                    value = block.getCurrentSettingValue() - 1000;
                }

                int[] rank = block.getSettingValueRank();
                int min = rank[0];
                if (value < min)
                {
                    block.setSettingValue(min);
                }
                else
                {
                    block.setSettingValue(value);
                }
                lastSettingValue[block.getId()] = block.getCurrentSettingValue();
                updataText();
            }
        }

    }
}
