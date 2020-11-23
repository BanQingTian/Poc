using NetWorkToolkit;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyNetObjectExample : NetObjectEntity
{
    public Text nameTip;
    public Text healthyTip;

    public Enemy enemyInfo
    {
        get
        {
            return (Enemy)entityInfo;
        }
    }

    public override void Init(Entity info)
    {
        base.Init(info);
        Debug.Log("Init the enemy :" + enemyInfo.id + "healthy:" + enemyInfo.healthy);
        nameTip.text = enemyInfo.id + " owner:" + enemyInfo.owner;
        healthyTip.text = enemyInfo.healthy.ToString();
    }

    public override Entity CreateEntity()
    {
        return ModelsUtil.EnemyIdentity();
    }

    public override void SerializeData()
    {
        base.SerializeData();
        // Do your serialize
        healthyTip.text = enemyInfo.healthy.ToString();
    }

    public override void DeSerializeData()
    {
        base.DeSerializeData();
        // Do your serialize
        healthyTip.text = enemyInfo.healthy.ToString();
    }

    public void Damage(float value)
    {
        Debug.Log("damage:" + value);
        enemyInfo.healthy -= value;
    }
}