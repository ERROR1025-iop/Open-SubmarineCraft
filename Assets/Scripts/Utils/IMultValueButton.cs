using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scraft
{
    public class IMultValueButton : MonoBehaviour
    {

        public string[] values;
        public Text buttonText;
        public Button button;

        int stack;
        int handle;
        int valueCount;

        void Start()
        {

        }

        public void init(int valueCount)
        {
            this.valueCount = valueCount;
            if(values == null || values.Length == 0)
            {
                values = new string[valueCount];
            }
            handle = 0;
            stack = 0;
        }

        public void setValueList(string[] values)
        {
            this.values = values;
        }

        public void addValue(string value)
        {
            values[stack] = value;
            stack++;
        }

        public void selectValue(int index)
        {
            handle = index;
            buttonText.text = ILang.get(values[index], "menu");
        }

        public int getSelectIndex()
        {
            return handle;
        }

        public string getValue()
        {
            return values[handle];
        }

        public void moveToNextValue()
        {
            handle++;
            if (handle >= valueCount)
            {
                handle = 0;
            }
            selectValue(handle);
        }

        public void setClickListener(UnityAction call)
        {
            button.onClick.AddListener(call);
        }
    }
}