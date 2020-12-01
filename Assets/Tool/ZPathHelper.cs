using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ZPathHelper
{
    public const string DirPath = "/storage/emulated/0/MultiDemo/Models/";
    public static string GetModelPath(string modelName)
    {
        if (!System.IO.Directory.Exists(DirPath))
        {
            System.IO.Directory.CreateDirectory(DirPath);
        }
        return string.Format("{0}{1}", DirPath, modelName);
    }

    public static string GetRootDirectory()
    {
        string path = Application.dataPath;
        return path;
    }


    public static string GetPersistentDataPath()
    {

#if UNITY_EDITOR
        int i = Application.dataPath.LastIndexOf('/');
        string path = Application.dataPath.Substring(0, i + 1) + "AssetBundles";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
#elif UNITY_ANDROID
        return Application.persistentDataPath;
#endif


        return "";
    }

    public static string GetABResPath()
    {
        string path;
#if UNITY_EDITOR
        int i = Application.dataPath.LastIndexOf('/');
        path = Directory.GetParent(Application.dataPath) + "/ABRes";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

#elif UNITY_ANDROID
        path = "/storage/emulated/0/ABRes";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
#endif
        return path;
    }
}