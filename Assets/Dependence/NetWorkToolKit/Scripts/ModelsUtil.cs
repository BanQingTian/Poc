using Colyseus;
using Colyseus.Schema;
using UnityEngine;

public static class ModelsUtil
{
    public static Quaternion ToUnity(this NetQuaternion qua)
    {
        Quaternion q = new Quaternion(qua.x, qua.y, qua.z, qua.w);
        return q;
    }

    public static NetQuaternion ToNet(this Quaternion qua)
    {
        NetQuaternion q = new NetQuaternion()
        {
            x = qua.x,
            y = qua.y,
            z = qua.z,
            w = qua.w
        };
        return q;
    }
    public static Vector3 ToUnity(this NetVector3 pos)
    {
        Vector3 v = new Vector3(pos.x, pos.y, pos.z);
        return v;
    }

    public static NetVector3 ToNet(this Vector3 pos)
    {
        NetVector3 v = new NetVector3()
        {
            x = pos.x,
            y = pos.y,
            z = pos.z
        };
        return v;
    }

    public static void Copy(this Entity entity, Entity destiny)
    {
        destiny.id = entity.id;
        destiny.owner = entity.owner;
        destiny.name = entity.name;
        if (destiny.position == null)
        {
            destiny.position = new NetVector3();
        }
        destiny.position.x = entity.position.x;
        destiny.position.y = entity.position.y;
        destiny.position.z = entity.position.z;
        if (destiny.rotation == null)
        {
            destiny.rotation = new NetQuaternion();
        }
        destiny.rotation.x = entity.rotation.x;
        destiny.rotation.y = entity.rotation.y;
        destiny.rotation.z = entity.rotation.z;
        destiny.rotation.w = entity.rotation.w;
        destiny.type = entity.type;
        destiny.extraInfo = entity.extraInfo;

        if ((entity is Player) && (destiny is Player))
        {
            ((Player)destiny).isconnect = ((Player)entity).isconnect;
        }
        else if ((entity is Enemy) && (destiny is Enemy))
        {
            ((Enemy)destiny).healthy = ((Enemy)entity).healthy;
        }
    }

    public static Entity EntityIdentity()
    {
        Entity destiny = new Entity();
        destiny.position = new NetVector3();
        destiny.rotation = new NetQuaternion();
        return destiny;
    }

    public static Player PlayerIdentity()
    {
        Player destiny = new Player();
        destiny.position = new NetVector3();
        destiny.rotation = new NetQuaternion();
        destiny.isconnect = true;
        return destiny;
    }

    public static Enemy EnemyIdentity()
    {
        Enemy destiny = new Enemy();
        destiny.position = new NetVector3();
        destiny.rotation = new NetQuaternion();
        destiny.healthy = 1;
        return destiny;
    }


    public static string GetString(this Entity entity)
    {
        return string.Format("id:{0} name:{1} pos:{2} rotation:{3}", entity.id, entity.name, entity.position.ToUnity().ToString()
        , entity.rotation.ToUnity().ToString());
    }

    public static string GetString(this Message msg)
    {
        return string.Format("id:{0} target:{1} content:{2}", msg.header.msgID, msg.header.target, msg.content);
    }

    public static string GetString(this CommondInfo msg)
    {
        return string.Format("func:{0} param:{1}", msg.func, msg.param);
    }

    public static string GetString(this RoomState room)
    {
        return string.Format("client num:{0} owner:{1}", room.entities.Count, room.owner);
    }
}