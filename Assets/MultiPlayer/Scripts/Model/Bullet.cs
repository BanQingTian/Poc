using NetWorkToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rig;
    public Rigidbody m_Rig
    {
        get
        {
            if (rig == null)
            {
                if (GetComponent<Rigidbody>() == null)
                {
                    rig = gameObject.AddComponent<Rigidbody>();
                }
                else
                {
                    rig = GetComponent<Rigidbody>();
                }
            }
            return rig;
        }
    }
    public bool Invalid = false;
    public string BelongToPlayerID;


    private void OnEnable()
    {
        Invoke("ReleaseObj", 2);
    }

    public void OnTriggerEnter(Collider other)
    {
        // 这个子弹只有是自己射击的才算碰撞 ，触发分数
        if (BelongToPlayerID == ColyseusClient.instance.SessionID)
            GameManager.Instance.ShootTarget();
        gameObject.transform.position = new Vector3(9999, 9999, 9999);
    }

    public void ResetBullet()
    {
        m_Rig.isKinematic = false;
        Invalid = false;
    }

    private void ReleaseObj()
    {
        m_Rig.isKinematic = true;
        PoolManager.Instance.Release(gameObject);
    }
}
