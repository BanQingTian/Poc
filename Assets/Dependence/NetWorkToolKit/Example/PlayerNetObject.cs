
using NetWorkToolkit;
using UnityEngine;
using System;

public class PlayerNetObject : NetObjectEntity
{
    public TextMesh m_ID;
    public float speed = 0.1f;
    public Transform controller;
    [SerializeField]
    public struct ExtroInfo
    {
        public Vector3 controllerPosition;
        public Quaternion controlleRotation;
    }

    public override void Init(Entity info)
    {
        base.Init(info);
        Debug.Log("Init the player :" + entityInfo.id);
        m_ID.text = entityInfo.id;
    }

    public override Entity CreateEntity()
    {
        return ModelsUtil.PlayerIdentity();
    }

    void Update()
    {
        if (isOwner)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += speed * Vector3.up;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += speed * Vector3.down;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += speed * Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += speed * Vector3.right;
            }
        }
    }

    public override void SerializeData()
    {
        base.SerializeData();
        // Do your serialize
        var extroinfo = JsonUtility.FromJson<ExtroInfo>(entityInfo.extraInfo);
        controller.transform.position = extroinfo.controllerPosition;
        controller.transform.rotation = extroinfo.controlleRotation;
    }

    public override void DeSerializeData()
    {
        base.DeSerializeData();
        // Do your serialize
        ExtroInfo extro = new ExtroInfo();
        extro.controllerPosition = controller.transform.position;
        extro.controlleRotation = controller.transform.rotation;
        this.entityInfo.extraInfo = JsonUtility.ToJson(extro);
    }
}