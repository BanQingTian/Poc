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

    private int playerCount;

    public int PlayerCount
    {
        get
        {
            return playerCount;
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
}
