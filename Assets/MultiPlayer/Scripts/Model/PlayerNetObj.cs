using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetWorkToolkit;
using NRKernal;

public class PlayerNetObj : NetObjectEntity
{
    public GameObject WeaponTarget;

    public Transform ShootPoint;

    private float ShootIntervalTime = 0.4f;

    private GameObject nrCamera;
    private GameObject ownerGO;

    [SerializeField]
    public struct ExtroInfo
    {
        public Vector3 controllerPosition;
        public Quaternion controlleRotation;
    }


    #region Override

    public override Entity CreateEntity()
    {
        return ModelsUtil.PlayerIdentity();
    }


    public override void Init(Entity info)
    {
        base.Init(info);
        nrCamera = GameObject.Find("NRCameraRig");
        if (isOwner)
        {
            ownerGO = gameObject;
        }
    }

    public override void SerializeData()
    {
        base.SerializeData();

        var ei = JsonUtility.FromJson<ExtroInfo>(entityInfo.extraInfo);
        //WeaponTarget.transform.position = ei.controllerPosition;
        WeaponTarget.transform.rotation = ei.controlleRotation;

        WeaponTarget.transform.position= Vector3.Lerp(WeaponTarget.transform.position, ei.controllerPosition, 0.6f);
    }

    public override void DeSerializeData()
    {

        base.transform.position = nrCamera.transform.position;
        base.transform.rotation = nrCamera.transform.rotation;

        ExtroInfo ei = new ExtroInfo();

        ei.controllerPosition = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightModelAnchor).position;
        ei.controlleRotation = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightModelAnchor).rotation;

        //WeaponTarget.transform.position = ei.controllerPosition;
        WeaponTarget.transform.rotation = ei.controlleRotation;
        WeaponTarget.transform.position = Vector3.Lerp(WeaponTarget.transform.position, ei.controllerPosition, 0.6f);
        this.entityInfo.extraInfo = JsonUtility.ToJson(ei);
    }

    #endregion

    float attackTimer = 0;
    private void Shoot_HaveInterval()
    {
        if(attackTimer > ShootIntervalTime)
        {
            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                attackTimer = 0;

                MessageManager.Instance.SendFireMsg(entityInfo.owner, ShootPoint.position, ShootPoint.rotation, 0);
            }
        }


        attackTimer += Time.fixedDeltaTime;

    }


    #region UnityInterface

    private void Update()
    {
        if(entityInfo == null)
        {
            return;
        }
        if (!isOwner)
        {
            return;
        }
        // 创建房间者不参与游戏射击
        if (IsRoomOwner())
        {
            return;
        }

        // 游戏开始方可设计
        if (GameManager.Instance.BeginGame)
        {
            Shoot_HaveInterval();
        }
    }

    #endregion


}
