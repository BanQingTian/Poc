using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AssetBundles;

using AssetBundleUtility = AssetBundles.Utility;

public class ResourceManager : Singleton<ResourceManager>
{
    private AssetBundleRepository m_Respository;

    private bool m_Inited;

    public IEnumerator Initialize()
    {
        if (m_Inited)
            yield break;
        m_Inited = true;
        m_Respository = new AssetBundleRepository();
        m_Respository.AddSearchPath(ZPathHelper.GetStreammingAssetsPath());
        m_Respository.AddSearchPath(ZPathHelper.GetPersistentDataPath());
       // m_Respository.AddSearchPath(ZPathHelper.GetABResPath());

        AssetBundleManager.ActiveVariants = new string[] { "boys", "girls" };
        AssetBundleManager.BaseDownloadingURL = "http://127.0.0.1:8080/Windows/";

        AssetBundleManager.overrideAssetBundleDownloadOperation += (string root, string assetBundleName) =>
        {
            return new AssetBundleOpenFromRepositoryOperation(root, assetBundleName, m_Respository);
        };

        yield return AssetBundleManager.Initialize();
    }

    public static AssetBundleLoadAssetOperation<T> LoadAssetAsync<T>(string rootName, string assetBundleName, string assetName) where T : UnityEngine.Object
    {
        return AssetBundleManager.LoadAssetImmediateAsync<T>(rootName, assetBundleName, assetName);
    }

    public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
    {
        return AssetBundleManager.LoadAssetAsync(AssetBundleUtility.AssetBundlesDefaultRootName, assetBundleName, assetName, type);
    }

    public static AssetBundleLoadAssetOperation<T> LoadAssetAsync<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
    {
        return AssetBundleManager.LoadAssetAsync<T>(AssetBundleUtility.AssetBundlesDefaultRootName, assetBundleName, assetName);
    }

    public static void LoadAssetAsync<T>(string rootName, string assetBundleName, string assetName, Action<T> onFinish) where T : UnityEngine.Object
    {
        ResourceManager.Instance.StartCoroutine(CoLoadAssetAsync<T>(rootName, assetBundleName, assetName, onFinish));
    }

    public static void LoadAssetAsync<T>(string assetBundleName, string assetName, Action<T> onFinish) where T : UnityEngine.Object
    {
        ResourceManager.Instance.StartCoroutine(CoLoadAssetAsync<T>(AssetBundleUtility.AssetBundlesDefaultRootName, assetBundleName, assetName, onFinish));
    }

    private static IEnumerator CoLoadAssetAsync<T>(string rootName, string assetBundleName, string assetName, Action<T> onFinish) where T : UnityEngine.Object
    {
        AssetBundleLoadAssetOperation operation = null;
        if (rootName.Equals(AssetBundleUtility.AssetBundlesDefaultRootName))
        {
            operation = AssetBundleManager.LoadAssetAsync(rootName, assetBundleName, assetName, typeof(T));
        }
        else
        {
            operation = AssetBundleManager.LoadAssetImmediateAsync(rootName, assetBundleName, assetName, typeof(T));
        }
        yield return operation;

        onFinish.Invoke(operation.GetAsset<T>());
    }

    public static AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
    {
        return AssetBundleManager.LoadLevelAsync(AssetBundleUtility.AssetBundlesDefaultRootName, assetBundleName, levelName, isAdditive);
    }

    public static void UnLoadAssetBundle(string abName)
    {
        AssetBundleManager.UnloadAssetBundle(string.Format("{0}/{1}", ZConstant.DefaultDir, abName));
    }

    public static void LoadLevelAsync(string assetBundleName, string assetName, bool isAdditive, Action onFinish)
    {
        ResourceManager.Instance.StartCoroutine(CoLoadLevelAsync(assetBundleName, assetName, isAdditive, onFinish));
    }

    private static IEnumerator CoLoadLevelAsync(string assetBundleName, string assetName, bool isAdditive, Action onFinish)
    {
        var operation = AssetBundleManager.LoadLevelAsync(AssetBundleUtility.AssetBundlesDefaultRootName, assetBundleName, assetName, isAdditive);
        yield return operation;

        onFinish.Invoke();
    }
}
