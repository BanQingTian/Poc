using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ZGlobal
{
    public static ZClientMode ClientMode = ZClientMode.Visitor;

    public static ZServiceMode ServiceMode = ZServiceMode.TEST;
}

public enum ZClientMode
{
    Curator,
    Visitor,
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

