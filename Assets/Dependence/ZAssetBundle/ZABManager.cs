using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ZABManager 
{
    private static ZABManager s_instance;
    public static ZABManager instance
    {
        get
        {
            if (null == s_instance)
                s_instance = new ZABManager();
            return s_instance;
        }
    }

    private AssetBundleManifest m_Manifest;
    private Dictionary<string, AssetBundle> m_abDic = new Dictionary<string, AssetBundle>();


    // 初始化加载Manifest，方便后续依赖查找
    public void Init()
    {
        string assetAbPath = Path.Combine(Application.streamingAssetsPath, "StreamingAssets");
        AssetBundle streamingAssetsAb = AssetBundle.LoadFromFile(assetAbPath);
        m_Manifest = streamingAssetsAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    }

    // 加载AssetBundle
    public AssetBundle LoadAssetBundle(string abName)
    {
        AssetBundle ab = null;
        if (!m_abDic.ContainsKey(abName))
        {
            string abResPath = Path.Combine(Application.streamingAssetsPath, abName);
            ab = AssetBundle.LoadFromFile(abResPath);
            m_abDic[abName] = ab;
        }
        else
        {
            ab = m_abDic[abName];
        }

        //加载依赖
        string[] dependences = m_Manifest.GetAllDependencies(abName);
        int dependenceLen = dependences.Length;
        if (dependenceLen > 0)
        {
            for (int i = 0; i < dependenceLen; i++)
            {
                string dependenceAbName = dependences[i];
                if (!m_abDic.ContainsKey(dependenceAbName))
                {
                    AssetBundle dependenceAb = LoadAssetBundle(dependenceAbName);
                    m_abDic[dependenceAbName] = dependenceAb;
                }
            }
        }

        return ab;
    }

    // 从AssetBundle中加载Asset
    public T LoadAsset<T>(string abName, string assetName) where T : Object
    {
        AssetBundle ab = LoadAssetBundle(abName);
        T t = ab.LoadAsset<T>(assetName);
        return t;
    }

    public void LoadAssetAsync<T>(string abName, string assetName, System.Action<T> onFinish) where T: Object
    {
        ZCoroutiner.StartCoroutine(CoLoadAssetAsync<T>(abName, assetName, onFinish));
    }

    public IEnumerator CoLoadAssetAsync<T> (string abName, string assetName, System. Action<T> onFinish) where T : Object
    {
        AssetBundle ab = LoadAssetBundle(abName);
        AssetBundleRequest abr = ab.LoadAssetAsync<T>(assetName);
        yield return abr.isDone;

        onFinish.Invoke(abr.asset as T);
    }
}
