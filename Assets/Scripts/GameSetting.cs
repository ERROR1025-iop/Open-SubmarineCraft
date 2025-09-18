using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

namespace Scraft
{
    public class GameSetting
    {

        public static bool isAndroid;
        public static bool isSteam;

        public static string setting_path = Application.persistentDataPath + "/config.data";
        static bool isLoaded;

        public static string version;
        public static int lang = 2;

        public static bool isCreateAi = false;
        public static bool isCreateDeep = true;
        public static bool isChannel100Activity = true;
        public static bool isMusicOpen = true;
        public static bool isAssemblerShowText = false;

        public static int renderMode = 0;
        public static bool renderUnderwaterEffect = true;
        public static bool renderLightbeam = true;
        public static bool isCareer = true;
        public static string viewAdTime;
        public static bool isViewTutorial = false;
        public static float diamonds = -1;


        static GameSetting()
        {
            isAndroid = false;
            isSteam = false;
            version = "version 3.6.1";
        }

        public static void init()
        {
            if (!File.Exists(setting_path))
            {
                save();
            }
            else
            {
                load();
            }
        }

        public static void save()
        {

            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();

            IUtils.keyValue2Writer(writer, "lang", lang);

            IUtils.keyValue2Writer(writer, "isCreateAi", isCreateAi);
            IUtils.keyValue2Writer(writer, "isCareer", isCareer);
            IUtils.keyValue2Writer(writer, "isCreateDeep", isCreateDeep);
            IUtils.keyValue2Writer(writer, "isChannel100Activity", isChannel100Activity);
            IUtils.keyValue2Writer(writer, "isMusicOpen", isMusicOpen);
            IUtils.keyValue2Writer(writer, "isAssemblerShowText", isAssemblerShowText);
            IUtils.keyValue2Writer(writer, "renderMode", renderMode);
            IUtils.keyValue2Writer(writer, "renderUnderwaterEffect", renderUnderwaterEffect);
            IUtils.keyValue2Writer(writer, "renderLightbeam", renderLightbeam);
            IUtils.keyValue2Writer(writer, "viewAdTime", viewAdTime);
            IUtils.keyValue2Writer(writer, "isViewTutorial", isViewTutorial);
            IUtils.keyValue2Writer(writer, "diamonds", diamonds);

            writer.WriteObjectEnd();
            IUtils.write2txt(setting_path, writer.ToString());
        }

        public static void load()
        {
            if (isLoaded)
            {
                return;
            }
            isLoaded = true;

            if (File.Exists(setting_path))
            {
                JsonData jsonData = JsonMapper.ToObject(IUtils.readFromTxt(setting_path));
                lang = IUtils.getJsonValue2Int(jsonData, "lang", 2);

                isCreateAi = IUtils.getJsonValue2Bool(jsonData, "isCreateAi", false);
                isCareer = IUtils.getJsonValue2Bool(jsonData, "isCareer", true);
                isCreateDeep = IUtils.getJsonValue2Bool(jsonData, "isCreateDeep", true);
                isChannel100Activity = IUtils.getJsonValue2Bool(jsonData, "isChannel100Activity", true);
                isMusicOpen = IUtils.getJsonValue2Bool(jsonData, "isMusicOpen", true);
                isAssemblerShowText = IUtils.getJsonValue2Bool(jsonData, "isAssemblerShowText", false);
                renderMode = IUtils.getJsonValue2Int(jsonData, "renderMode", 1);
                renderUnderwaterEffect = IUtils.getJsonValue2Bool(jsonData, "renderUnderwaterEffect", true);
                renderLightbeam = IUtils.getJsonValue2Bool(jsonData, "renderLightbeam", true);
                viewAdTime = IUtils.getJsonValue2String(jsonData, "viewAdTime");
                isViewTutorial = IUtils.getJsonValue2Bool(jsonData, "isViewTutorial", false);
                diamonds = IUtils.getJsonValue2Float(jsonData, "diamonds", -1f);
            }
        }
    }
}