using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class GamePath
    {
        public static string MID = "7B94042G464FA171B0C31BC6B2C04911";
        public static string SDPATH = "";
        public static string logFolder;
        public static string shipsFolder;
        public static string dpartFolder;
        public static string cacheFolder;
        public static string thumbnailFolder;
        public static string builderThumbnailFolder;
        public static string assemblerThumbnailFolder;
        public static string groupsFolder;
        public static string groupsThumbnailFolder;
        public static string customFolder;
        public static string customThumbnailFolder;
        public static string worldFolder;
        public static string worldThumbnailFolder;
        public static string modFolder;
        public static string exportFolder;

        static bool isInit = false;

        static public void init(string Mid)
        {
            if (!isInit)
            {
                string writablePath = Application.persistentDataPath;
                Debug.Log("Persistent Data Path: " + writablePath);
                string sdpath = System.IO.Path.Combine(writablePath, "SubmarineCraft") + "/";

                SDPATH = sdpath;
                MID = Mid;

                logFolder = registerFolder(SDPATH + "logs/");
                shipsFolder = registerFolder(SDPATH + "ships/");
                dpartFolder = registerFolder(SDPATH + "ships3D/");
                cacheFolder = registerFolder(SDPATH + "cache/");
                thumbnailFolder = registerFolder(SDPATH + "thumbnail/");
                builderThumbnailFolder = registerFolder(thumbnailFolder + "builder/");
                assemblerThumbnailFolder = registerFolder(thumbnailFolder + "assembler/");
                groupsFolder = registerFolder(SDPATH + "groups/");
                groupsThumbnailFolder = registerFolder(thumbnailFolder + "groups/");
                customFolder = registerFolder(SDPATH + "custom/");
                customThumbnailFolder = registerFolder(thumbnailFolder + "custom/");
                worldFolder = registerFolder(SDPATH + "world/");
                worldThumbnailFolder = registerFolder(thumbnailFolder + "world/");
                modFolder = registerFolder(SDPATH + "mods/");
                exportFolder = registerFolder(SDPATH + "exps/");

                isInit = true;
            }
        }

        static string registerFolder(string path)
        {
            IUtils.createFolder(path);
            return path;
        }

    }
}