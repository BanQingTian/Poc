using Colyseus;
using System;

[Serializable]
public class Metadata
{
    public string roomInfo;
}

[Serializable]
public class CustomRoomAvailable : RoomAvailable
{
    public Metadata metadata;

    public override string ToString()
    {
        return string.Format("clients:{0} maxClients:{1}  roomInfo:{2}", clients, maxClients, metadata.roomInfo);
    }
}