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
    /// 游戏模式
    /// </summary>
    public static ZCurGameStatusMode CurGameStatusMode = ZCurGameStatusMode.WAITING_STATUS;
    /// <summary>
    /// service版本
    /// </summary>
    public static ZServiceMode ServiceMode = ZServiceMode.TEST;
}

public enum ZClientMode
{
    Curator,
    Visitor,
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
    /// 测试模式
    /// </summary>
    TEST = 0,
    /// <summary>
    /// 本地Server
    /// </summary>
    LOCAL = 1,
    /// <summary>
    /// 云端Server
    /// </summary>
    CLOUD = 2    
}

