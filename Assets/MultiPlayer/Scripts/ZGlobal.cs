using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ZGlobal
{
    /// <summary>
    /// 客户端类型
    /// </summary>
    public static ZClientMode ClientMode = ZClientMode.Visitor; 
    /// <summary>
    /// service版本
    /// </summary>
    public static ZServiceMode ServiceMode = ZServiceMode.LOCAL_HTTP;
    /// <summary>
    /// 游戏模式
    /// </summary>
    public static ZCurGameStatusMode CurGameStatusMode = ZCurGameStatusMode.WAITING_STATUS;
    /// <summary>
    /// Assetbudle 状体
    /// </summary>
    public static ZCurAssetBundleStatus CurABStatus = ZCurAssetBundleStatus.S0101;
}

public enum ZClientMode
{
    Curator,
    Visitor,
}

public enum ZCurAssetBundleStatus
{
    S0101 = 0,
    S0102 = 1,
    S0103 = 2,
    S0104,
    S0105,
    S0106,
}


public enum ZCurGameStatusMode
{
    /// <summary>
    /// 等待状态
    /// </summary>
    WAITING_STATUS,
    /// <summary>
    /// 小游戏状态
    /// </summary>
    MINI_GAME_STATUS,
    /// <summary>
    /// 展示状态
    /// </summary>
    MODELS_SHOW_STATUS,
}

public enum ZServiceMode
{
    /// <summary>
    /// 本地http Server
    /// </summary>
    LOCAL_HTTP = 0,
    /// <summary>
    /// 本地https server
    /// </summary>
    LOCAL_HTTPS = 1,
    /// <summary>
    /// 云端Server
    /// </summary>
    CLOUD ,
}

