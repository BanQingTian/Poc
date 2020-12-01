using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AssetBundles
{
    public partial class AssetBundleManager : MonoBehaviour
    {
        static public AssetBundleLoadAssetOperation LoadAssetImmediateAsync(string root, string assetBundleName, string assetName, System.Type type)
        {
            Log(LogType.Info, "Loading " + assetName + " from " + assetBundleName + " bundle");

            AssetBundleLoadAssetOperation operation = null;
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                if (assetPaths.Length == 0)
                {
                    Log(LogType.Error, "There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                }

                UnityEngine.Object target = null;
                if (assetPaths.Length > 0)
                {
                    target = AssetDatabase.LoadAssetAtPath(assetPaths[0], type);
                }

                operation = new AssetBundleLoadAssetOperationSimulation(target);
            }
            else
#endif
            {
                LoadAssetBundle(root, assetBundleName);
                operation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);

                m_InProgressOperations.Add(operation);
            }

            return operation;
        }

        static public AssetBundleLoadAssetOperation<T> LoadAssetImmediateAsync<T>(string root, string assetBundleName, string assetName) where T : UnityEngine.Object
        {
            Log(LogType.Info, "Loading " + assetName + " from " + assetBundleName + " bundle");

            AssetBundleLoadAssetOperation<T> operation = null;
            System.Type type = typeof(T);

#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                if (assetPaths.Length == 0)
                {
                    Log(LogType.Error, "There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                }

                UnityEngine.Object target = null;
                if (assetPaths.Length > 0)
                {
                    target = AssetDatabase.LoadAssetAtPath(assetPaths[0], type);
                }

                operation = new AssetBundleLoadAssetOperationSimulation<T>(target);
            }
            else
#endif
            {
                LoadAssetBundle(root, assetBundleName);
                operation = new AssetBundleLoadAssetOperationFull<T>(assetBundleName, assetName);

                m_InProgressOperations.Add(operation);
            }

            return operation;
        }
    }
}