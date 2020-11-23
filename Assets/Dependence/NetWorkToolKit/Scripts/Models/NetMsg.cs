namespace NetWorkToolkit
{
    using Colyseus.Schema;
    using UnityEngine;
    using System;

    [Serializable]
    public struct ConnectResult
    {
        public bool result;
        public string version;
    }

    public enum MsgId
    {
        // Connect server
        Connect = 0,

        // Disconnect from server
        DisConnect = 1,

        // Start game
        StartGame = 2,

        // End game
        EndGame = 3,

        //Create a net object
        CreateANetObject = 4,

        //Destroy a net object
        DestroyANetObject = 5,

        // Command
        Commond = 6,

        // Update scene data
        UpdateRoomInfo = 7,

        // Heart beat package
        HeartBeat = 8,

        UploadScores = 9,

        UploadDeviceInfo = 10,
    }

    public enum Target
    {
        Owner = 0,
        All = 1,
        Server = 2,
        Other = 3,
    }

    public enum EntityType
    {
        Player = 0,
        Normal = 1,
        Enemy = 2,
    }

    [Serializable]
    public struct AppInfo
    {
        public string snCode;
        public string macAddress;
        public int startUpTimeStamp;
    }

    [Serializable]
    public class ScoreInfo
    {
        public ScoreItem[] scores;
    }

    [Serializable]
    public class ScoreItem
    {
        public string clientID;
        public float score;
    }

    [Serializable]
    public class ServerInfo
    {
        public string tag;
        public string ip;
        public string port;
        public string country;

        public string city;
        public string continent;

        public override string ToString()
        {
            return string.Format("tag:{0} ip:{1} port:{2} country:{3} city:{4} continent:{4}", tag, ip, port, country, city, continent);
        }
    }

    [Serializable]
    public class ServerList
    {
        public ServerInfo[] list;
    }
}