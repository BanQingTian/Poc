using UnityEngine;

namespace NetWorkToolkit
{
    public class NetObjectEntity : MonoBehaviour
    {
        public Entity entityInfo;
        public bool isOwner;

        public virtual void Init(Entity info)
        {
            entityInfo = CreateEntity();
            info.Copy(entityInfo);
            isOwner = IsOwner();
            ColyseusClient.instance.RegistEntityObject(info.id, this);
        }

        public virtual Entity CreateEntity()
        {
            return ModelsUtil.EntityIdentity();
        }

        public void DownLoadInfo(Entity entity)
        {
            if (isOwner) return;
            if (!entity.id.Equals(entityInfo.id))
            {
                Debug.LogError("Id not match!!!");
                return;
            }
            entity.Copy(entityInfo);
            this.SerializeData();
        }

        public void UpLoadInfo()
        {
            if (isOwner)
            {
                DeSerializeData();
                ColyseusClient.instance.SyncEntity(entityInfo);
            }
        }

        // Serialize the entity data
        public virtual void SerializeData()
        {
            this.transform.position = entityInfo.position.ToUnity();
            this.transform.rotation = entityInfo.rotation.ToUnity();
        }

        public virtual void DeSerializeData()
        {
            entityInfo.position = transform.position.ToNet();
            entityInfo.rotation = transform.rotation.ToNet();
        }

        // is room creater
        public bool IsRoomOwner()
        {
            if (entityInfo != null && ColyseusClient.instance.GetCurrentRoom().State.owner == ColyseusClient.instance.SessionID)
                return true;
            return false;
        }

        private bool IsOwner()
        {
            if (entityInfo.owner.Equals(ColyseusClient.instance.SessionID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
