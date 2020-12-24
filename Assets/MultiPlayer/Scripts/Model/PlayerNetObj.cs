using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetWorkToolkit;
using NRKernal;

/// <summary>
/// 玩家的网络实体+数据
/// </summary>
public class PlayerNetObj : NetObjectEntity
{
    public GameObject WeaponTarget;
    public Transform ShootPoint;
    public GameObject Bullet;
    public bool FinishScanMarker = false;

    private int mScore;
    public int Score { get; set; }

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

        WeaponTarget.transform.position = Vector3.Lerp(WeaponTarget.transform.position, ei.controllerPosition, 0.35f);
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
        WeaponTarget.transform.position = Vector3.Lerp(WeaponTarget.transform.position, ei.controllerPosition, 0.35f);
        this.entityInfo.extraInfo = JsonUtility.ToJson(ei);
    }

    #endregion

    float attackTimer = 0;
    private void Send_Shoot_HaveInterval_Msg()
    {
        if (attackTimer > ShootIntervalTime)
        {
            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                attackTimer = 0;
                MessageManager.Instance.SendFireMsg(entityInfo.owner, 0);
                Debug.Log("Send_Shoot_HaveInterval_Msg");
            }
        }
        attackTimer += Time.fixedDeltaTime;
    }

    public void Shoot()
    {
        var b = PoolManager.Instance.Get(Bullet);
        b.SetActive(true);
        b.GetComponent<Bullet>().ResetBullet();
        b.transform.position = Bullet.transform.position;
        b.transform.rotation = Bullet.transform.rotation;
        b.transform.localScale = Bullet.transform.lossyScale;
        b.GetComponent<Rigidbody>().AddForce(b.transform.forward * 500);
    }


    #region UnityInterface

    private void Update()
    {
        if (entityInfo == null)
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
            //Send_Shoot_HaveInterval_Msg();
        }
    }

    #endregion


}
