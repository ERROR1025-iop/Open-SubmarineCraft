using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class Console : MonoBehaviour
    {

        static public Console instance;
        static public bool isArouse;

        static public Text consoleText;

        static Dictionary<int, string> stringArr;
        static int stack;
        static int showRow;
        static int showStack;

        static int longInfoRow;
        static int shortInfoRow;

        void Start()
        {
            instance = this;
            stringArr = new Dictionary<int, string>();
            consoleText = GetComponent<Text>();
            stack = 0;
            showRow = 4;
            showStack = 0;
            longInfoRow = 15;
            shortInfoRow = 4;
            consoleText.text = "";
        }

        public static void printLang(string content)
        {
            print(ILang.get(content));
        }

        public static void print(string content)
        {
            stringArr.Add(stack, content + "\n");
            Debug.Log(content);
            stack++;
            showRow = isArouse ? longInfoRow : shortInfoRow;
            showStack = 0;
            showArrText();
        }

        public static void arouse(bool show)
        {
            isArouse = show;

            if (show)
            {
                showRow = longInfoRow;
                showArrText();
            }
            else
            {
                showRow = shortInfoRow;
                showArrText();
            }
        }

        static void showArrText()
        {
            string t = "";
            foreach (KeyValuePair<int, string> value in stringArr)
            {
                if (value.Key > stack - showRow)
                    t = t + value.Value;
            }

            if (consoleText != null)
            {
                consoleText.text = t;
            }
        }

        void Update()
        {
            if (isArouse == false)
            {
                showStack++;
                if (showStack > 100)
                {
                    showRow = 0;
                    isArouse = false;
                }
                if (showStack > 300)
                {
                    showStack = 0;
                    showRow = 0;
                    showArrText();
                }
            }
        }
    }
}