using Scraft.BlockSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class PoolerSVSelector 
    {
        Transform objectTrans;
        PoolerInput poolerInput;

        IMultValueButton displayButton;
        Text valueText;
        Text nameText;

        public PoolerSVSelector(PoolerInput poolerInput)
        {
            this.poolerInput = poolerInput;
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

            show(false);
        }       

        public void show(bool isShow)
        {
            objectTrans.gameObject.SetActive(isShow);
            if (isShow)
            {
                updataText();
            }
        }        

        void updataText()
        {
            SolidBlock block = poolerInput.selectBlock as SolidBlock;
            if (block != null)
            {
                nameText.text = ILang.get(block.getSettingValueName(), "menu");
                valueText.text = block.getCurrentSettingValue().ToString();
            }
        }

        void onDisplayButtonClick()
        {
            SolidBlock selectBlock = poolerInput.selectBlock as SolidBlock;
            if (selectBlock != null)
            {
                displayButton.moveToNextValue();              
            }
        }

        void onUpButtonClick()
        {
            SolidBlock block = poolerInput.selectBlock as SolidBlock;
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

                updataText();
            }
        }

        void onDownButtonClick()
        {
            SolidBlock block = poolerInput.selectBlock as SolidBlock;
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

                updataText();
            }
        }
    }
}
