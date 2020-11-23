using Colyseus;
using Colyseus.Schema;
using NetWorkToolkit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class SocketIOExample : MonoBehaviour
{
    public Button ConnectBtn;
    public Button GetRoomBtn;
    public Button CreateRoomBtn;
    public Button JoinRoomBtn;
    public Button LeaveRoomBtn;
    public Button SendMsgBtn;
    public Button CreateBossBtn;
    public Button ShootBossBtn;
    public Button StartGameBtn;
    public Button EndGameBtn;
    public Button UploadScoresBtn;
    public Button UploadDeviceinfoBtn;
    public Text msgTips;
    public Text roomInfo;

    public enum ServerType
    {
        Product = 0,
        Test,
        Local,
        TestLocal,
        TestGerm

    }

    public ServerType serverType = ServerType.Test;

    private List<string> ServerList = new List<string>
    {
        "47.254.87.239",
        "192.168.71.140",
        "localhost",
        "192.168.71.69",
        "80.187.128.42"
    };

    ColyseusClient client;

    private Dictionary<string, GameObject> netObjDict = new Dictionary<string, GameObject>();

    void Start()
    {
        client = ColyseusClient.instance;
        GetServerList();

        client.AddListener(MsgId.Commond, OnCommandResp);
        client.AddListener(MsgId.Connect, OnConnectResp);
        client.AddListener(MsgId.DisConnect, OnDisConnectResp);
        client.AddListener(MsgId.CreateANetObject, OnCreateANetObjectResp);
        //client.AddListener(MsgId.CreateARoom, OnCreateARoomResp);
        client.AddListener(MsgId.DestroyANetObject, OnDestroyANetObjectResp);
        //client.AddListener(MsgId.GetRoomList, OnGetRoomListResp);
        //client.AddListener(MsgId.JoinARoom, OnJoinARoomResp);
        //client.AddListener(MsgId.LeaveARoom, OnLeaveARoomResp);
        client.AddListener(MsgId.StartGame, OnStartGameResp);
        client.AddListener(MsgId.UpdateRoomInfo, OnUpdateRoomInfo);

        #region btn event
        ConnectBtn.onClick.AddListener(() =>
        {
            //test :192.168.71.140
            client.ConnectToServer(ServerList[(int)serverType], "2567", (result, obj) =>
             {
                 if (result == ColyseusClient.ColyseusClientResult.Success)
                 {
                     Debug.Log("Connect server success!!!");
                 }
                 else
                 {
                     Debug.Log("Connect server failed!!!");
                 }
             });
        });
        GetRoomBtn.onClick.AddListener(() =>
        {
            client.GetAvailableRooms((result, obj) =>
            {
                if (result == ColyseusClient.ColyseusClientResult.Success)
                {
                    Debug.Log("Get rooms success!!!");
                    OnGetRoomListResp(obj);
                }
                else
                {
                    Debug.Log("Get rooms failed!!!");
                }
            });
            //client.SendMsg(MsgId.GetRoomList, Target.Owner, null);
        });
        CreateRoomBtn.onClick.AddListener(() =>
        {
            client.CreateRoom("testRoom", "123456", (result, obj) =>
              {
                  if (result == ColyseusClient.ColyseusClientResult.Success)
                  {
                      Debug.Log("Create Room success!!!");
                      OnCreateARoomResp(obj);
                  }
                  else
                  {
                      Debug.Log("Create Room failed :" + obj);
                  }
              });
        });
        JoinRoomBtn.onClick.AddListener(() =>
        {
            client.JoinOrCreateRoom("123456", (result, obj) =>
             {
                 if (result == ColyseusClient.ColyseusClientResult.Success)
                 {
                     Debug.Log("Join a room success!!!");
                 }
                 else
                 {
                     Debug.Log("Join a room failed! error code is :" + obj);
                 }
             });
        });
        LeaveRoomBtn.onClick.AddListener(() =>
        {
            client.LeaveRoom((result, obj) =>
            {
                if (result == ColyseusClient.ColyseusClientResult.Success)
                {
                    Debug.Log("Leave Room success!!!");
                    OnLeaveARoomResp(obj);
                }
                else
                {
                    Debug.Log("Leave Room failed!!!");
                }
            });
        });
        SendMsgBtn.onClick.AddListener(() =>
        {
            SendCommond();
        });
        CreateBossBtn.onClick.AddListener(() =>
        {
            CreateAEnemy();
        });
        ShootBossBtn.onClick.AddListener(() =>
        {
            CommondInfo commond = new CommondInfo()
            {
                func = "damage",
                param = string.Format("{0},{1}", boss.enemyInfo.id, 0.1f)
            };
            client.SendMsg(MsgId.Commond, Target.All, commond);
        });
        StartGameBtn.onClick.AddListener(() =>
        {
            client.SendMsg(MsgId.StartGame, Target.All, null);
        });
        EndGameBtn.onClick.AddListener(() =>
        {
            client.SendMsg(MsgId.EndGame, Target.Server, null);
        });
        UploadScoresBtn.onClick.AddListener(() =>
        {
            var ScoreInfo = new ScoreInfo();
            ScoreInfo.scores = new ScoreItem[4];
            ScoreInfo.scores[0] = new ScoreItem() { clientID = "11111", score = 12 };
            ScoreInfo.scores[1] = new ScoreItem() { clientID = "22222", score = 12 };
            ScoreInfo.scores[2] = new ScoreItem() { clientID = "33333", score = 12 };
            ScoreInfo.scores[3] = new ScoreItem() { clientID = "44444", score = 12 };
            client.SendMsg(MsgId.UploadScores, Target.Server, ScoreInfo);
        });
        UploadDeviceinfoBtn.onClick.AddListener(() =>
        {
            client.SendMsg(MsgId.UploadDeviceInfo, Target.Server, new AppInfo()
            {
                snCode = "0000-1111",
                macAddress = Utils.GetMacAddress(),
                startUpTimeStamp = 112312123
            }); ;
        });
        #endregion
    }

    private async void GetServerList()
    {
        var result = await client.GetServerList();
        foreach (var item in result)
        {
            Debug.Log(item.ToString());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            client.CreateRoom("testRoom", "123456", (result, obj) =>
            {
                if (result == ColyseusClient.ColyseusClientResult.Success)
                {
                    Debug.Log("Create Room success!!!");
                    OnCreateARoomResp(obj);
                }
                else
                {
                    Debug.Log("Create Room failed, error code:" + obj);
                }
            });
        }
    }

    #region response
    private void OnConnectResp(object obj)
    {
        //obj is Message
        Debug.Log("OnConnectResp");
    }

    private void OnDisConnectResp(object obj)
    {
        // obj is Message
        Debug.Log("OnDisConnectResp");
        Clear();
    }

    private void OnStartGameResp(object obj)
    {
        // obj is Message
        Debug.Log("OnStartGameResp");
    }

    private void OnLeaveARoomResp(object obj)
    {
        Clear();
        // obj is Room<RoomState>
        if (obj != null) Debug.Log("OnLeaveARoomResp:" + ((Room<RoomState>)obj).Id);
    }

    private void Clear()
    {
        foreach (var item in netObjDict)
        {
            DestroyImmediate(item.Value.gameObject);
        }
        netObjDict.Clear();
        roomInfo.text = string.Empty;
    }

    private void OnJoinARoomResp(object obj)
    {
        // obj is Room<RoomState>
        Debug.Log("OnJoinARoomResp :" + ((Room<RoomState>)obj).State.GetString());
    }

    private void OnGetRoomListResp(object obj)
    {
        // obj is List<CustomRoomAvailable>
        var roomsAvailable = obj as List<CustomRoomAvailable>;
        Debug.Log("OnGetRoomListResp :" + roomsAvailable.Count);
        for (var i = 0; i < roomsAvailable.Count; i++)
        {
            var roomstate = JsonUtility.FromJson<RoomState>(roomsAvailable[i].metadata.roomInfo);
            Debug.Log(roomstate.roomName + " statte:" + roomstate.state + " owner :" + roomstate.owner);
        }
    }

    private void OnCreateARoomResp(object obj)
    {
        // obj is Room<RoomState>
        Debug.Log("OnCreateARoomResp :" + ((Room<RoomState>)obj).State.owner + " name:" + ((Room<RoomState>)obj).State.roomName);
    }

    private EnemyNetObjectExample boss;
    private void OnCreateANetObjectResp(object obj)
    {
        // obj is Entity
        Entity entity = obj as Entity;
        Debug.Log("OnCreateANetObjectResp :" + entity.ToString());
        GameObject go;
        NetObjectEntity netobject;

        if (entity.type == (int)EntityType.Player)
        {
            // create by entity.type
            go = Instantiate(Resources.Load("Player")) as GameObject;
            netobject = go.GetComponent<PlayerNetObject>();
        }
        else if (entity.type == (int)EntityType.Enemy)
        {
            // create by entity.type
            go = Instantiate(Resources.Load("Enemy")) as GameObject;
            netobject = go.GetComponent<EnemyNetObjectExample>();
            boss = (EnemyNetObjectExample)netobject;
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            netobject = go.AddComponent<NetObjectEntity>();
        }
        netobject.Init(entity);
        netObjDict.Add(entity.id, go);
    }

    private void OnDestroyANetObjectResp(object obj)
    {
        // obj is NetObjectEntity
        Debug.Log("OnDestroyANetObjectResp :" + ((NetObjectEntity)obj).gameObject.name);
        var id = ((NetObjectEntity)obj).entityInfo.id;
        GameObject go;
        netObjDict.TryGetValue(id, out go);
        DestroyImmediate(go);
    }

    private void OnCommandResp(object obj)
    {
        // obj is CommondInfo
        var commond = obj as CommondInfo;
        if (commond.func.Equals("damage"))
        {
            var strs = commond.param.Split(',');
            boss.Damage(float.Parse(strs[1]));
        }
        msgTips.text = commond.GetString();
    }

    private void OnUpdateRoomInfo(object obj)
    {
        // obj is a RoomState
        Debug.Log("OnUpdateRoomInfo " + ((RoomState)obj).GetString());
        roomInfo.text = string.Format("Name:{0}\nState:{1}\nOwner:{2}\nNum:{3}", ((RoomState)obj).roomName, ((RoomState)obj).state, ((RoomState)obj).owner, ((RoomState)obj).entities.Count);
    }
    #endregion

    #region test
    // Send a commond
    public void SendCommond()
    {
        CommondInfo commond = new CommondInfo()
        {
            func = "test",
            param = "hello,world,1,2,3"
        };
        client.SendMsg(MsgId.Commond, Target.All, commond);
    }

    // Send a start msg
    public void StartGame()
    {
        client.SendMsg(MsgId.StartGame, Target.All, null);
    }

    public void CreateAEnemy()
    {
        client.SendMsg(MsgId.CreateANetObject, Target.Server, EntityType.Enemy);
    }
    #endregion
}

