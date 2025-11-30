using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.IO;
using LitJson;
using Scraft.BlockSpace;
using Scraft.DpartSpace;
using Scraft;

namespace Scraft {
    public class ModLoader : MonoBehaviour
    {
        static public bool isLoaded;

        static public Dictionary<string, Assembly> assemblytDictionary;
        static public Dictionary<string, AssetBundle> assetBundleDictionary;
        static public Dictionary<string, ModInfo> modInfos;
        static public Dictionary<string, ModConfig> modConfigs;

        void Start()
        {
            if(!isLoaded)
            {
                load();
                isLoaded = true;
            }            
        }

        static public void reload()
        {
            BlocksManager blocksManager = new BlocksManager();
            if (World.instance != null)
            {
                World.instance.blocksManager = blocksManager;
            }            
            load();
        }

        static public void load()
        {
            BlocksManager.get();
            DpartsManager.get();
            assemblytDictionary = new Dictionary<string, Assembly>();
            assetBundleDictionary = new Dictionary<string, AssetBundle>();
            modInfos = new Dictionary<string, ModInfo>();
            modConfigs = new Dictionary<string, ModConfig>();

            if (Directory.Exists(GamePath.modFolder))
            {
                DirectoryInfo direction = new DirectoryInfo(GamePath.modFolder);
                DirectoryInfo[] directories = direction.GetDirectories("*", SearchOption.TopDirectoryOnly);
                if (directories.Length > 0)
                {
                    for (int i = 0; i < directories.Length; i++)
                    {
                        loadMod(directories[i].Name);
                    }
                }
            }
        }

        static public void loadMod(string name)
        {
            bool isActivited = loadConfigFile(name);           
            loadLangFiles(name);
            loadAssestes(name);
            loadDll(name);
            
        }

        static public bool loadConfigFile(string name)
        {
            string path = string.Format("{0}{1}/config.txt", GamePath.modFolder, name);
            ModConfig modConfig = new ModConfig(name);
            if (File.Exists(path))
            {
                JsonData jsonData = JsonMapper.ToObject(IUtils.readFromTxt(path));
                modConfig.loadFromJsonData(jsonData);
            }
            modConfigs.Add(name, modConfig);
            return modConfig.isActivited;
        }

        static public void loadLangFiles(string name)
        {
            loadLangFile(name, "ENG");
            loadLangFile(name, "zh_CHS");
        }

        static public void loadLangFile(string name, string lang)
        {
            string langPath = string.Format("{0}{1}/lang/{2}.txt", GamePath.modFolder, name, lang);
            if (File.Exists(langPath))
            {
                ILang.loadLangFileFromExternal(langPath, name + lang);
            }
        }


        static public void loadDll(string name)
        {
            string path1 = string.Format("{0}{1}/core.dll", GamePath.modFolder, name);
            string path2 = string.Format("{0}{1}/{2}.dll", GamePath.modFolder, name, name);
            Assembly am = null;
            if (File.Exists(path1))
            {
                am = Assembly.Load(IUtils.readFromDll(path1));                
            }
            else if(File.Exists(path2))
            {
                am = Assembly.Load(IUtils.readFromDll(path2));
            }
            if(am != null)
            {
                assemblytDictionary.Add(name, am);
                modInfos.Add(name, doReflectionMethod(am, name, name + ".Register", "initialize", null) as ModInfo);
                if (modConfigs[name].isActivited)
                {                    
                    doReflectionMethod(am, name, name + ".Register", "registerBlocks", null);
                    doReflectionMethod(am, name, name + ".Register", "registerDparts", null);
                }
            }            
        }        

        static public void loadAssestes(string name)
        {
            string path = string.Format("{0}{1}/{2}.unity3d", GamePath.modFolder, name, name);
            AssetBundle ab = null;
            if (File.Exists(path))
            {

                ab = AssetBundle.LoadFromFile(path);
            }
            assetBundleDictionary.Add(name, ab);
        }

        static public object doReflectionMethod(Assembly am, string assetName, string className, string methodName, object[] parameters)
        {                      
            Type type = am.GetType(className);
            MethodInfo method = type.GetMethod(methodName);
            return method.Invoke(type, parameters);
        }        
    }

    public class ModConfig
    {
        public string name;
        public bool isActivited;
        public ulong pid;

        public ModConfig(string name)
        {
            this.name = name;
            isActivited = true;
            pid = 0;
        }

        public ModConfig(string name, JsonData jsonData)
        {            
            this.name = name;
            loadFromJsonData(jsonData);
        }

        public void loadFromJsonData(JsonData jsonData)
        {
            string path = string.Format("{0}{1}/config.txt", GamePath.modFolder, name);
            isActivited = IUtils.getJsonValue2Bool(jsonData, "isActivited", true);
            pid = ulong.Parse(IUtils.getJsonValue2String(jsonData, "pid", "0"));
        }

        public void saveFile()
        {
            string path = string.Format("{0}{1}/config.txt", GamePath.modFolder, name);
            JsonWriter writer = new JsonWriter();
            writer.WriteObjectStart();
            IUtils.keyValue2Writer(writer, "name", name);
            IUtils.keyValue2Writer(writer, "isActivited", isActivited);
            IUtils.keyValue2Writer(writer, "pid", pid.ToString());
            writer.WriteObjectEnd();
            IUtils.write2txt(path, writer.ToString());
        }

        public string GetFolderPath()
        {
            return string.Format("{0}{1}/", GamePath.modFolder, name);
        }
        public string GetZipPath()
        {
            return string.Format("{0}{1}.zip", GamePath.cacheFolder, name);
        }
    }
}
