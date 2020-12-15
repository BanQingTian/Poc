using Colyseus;
using Colyseus.Schema;
using GameDevWare.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NetWorkToolkit
{
    public class ColyseusClient
    {
        public readonly int PSW_MISMATCH_ERROCODE = 1000;
        public readonly int JOIN_ROOM_NOTALLOWED_ERROCODE = 1001;
        public readonly int ALREADY_INROOM_ERROCODE = 1002;
        private string UUID;

        private static ColyseusClient m_Instance = null;
        public static ColyseusClient instance
        {

            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new ColyseusClient();
                    m_Instance.UUID = System.Guid.NewGuid().ToString("N");
                }
                return m_Instance;
            }
        }

        public enum ColyseusClientResult
        {
            Success,
            Failed,
            InvalidAgument
        }

        public delegate void ColyseusClientResultEvent(ColyseusClientResult result, object obj);
        public delegate void ColyseusClientListener(object msg);
        public Dictionary<MsgId, ColyseusClientListener> m_MsgListener;
        protected Client client;
        protected Room<RoomState> room;
        public string roomName = "Nreal";
        private string m_ServerIP = "localhost";
        protected IndexedDictionary<string, NetObjectEntity> entities = new IndexedDictionary<string, NetObjectEntity>();
        public enum NetState
        {
            Uninitialized,
            Disconnect,
            Connect,
            JoinARoom
        }

        private NetState currentState = NetState.Uninitialized;

        private const int TimeOut = 2;

        public static string localIP;

        public ColyseusClient()
        {
            m_MsgListener = new Dictionary<MsgId, ColyseusClientListener>();
            ClientUpdator.Instance.StartCoroutine(SynEntityInfoCorutine());
            m_MsgListener.Add(MsgId.HeartBeat, OnHeartBeat);
            currentState = NetState.Disconnect;
            GetPublicIPAddress();
        }

        public async void GetPublicIPAddress()
        {
            localIP = await new WebClient().DownloadStringTaskAsync("https://api.ipify.org");
            Debug.Log("GetPublicIPAddress success:" + localIP);
        }

        public Room<RoomState> GetCurrentRoom()
        {
            return room;
        }

        public string SessionID
        {
            get
            {
                if (room != null)
                {
                    return room.SessionId;
                }
                else return null;
            }
        }

        public void AddListener(MsgId id, ColyseusClientListener listener)
        {
            if (m_MsgListener.ContainsKey(id))
            {
                Debug.Log("Msg listener has repeated!!");
                return;
            }
            m_MsgListener[id] = listener;
        }

        public void RemoveListener(MsgId id, ColyseusClientListener listener)
        {
            if (!m_MsgListener.ContainsKey(id))
            {
                Debug.Log("Msg listener was not exist!");
                return;
            }
            m_MsgListener.Remove(id);
        }

        private void OprateServerMsg(Message pack)
        {
            ColyseusClientListener listener;
            if (m_MsgListener.TryGetValue((MsgId)pack.header.msgID, out listener))
            {
                listener?.Invoke(TryGetObjectByID(pack));
            }
        }

        private double lastHeartBeat = -1;
        private void OnHeartBeat(object msg)
        {
            lastHeartBeat = GetTimeStamp();
            currentState = NetState.JoinARoom;
        }

        public double GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalSeconds;
        }

        private object TryGetObjectByID(Message msg)
        {
            object obj;
            switch (msg.header.msgID)
            {
                case (int)MsgId.Commond:
                    obj = JsonUtility.FromJson<CommondInfo>(msg.content);
                    break;
                default:
                    obj = msg;
                    break;
            }

            return obj;
        }

        public void RegistEntityObject(string id, NetObjectEntity obj)
        {
            entities[id] = obj;
        }

        private IEnumerator SynEntityInfoCorutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f / 30);
                if (this.currentState == NetState.JoinARoom)
                {
                    foreach (var item in entities)
                    {
                        if (entities != null)
                            item.Value.UpLoadInfo();
                    }
                }
            }
        }

        private IEnumerator HeartBeat()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                this.SendMsg(MsgId.HeartBeat, Target.Server, null);

                if ((currentState == NetState.JoinARoom || currentState == NetState.Connect)
                    && (GetTimeStamp() - lastHeartBeat) > TimeOut)
                {
                    Debug.LogError("Time out!!! Leave the room!");
                    // Time out...
                    OnLeaveRoom(NativeWebSocket.WebSocketCloseCode.Normal);
                    lastHeartBeat = GetTimeStamp();
                }
            }
        }
        #region colyseus client

        public async Task<List<ServerInfo>> GetServerList()
        {
            var uriBuilder = new UriBuilder(Constant.GateServerURL);

            var req = new UnityWebRequest(Constant.GateServerURL, "GET");
            req.SetRequestHeader("Accept", "application/json");
            req.downloadHandler = new DownloadHandlerBuffer();
            await req.SendWebRequest();

            var json = req.downloadHandler.text;
            Debug.Log("response:" + json);
            List<ServerInfo> response;
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            else
            {
                response = LitJson.JsonMapper.ToObject<List<ServerInfo>>(json);
            }
            return response;
        }

        public async void ConnectToServer(string host, string port, ColyseusClientResultEvent callback)
        {
            //this.m_ServerIP = ip;
            string endpoint = null;

            //string endpoint = string.Format("{0}://{1}", NetWorkToolkit.Constant.DefaultScheme, host);

            switch (ZGlobal.ServiceMode)
            {
                case ZServiceMode.LOCAL_HTTP:
                    m_ServerIP = host;
                    ZConstant.DefaultScheme = "ws";
                    ZConstant.ReplaceScheme = "http";
                    endpoint = string.Format("ws://{0}:{1}", m_ServerIP, port);
                    break;
                case ZServiceMode.LOCAL_HTTPS:
                    ZConstant.DefaultScheme = "wss";
                    ZConstant.ReplaceScheme = "https";
                    endpoint = string.Format("{0}://{1}", ZConstant.DefaultScheme, host);
                    break;
                case ZServiceMode.CLOUD:
                    // TODO
                    break;
                default:
                    break;
            }


            Debug.Log("Connect to server:" + endpoint);
            // Connect to the colyeus Server
            client = ColyseusManager.Instance.CreateClient(endpoint, "");
            //var result = await client.IsServerAvailable();
            //if (result.result)
            {
                //Debug.Log("Server is available version:" + result.version);
                callback?.Invoke(ColyseusClientResult.Success, null);
                ClientUpdator.Instance.StopCoroutine(HeartBeat());
                ClientUpdator.Instance.StartCoroutine(HeartBeat());
            }
            //else
            //{
            //    callback?.Invoke(ColyseusClientResult.Failed, null);
            //}
        }

        private ColyseusClientResultEvent onCreateRoom;
        private bool _CreateRoomLock = false;
        public async void CreateRoom(string name = "gameroom", string psw = "123456", ColyseusClientResultEvent callback = null)
        {
            if (_CreateRoomLock)
            {
                return;
            }
            _CreateRoomLock = true;
            if (currentState == NetState.JoinARoom)
            {
                _CreateRoomLock = false;
                Debug.LogError("Already in a room!!!!!!!!!!!");
                callback?.Invoke(ColyseusClientResult.Failed, ALREADY_INROOM_ERROCODE);
                return;
            }
            try
            {
                var option = new Dictionary<string, object>() { { "roomName", name }, { "psw", psw }, { "ip", localIP }, { "uuid", UUID } };
                room = await client.Create<RoomState>(roomName, option);
                onCreateRoom = callback;
                _CreateRoomLock = false;
            }
            catch (Exception e)
            {
                _CreateRoomLock = false;
                Debug.Log("join error :" + e.Message);
                callback?.Invoke(ColyseusClientResult.Failed, GetErrorCode(e));
            }
            lastHeartBeat = GetTimeStamp();
            this.RegisRoomEvent(room);
            currentState = NetState.Connect;
        }

        private bool _JoinRoomLock = false;
        public async void JoinRoom(string roomId, string psw = "1234567", ColyseusClientResultEvent callback = null)
        {
            if (_JoinRoomLock)
            {
                return;
            }
            _JoinRoomLock = true;
            if (currentState == NetState.JoinARoom)
            {
                _JoinRoomLock = false;
                callback?.Invoke(ColyseusClientResult.Failed, ALREADY_INROOM_ERROCODE);
                return;
            }
            try
            {
                room = await client.JoinById<RoomState>(roomId, new Dictionary<string, object>() { { "psw", psw }, { "uuid", UUID } });
                _JoinRoomLock = false;
            }
            catch (Exception e)
            {
                _JoinRoomLock = false;
                Debug.LogError(e.Message);
                callback?.Invoke(ColyseusClientResult.Failed, GetErrorCode(e));
                throw;
            }
            lastHeartBeat = GetTimeStamp();
            this.RegisRoomEvent(room);
            callback?.Invoke(ColyseusClientResult.Success, room);
            currentState = NetState.Connect;
        }

        public async void JoinOrCreateRoom(string psw = "123456", ColyseusClientResultEvent callback = null)
        {
            if (_JoinRoomLock)
            {
                return;
            }
            _JoinRoomLock = true;
            if (currentState == NetState.JoinARoom)
            {
                _JoinRoomLock = false;
                callback?.Invoke(ColyseusClientResult.Failed, ALREADY_INROOM_ERROCODE);
                return;
            }
            try
            {
                room = await client.JoinOrCreate<RoomState>(roomName, new Dictionary<string, object>() { { "psw", psw }, { "uuid", UUID } });
                _JoinRoomLock = false;
            }
            catch (Exception e)
            {
                _JoinRoomLock = false;
                Debug.Log(e.Message);
                callback?.Invoke(ColyseusClientResult.Failed, GetErrorCode(e));
                throw;
            }
            lastHeartBeat = GetTimeStamp();
            this.RegisRoomEvent(room);
            callback?.Invoke(ColyseusClientResult.Success, room);
            currentState = NetState.Connect;
        }

        private int GetErrorCode(Exception e)
        {
            string code = e.Message.Split(':')[0];
            int error_code;
            int.TryParse(code, out error_code);
            return error_code;
        }

        //private async void JoinRoom()
        //{
        //    if (room != null)
        //    {
        //        this.LeaveRoom();
        //    }
        //    room = await client.Join<RoomState>(roomName, new Dictionary<string, object>() { });

        //    this.RegisRoomEvent(room);

        //    currentState = NetState.Connect;
        //}

        //public async void ReconnectRoom()
        //{
        //    if (room == null)
        //    {
        //        return;
        //    }
        //    string roomId = PlayerPrefs.GetString("roomId");
        //    string sessionId = PlayerPrefs.GetString("sessionId");
        //    if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(roomId))
        //    {
        //        Debug.Log("Cannot Reconnect without having a roomId and sessionId");
        //        return;
        //    }

        //    room = await client.Reconnect<RoomState>(roomId, sessionId);
        //    Debug.Log("Reconnected into room successfully.");

        //    RegisRoomEvent(room);
        //    currentState = NetState.Connect;
        //}

        private void RegisRoomEvent(Room<RoomState> room)
        {
            if (room == null)
            {
                return;
            }
            PlayerPrefs.SetString("roomId", room.Id);
            PlayerPrefs.SetString("sessionId", room.SessionId);
            PlayerPrefs.Save();

            room.OnLeave += OnLeaveRoom;
            room.OnError += (message) => Debug.LogError(message);
            room.OnStateChange += OnStateChangeHandler;
            room.OnMessage += OnMessage;

            room.State.entities.OnAdd += OnEntityAdd;
            room.State.entities.OnRemove += OnEntityRemove;
            room.State.entities.OnChange += OnEntityChange;
        }

        public async void LeaveRoom(ColyseusClientResultEvent callback = null)
        {
            if (room == null || currentState == NetState.Disconnect)
            {
                return;
            }
            try
            {
                await room.Leave(false);
            }
            catch (Exception)
            {
                Clear();
                callback?.Invoke(ColyseusClientResult.Failed, null);
                throw;
            }
            Clear();
            callback?.Invoke(ColyseusClientResult.Success, room);
        }

        private void OnLeaveRoom(NativeWebSocket.WebSocketCloseCode code)
        {
            Debug.Log("OnLeaveRoom:" + code);
            Trigger(MsgId.DisConnect, null);
            currentState = NetState.Disconnect;
            Clear();
        }

        private void Clear()
        {
            room = null;
            entities.Clear();
        }

        private void Trigger(MsgId id, object content = null)
        {
            ColyseusClientListener listener;
            if (m_MsgListener.TryGetValue(id, out listener))
            {
                listener?.Invoke(content);
            }
        }

        public async void GetAvailableRooms(ColyseusClientResultEvent callback)
        {
            CustomRoomAvailable[] roomsAvailable;
            try
            {
                roomsAvailable = await client.GetAvailableRooms<CustomRoomAvailable>(roomName);
            }
            catch (Exception)
            {
                callback?.Invoke(ColyseusClientResult.Failed, null);
                throw;
            }
            List<CustomRoomAvailable> validRooms = new List<CustomRoomAvailable>();
            foreach (var item in roomsAvailable)
            {
                if (item != null && item.clients > 0 && item.metadata != null)
                {
                    validRooms.Add(item);
                }
            }
            callback?.Invoke(ColyseusClientResult.Success, validRooms);
        }

        void OnMessage(object msg)
        {
            if (msg is Message)
            {
                OprateServerMsg((Message)msg);
            }
        }

        void OnStateChangeHandler(RoomState state, bool isFirstState)
        {
            if (currentState == NetState.Connect)
            {
                currentState = NetState.JoinARoom;
                onCreateRoom?.Invoke(ColyseusClientResult.Success, room);
                onCreateRoom = null;
            }
            else
            {
                // update the state
                Trigger(MsgId.UpdateRoomInfo, state);
            }
        }

        void OnEntityAdd(Entity entity, string key)
        {
            Trigger(MsgId.CreateANetObject, entity);
        }

        void OnEntityRemove(Entity entity, string key)
        {
            if (!entities.ContainsKey(entity.id))
            {
                Debug.Log("There is not the entity.");
                return;
            }
            NetObjectEntity obj;
            entities.TryGetValue(entity.id, out obj);
            entities.Remove(entity.id);
            Trigger(MsgId.DestroyANetObject, obj);
        }

        void OnEntityChange(Entity entity, string key)
        {
            //Debug.Log("OnEntityChange :" + entity.id + " pos:" + entity.GetString());
            NetObjectEntity obj;
            entities.TryGetValue(entity.id, out obj);
            obj?.DownLoadInfo(entity);
            if (entity is Player && !((Player)entity).isconnect)
            {
                this.OnEntityRemove(entity, entity.id);
            }
        }
        #endregion

        #region request
        public void SendMsg(MsgId id, Target tar, object obj)
        {
            if (currentState != NetState.JoinARoom)
            {
                return;
            }

            Message pack = new Message();
            pack.header = new Header()
            {
                msgID = (int)id,
                target = (int)tar
            };
            pack.content = obj == null ? "" : JsonUtility.ToJson(obj);
            // Debug.Log("send a msg:" + pack.GetString());
            SendMsg(pack);
        }

        public void SyncEntity(Entity entity)
        {
            SendMsg(entity);
        }

        public void SendMsg(object msg)
        {
            if (currentState != NetState.JoinARoom)
            {
                return;
            }

            room?.Send(msg);
        }
        #endregion
    }
}
