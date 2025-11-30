using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

namespace Scraft
{
    public class ILang : MonoBehaviour
    {

        static string langType = "zh_CHS";

        static Dictionary<string, JsonData> jsonDataArr;
        public static string[] langList;
        public static bool isLoaded;
        static string folder;

        static public void loadLangData()
        {
            GameSetting.load();

            langList = new string[3];
            langList[0] = "ENG";
            langList[1] = "de";
            langList[2] = "zh_CHS";

            langType = langList[GameSetting.lang];
            jsonDataArr = new Dictionary<string, JsonData>();
            folder = "Lang/" + langType + "/";
            loadLangFile(folder, "area.txt");
            loadLangFile(folder, "card.txt");
            loadLangFile(folder, "main.txt");
            loadLangFile(folder, "menu.txt");
            loadLangFile(folder, "information.txt");
            loadLangFile(folder, "tap.txt");
            loadLangFile(folder, "dpart.txt");
            loadLangFile(folder, "dpart-information.txt");
            loadLangFile(folder, "input.txt");
            loadLangFile(folder, "research.txt");
            loadLangFile(folder, "station.txt");
            loadLangFile(folder, "tutorial.txt");
            loadLangFile(folder, "tutorial_pc.txt");
            isLoaded = true;
        }

        static public void loadLangFile(string folder, string fileName)
        {
            try{
                string type = Path.GetFileNameWithoutExtension(fileName);
                string filePath = folder + type;
                TextAsset textAsset = Resources.Load(filePath) as TextAsset;
                JsonData data = JsonMapper.ToObject(textAsset.text);
                jsonDataArr.Add(type, data);
            }
            catch(System.Exception e)
            {
                Debug.LogError(folder + "/" + fileName + ":" + e.ToString());
            }
        }

        static public void loadLangFileFromExternal(string path, string type)
        {
            if (!jsonDataArr.ContainsKey(type))
            {
                JsonData data = JsonMapper.ToObject(IUtils.readFromTxt(path));
                jsonDataArr.Add(type, data);
            }
        }

        static public string getSelectedLangName()
        {
            return getLangName(GameSetting.lang);
        }

        static public string getLangName(int index)
        {
            return langList[index];
        }

        static public string get(string keyword)
        {
            return get(keyword, "menu");
        }

        static public string modGet(string keyword, string modName)
        {
            string type = modName + getSelectedLangName();
            return get(keyword, type);
        }

        static public string get(string keyword, string type)
        {
            if (isLoaded == false)
            {
                loadLangData();
            }
            string name = keyword;

            name = IUtils.getJsonValue2String(jsonDataArr[type], keyword);

            if (name != null)
            {
                return name;
            }
            else
            {
                return keyword;
            }
        }
    }
}
