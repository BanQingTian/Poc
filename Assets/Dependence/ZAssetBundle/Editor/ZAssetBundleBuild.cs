using UnityEditor;
using UnityEngine;

public class ZAssetBundleBuild
{
    [MenuItem("Tools/BuildAB")]
    public static void BuildAB()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath,
                                        BuildAssetBundleOptions.ChunkBasedCompression,
                                        BuildTarget.Android);
    }
}
