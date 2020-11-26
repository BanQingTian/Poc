using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody m_Rig;
    public bool Invalid = false;
    public string BelongToPlayerID;


    private void OnEnable()
    {
        Invoke("ReleaseObj", 1);
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
