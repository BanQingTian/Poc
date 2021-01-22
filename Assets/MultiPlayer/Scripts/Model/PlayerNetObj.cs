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

    public void SetWeaponAndBullet(GameObject w, GameObject b)
    {
        WeaponTarget = w;
        Bullet = b;
    }

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
        WeaponTarget.transform.position = ei.controllerPosition;
        WeaponTarget.transform.rotation = ei.controlleRotation;

        ShootPoint.transform.position = WeaponTarget.transform.position /*+ new Vector3(0, 0, 0.1f)*/;
        ShootPoint.transform.rotation = WeaponTarget.transform.rotation;

        //WeaponTarget.transform.position = Vector3.Lerp(WeaponTarget.transform.position, ei.controllerPosition, 0.35f);
    }

    public override void DeSerializeData()
    {

        base.transform.position = nrCamera.transform.position;
        base.transform.rotation = nrCamera.transform.rotation;

        ExtroInfo ei = new ExtroInfo();

        ei.controllerPosition = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightModelAnchor).position;
        ei.controlleRotation = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightModelAnchor).rotation;


        WeaponTarget.transform.position = ei.controllerPosition;
        WeaponTarget.transform.rotation = ei.controlleRotation;

        ShootPoint.transform.position = WeaponTarget.transform.position /*+ new Vector3(0, 0, 0.1f)*/;
        ShootPoint.transform.rotation = WeaponTarget.transform.rotation;
        //WeaponTarget.transform.position = Vector3.Lerp(WeaponTarget.transform.position, ei.controllerPosition, 0.35f);
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

    public void Shoot(string belongid)
    {
        var b = PoolManager.Instance.Get(Bullet);
        b.SetActive(true);

        Bullet sb = b.GetComponent<Bullet>();
        if (sb == null)
        {
            sb = b.AddComponent<Bullet>();
        }
        sb.BelongToPlayerID = belongid;
        // b.GetComponent<Bullet>().ResetBullet();
        b.transform.position = ShootPoint.transform.position;
        b.transform.rotation = ShootPoint.transform.rotation;
        b.transform.localScale = Bullet.transform.lossyScale;

        b.GetComponent<Collider>().isTrigger = true;

        var rig = b.GetComponent<Rigidbody>();
        if (rig == null)
        {
            rig = b.AddComponent<Rigidbody>();
        }
        rig.useGravity = false;
        rig.isKinematic = false;

        rig.AddForce(b.transform.forward * 100);
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
    }

    #endregion


}
