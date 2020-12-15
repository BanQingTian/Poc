using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRToolkit;

/// <summary>
/// 本地数据类
/// </summary>
public class PlayerMe
{
    private Dictionary<string, PlayerNetObj> PlayerDict = new Dictionary<string, PlayerNetObj>();
    private Dictionary<string, GameObject> AssetBundleGODict = new Dictionary<string, GameObject>();

    private int playerCount;

    public int PlayerCount
    {
        get
        {
            return playerCount;
        }
    }

    public PlayerNetObj GetOwnerPlayerNetObj
    {
        get
        {
            foreach (var item in PlayerDict)
            {
                if (item.Value.isOwner)
                    return item.Value;
            }
            Debug.LogError("[CZLOG] GetOwnerPlayerNetObj Failed !!!");
            return null;
        }
    }

    public PlayerNetObj GetPlayerNetObj(string playerid )
    {
        if (PlayerDict.ContainsKey(playerid))
        {
            return PlayerDict[playerid];
        }
        else
        {
            Debug.LogError("ZLOG  ----  Dont contain playerid");
            return null;
        }
    }

    public void AddPlayer(string playerid, PlayerNetObj pno)
    {
        if (!PlayerDict.ContainsKey(playerid))
        {
            PlayerDict.Add(playerid, pno);
            playerCount++;
        }
    }

    public void RemovePlayer(string playerid)
    {
        PlayerNetObj obj = null;
        if (PlayerDict.TryGetValue(playerid, out obj))
        {
            GameObject.Destroy(obj.gameObject);
            PlayerDict.Remove(playerid);
            playerCount--;
        }
    }

    public void ClearPlayerData()
    {
        foreach (var item in PlayerDict.Values)
        {
            GameObject.Destroy(item.gameObject);
        }

        PlayerDict.Clear();
        playerCount = 0;
    }


    public GameObject GetAssetBundleGO(string abName)
    {
        GameObject go;
        if (AssetBundleGODict.TryGetValue(abName,out go))
        {
            go.SetActive(true);
            return go;
        }
        Debug.Log("{CZLOG] GetAssetBundleGameObject Failed !!!");
        return null;
    }

    public void AddAssetBundleGO(string abName, GameObject g)
    {
        GameObject go;
        if (!AssetBundleGODict.TryGetValue(abName, out go))
        {
            AssetBundleGODict.Add(abName, go);
        }
        else
        {
            Debug.Log("{CZLOG] GetAssetBundleGameObject Failed !!!");
        }
    }

    public void RemoveAssetBundleGO(string abName, bool delete = false)
    {
        GameObject go;
        if (AssetBundleGODict.TryGetValue(abName, out go))
        {
            if (delete)
            {
                GameObject.Destroy(go);
                AssetBundleGODict.Remove(abName);
            }
            else
            {
                go.SetActive(false);
            }
        }
        else
        {
            Debug.Log("{CZLOG] RemoveAssetBundleGO Failed !!!");
        }
    }

}
