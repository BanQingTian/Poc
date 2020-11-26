using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ZGlobal
{
    public static ZClientMode ClientMode = ZClientMode.Visiter;

    public static ZServiceMode ServiceMode = ZServiceMode.TEST;
}

public enum ZClientMode
{
    Master,
    Visiter,
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

