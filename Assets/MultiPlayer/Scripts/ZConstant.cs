﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ZConstant
{
    /// <summary>
    /// 创建房间后将在xs后关闭房间加入权限
    /// </summary>
    public const int WaitToTurnOffRoomPerissionTime = 20;
    /// <summary>
    /// 全部扫秒完maker，将在3s后开启minigame
    /// </summary>
    public const float AllScanReadyWaitTime = 5;

    public const string Event__Capture__ = "event_capture";
    public const string Event__MiniGame__ = "event_minigame";
    public const string Event__ModelShow__ = "event_modelshow";
    public const string Event__Rotate__ = "event_rotate";

    public const string DefaultDir = "lgu";

    public static string DefaultScheme = "ws";
    public static string ReplaceScheme = "http";


    #region Follow_UI_Name

    public const string Dialog = "Dialog";
    public const string UICanvas = "UI_Canvas";

    #endregion

    #region ui_name - bundle

    public const string First = "1st";
    public const string FirstPress = "1st_press";

    public const string Second = "2nd";
    public const string SecondPress = "2nd_press";

    public const string Third = "3rd";
    public const string ThirdPress = "3rd_press";

    public const string Back = "back";

    public const string Bg1 = "bg1";
    public const string Bg2 = "bg2";

    public const string Logo = "logo";

    public const string Minigame = "minigame";
    public const string MinigamePress = "minigame_press";

    public const string Model = "model";
    public const string ModelPress = "model_press";

    public const string Photo = "photo";
    public const string PhotoPress = "photo_press";

    public const string Rotate = "rotate";
    public const string RotatePress = "rotate_press";

    public const string TouchScreen = "touchscreen";

    #endregion
}
