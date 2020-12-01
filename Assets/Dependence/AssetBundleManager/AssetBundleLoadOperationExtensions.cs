using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

using AssetBundles;
using NREAL.AR.Async;
using NREAL.AR.Utility;

public static class AssetBundleLoadOperationExtensions
{
    public static IEnumeratorAwaitExtensions.CoroutineAwaiter<AssetBundleLoadOperation> GetAwaiter(this AssetBundleLoadOperation instruction)
    {
        return IEnumeratorAwaitExtensions.GetAwaiterReturnSelf<AssetBundleLoadOperation>(instruction);
    }

    public static IEnumeratorAwaitExtensions.CoroutineAwaiter<AssetBundleLoadAssetOperation> GetAwaiter(this AssetBundleLoadAssetOperation instruction)
    {
        return IEnumeratorAwaitExtensions.GetAwaiterReturnSelf<AssetBundleLoadAssetOperation>(instruction);
    }

    public static IEnumeratorAwaitExtensions.CoroutineAwaiter<T> GetAwaiter<T>(this AssetBundleLoadAssetOperation<T> instruction) where T : UnityEngine.Object
    {
        var awaiter = new IEnumeratorAwaitExtensions.CoroutineAwaiter<T>();
        IEnumeratorAwaitExtensions.RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
            AssetBundleAssetRequest<T>(awaiter, instruction)));
        return awaiter;
    }

    private static IEnumerator AssetBundleAssetRequest<T>(IEnumeratorAwaitExtensions.CoroutineAwaiter<T> awaiter, AssetBundleLoadAssetOperation<T> instruction) where T : UnityEngine.Object
    {
        yield return instruction;
        awaiter.Complete(instruction.GetAsset(), null);
    }
}
