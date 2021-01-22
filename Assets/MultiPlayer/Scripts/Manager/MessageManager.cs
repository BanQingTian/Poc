using NetWorkToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Colyseus;
using static NetWorkToolkit.ColyseusClient;
using System;
using System.Linq;

public class S2CFuncName
{
    public static string Fire = "Fire";
    public static string Rotate = "Rotate";
    public static string ImprisonMonster = "Imprison";
    public static string CreateMonster = "CreateMonster";
    public static string DamageMonster = "DamageMonster";
    public static string PlayBattle = "PlayBattle";
    public static string ChangeWeapon = "ChangeWeapon";
    public static string GameOver = "GameOver";
    public static string AddScore = "AddScore";

    public static string Countdown = "Countdown";
    public static string BeginScanMaker = "BeginScanMaker";
    public static string ScanMaker = "ScanMaker";
    public static string PlayMiniGame = "PlayMiniGame";
    public static string PlayShowModels = "PlayShowModels";
    public static string PlayNextAnim = "PlayNextAnim";
    public static string ChargeOnce = "ChargeOnce";

    public static string Test = "test";
}

public class MessageManager
{

    private static MessageManager _instance;
    public static MessageManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MessageManager();
            return _instance;
        }
    }

    private bool m_Initialize = false;
    private ColyseusClient client;

    public Action ConnectSuccessEvent;
    public Action ConnectFaildEvent;

    public void InitializeMessage(Action connectSuccessEvent = null)
    {
        if (m_Initialize) return;

        client = ColyseusClient.instance;

        client.AddListener(MsgId.Commond, OnCommandResp);
        client.AddListener(MsgId.DisConnect, OnDisConnectResp);
        client.AddListener(MsgId.CreateANetObject, OnCreateANetObjectResp);
        client.AddListener(MsgId.DestroyANetObject, OnDestroyANetObjectResp);
        client.AddListener(MsgId.StartGame, OnStartGameResp);
        client.AddListener(MsgId.UpdateRoomInfo, OnUpdateRoomInfoResp);


        ConnectSuccessEvent += connectSuccessEvent;

        m_Initialize = true;
    }


    #region SendMessage    

    public void SendConnectServerMsg(string host, string route)
    {
        //ConnectFaildEvent += GameManager.Instance.Reconnect;
        //GlobalTip.AkunamoTataPlus("Server connection...");
        //serverIP = serverIP.Replace(" ", "");
        client.ConnectToServer(host, route, OnConnectResp);
        //client.ConnectToServer("192.168.68.55", route, OnConnectResp);

    }

    public void SendCurGameCountdownTime(int ct)
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.Countdown,
            param = ct.ToString(),
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendStartScanMarker()
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.BeginScanMaker,
            param = "shelter",
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendScanMakerFinish(string playeid)
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.ScanMaker,
            param = playeid,
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendPlayMiniGame()
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.PlayMiniGame,
            param = ((int)ZCurAssetBundleStatus.S0102).ToString(),
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendPlayShowModels(ZCurAssetBundleStatus abs)
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.PlayShowModels,
            param = ((int)abs).ToString(),
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendRotate()
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.Rotate,
            param = "shelter"
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendLeaveRoomMsg()
    {
        client.LeaveRoom(onLeaveRoom);
    }

    public void SendCreateRoomMsg(string roomName = "NrealRoom", string roomPassword = "nreal")
    {
        client.CreateRoom(roomName, roomPassword, OnCreateARoomResp);
    }

    public void SendDestoryPlayerObjMsg()
    {
        client.SendMsg(MsgId.DestroyANetObject, Target.All, null);
    }

    public void SendRefreshRoomList()
    {
        //client.SendMsg(MsgId.GetRoomList, Target.All, null);
        //client.SendMsg(MsgId.UpdateRoomInfo, Target.All, null);
        client.GetAvailableRooms(OnUpdateRoomInfoResp);
    }

    public void SendJoinRoomByIdMsg(string roomid, string password = "nreal")
    {
        // client.GetAvailableRooms(onWhetherCanJoinRoom);

        client.JoinRoom(roomid, password, OnJoinARoomResp);
    }

    public void SendStartGame() // just house-owner send
    {
        client.SendMsg(MsgId.StartGame, Target.All, null);
    }

    public void SendEndGame()
    {
        client.SendMsg(MsgId.EndGame, Target.Server, null);
    }

    public void SendFireMsg(string playerid/*, Vector3 pos, Quaternion qua*/, int type)
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.Fire,
            param = string.Format("{0},{1}", playerid,/* ZUtils.Vector2String(pos), ZUtils.Quaternion2String(qua),*/ type),
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendAddChargeCount()
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.ChargeOnce,
            param = "charge"
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendPlayNextAnim()
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.PlayNextAnim,
            param = "anim"
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendCreateMonsterMsg() //just house-owner send
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.CreateMonster,
            //param = string.Format("{0},{1}", ZUtils.GetUUID() + GameManager.Instance.MonsterNumber, GameManager.Instance.MonsterNumber++),
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendDamageMonsterMsg(string monsterKey, int damageHp, string belongPlayerId)
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.DamageMonster,
            param = string.Format("{0},{1},{2}", monsterKey, damageHp, belongPlayerId),
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendGameOverMsg(bool win) //just house-owner send
    {
        // win = 0   lose = 1
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.GameOver,
            param = win ? "0" : "1",
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendAddScore(string belongid, int score)
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.AddScore,
            param = string.Format("{0},{1}", belongid, score.ToString()),
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendScore(bool win)
    {


        Debug.LogError("[CZ] send score -->" + win);
    }


    public void SendImprisonMonsterMsg(string mk)
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.ImprisonMonster,
            param = mk,
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendChangeWeapon(string playerid, bool isleft)
    {
        CommondInfo commond = new CommondInfo()
        {
            func = S2CFuncName.ChangeWeapon,
            param = string.Format("{0},{1}", playerid, isleft.ToString()),
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    public void SendDeviceInfo()
    {
        //client.SendMsg(MsgId.UploadDeviceInfo, Target.Server, new AppInfo()
        //{
        //    snCode = TowerMain.Instance.SN,
        //    macAddress = Utils.GetMacAddress(),
        //    startUpTimeStamp = TowerMain.Instance.Timestamp
        //});
    }

    public struct AppInfo
    {
        public string snCode;
        public string macAddress;
        public int startUpTimeStamp;
    }

    #endregion

    #region response
    private void OnConnectResp(ColyseusClientResult result, object obj)
    {
        if (result == ColyseusClientResult.Success)
        {
            Debug.Log("[OnConnectResp success]");

            if (ZGlobal.ClientMode == ZClientMode.Visitor)
            {
                GameManager.Instance.VisitModeSearchRoom();
            }

        }
        else
        {
            //ConnectFaildEvent?.Invoke(); move to heartbeat
        }
    }

    private void OnDisConnectResp(object obj)
    {
        // obj is null

    }

    private void OnStartGameResp(object obj)
    {
        Debug.Log("[Server Response] OnStartGameResp --- " + obj);

        Debug.Log("[CZLOG] StartGame , TurnOff Permission To Join Room");
    }

    private void OnJoinARoomResp(ColyseusClientResult result, object obj)
    {
        // obj is Room<State>
        Debug.Log("[Server Response] OnJoinARoomResp --- " + obj);

        if (result == ColyseusClientResult.Success)
        {
            Debug.Log("[CZLOG] Join Room Successfully");
        }
        else
        {
            Debug.Log("[CZLOG] Join Room Failed");
        }
    }

    private void onLeaveRoom(ColyseusClientResult result, object obj)
    {
        if (result == ColyseusClientResult.Success)
        {
            Debug.Log("LeaveRoom Success");
            //GameManager.Instance.RemoveAllPlayerObj();
        }
        else
        {
            Debug.Log("LeaveRoom Failed");
            //GameManager.Instance.RemoveAllPlayerObj();
        }
    }

    private void OnCreateARoomResp(ColyseusClientResult result, object obj)
    {
        // obj is Room<State>
        Debug.Log("[Server Response] OnCreateARoomResp --- " + obj);
        if (result == ColyseusClientResult.Success)
        {
            Debug.Log("[CZLOG] Create a room successful");
            ZGlobal.ClientMode = ZClientMode.Curator;
            GameManager.Instance.ShowHint(HintType.WaitingOthers);
            GameManager.Instance.BeginCountdown();
        }
        else
        {
            Debug.Log("Create Room failed + " + (int)obj);
        }
    }

    private void OnCreateANetObjectResp(object obj)
    {
        Debug.Log("[Server Response] OnCreateANetObjectResp --- " + ((Entity)obj).extraInfo);

        Entity entity = obj as Entity;
        var go = GameObject.Instantiate<PlayerNetObj>(GameManager.Instance.PlayerPrefab);
        go.WeaponTarget = GameObject.Instantiate<GameObject>(GameManager.Instance.WeapoonPrefab.gameObject);
        //go.Bullet = GameObject.Instantiate<GameObject>(GameManager.Instance.BulletPrefab.gameObject);
        go.Init(entity);

        GameManager.Instance.AddPlayerData(entity.owner, go);

        //GameManager.Instance.AddPlayer(entity.owner, go.GetComponent<PlayerNetObjectEntity>());
    }

    private void OnDestroyANetObjectResp(object obj)
    {
        // obj is NetObjectEntity
        Debug.Log("[Server Response] OnDestroyANetObjectResp --- " + obj);

        GameManager.Instance.RemovePlayerData(((NetObjectEntity)obj).entityInfo.owner);

        //GameManager.Instance.ReadyShowPlayUI();
    }

    private void OnUpdateRoomInfoResp(object obj)
    {
        //Debug.Log("[Server Response] OnUpdateRoomInfoResp --- " + obj);
    }

    private void OnUpdateRoomInfoResp(ColyseusClientResult result, object obj)
    {
        //Debug.Log("[Server Response] OnUpdateRoomInfoResp --- " + obj);

        var rooms = (List<CustomRoomAvailable>)obj;

        if (result == ColyseusClientResult.Success)
        {
            // obj is List<CustomRoomAvailable>
            var roomsAvailable = obj as List<CustomRoomAvailable>;
            //Debug.Log("OnGetRoomListResp :" + roomsAvailable.Count);

            if (roomsAvailable.Count > 0)
            {
                var roomState = JsonUtility.FromJson<RoomState>(roomsAvailable[0].metadata.roomInfo);
                if (roomState.state == 0)
                {
                    SendJoinRoomByIdMsg(roomState.roomID);
                }
            }
            else // 没有房间就创建房间进入，开始倒计时
            {
                GameManager.Instance.SendCreateRoom();
            }
        }
        else
        {
            Debug.LogError("[Server Response] OnUpdateRoomInfoResp Failed !!!");
        }
    }

    private void OnCommandResp(object obj)
    {
        CommondInfo commond = obj as CommondInfo;
        GameManager.Instance.S2CFuncTable[commond.func](commond.param);
    }
    #endregion


    #region for send score

    [Serializable]
    public class ScoreInfo
    {
        public ScoreItem[] scores;
        public bool result;
    }

    [Serializable]
    public class ScoreItem
    {
        public string clientID;
        public float score;
    }

    #endregion

}
