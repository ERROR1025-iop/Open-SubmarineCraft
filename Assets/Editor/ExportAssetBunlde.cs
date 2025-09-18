using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExportAssetBunlde 
{
	static string path = "D:/mnt/sdcard/SubmarineCraft/mods/" + "ExportAssetBunlde.unity3d";
	
    [MenuItem("Tools/Build AssetBundle From Selection - Track dependencies - Win64")]
    static void ExportResourceWin64()
    {        
        if (path.Length != 0)
        {
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows64);
            Selection.objects = selection;
        }
    }

    [MenuItem("Tools/Build AssetBundle From Selection - No dependency tracking - Win64")]
    static void ExportResourceNoTrackWin64()
    {       
        if (path.Length != 0)
        {
            BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path, BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows64);

        }
    }

    [MenuItem("Tools/Build AssetBundle From Selection - Track dependencies - Android")]
    static void ExportResourceAndroid()
    {       
        if(path.Length != 0)
        {
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies, BuildTarget.Android);
            Selection.objects = selection;
        }
    }

    [MenuItem("Tools/Build AssetBundle From Selection - No dependency tracking - Android")]
    static void ExportResourceNoTrackAndroid()
    {      
        if (path.Length != 0)
        {           
            BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path, BuildAssetBundleOptions.CollectDependencies, BuildTarget.Android);
           
        }
    }
}
