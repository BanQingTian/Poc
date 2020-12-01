using System.Collections;
using System.Collections.Generic;
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
}