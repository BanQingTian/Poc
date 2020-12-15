using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinigameBehavior : BaseBehaviour
{
    private static MinigameBehavior mb;

    private const float waitTime = 3.5f; // 动画间隔时间
    private float time = 0;
    private void Update()
    {
        if(time < waitTime)
        {
            time += Time.deltaTime;
        }
    }

    public void Init()
    {
        StartCoroutine(InitCor());
    }
    public IEnumerator InitCor()
    {
        yield return ResourceManager.Instance.Initialize();
        //InitResource<GameObject>(Processing);
    }

    public void InitResource<T>(Action<T> Finish) where T : UnityEngine.Object
    {
        // load assetbundle
        //ResourceManager.LoadAssetAsync<GameObject>("LGU_Models", "m1", (GameObject prefab) =>
        //{
        //    var go = GameObject.Instantiate(prefab);
        //    Finish.Invoke(MGModel as T);
        //});

        Finish.Invoke(MGModel as T);
    }

    public void Processing(GameObject go)
    {
        MGModel = go;
        MGModelAnim = MGModel.GetComponent<Animator>();
        MGModelAnimStatusInfo = MGModelAnim.GetCurrentAnimatorStateInfo(0);
    }

    float _freshTime = 0;
    public string GetAnimPlayingName()
    {
        _freshTime += Time.deltaTime;
        if(_freshTime > 1)
        {
            if (MGModelAnim != null)
            {
                MGModelAnimStatusInfo = MGModelAnim.GetCurrentAnimatorStateInfo(0);
            }
            _freshTime = 0;
        }

        if ((
              MGModelAnimStatusInfo.IsName("Idle_01")
           || MGModelAnimStatusInfo.IsName("Idle_02")
           || MGModelAnimStatusInfo.IsName("Idle_03")
           || MGModelAnimStatusInfo.IsName("Idle_04")
           || MGModelAnimStatusInfo.IsName("Idle_05")
           || MGModelAnimStatusInfo.IsName("Idle_06")
           || MGModelAnimStatusInfo.IsName("Idle_07")
           ))
        {
            Debug.Log("~~~~Idle");

            return "Idle";
        }
        else if (MGModelAnimStatusInfo.IsName("End"))
        {
            Debug.Log("~~~~end");
            return "End";
        }
        else
        {
            Debug.Log("~~~~Charge");
            return "Charge";
        }


    }
    public void PlayNextAnim()
    {
        if (time < waitTime) return;
        switch (GetAnimPlayingName())
        {
            case "Idle":
                MGModelAnim.SetTrigger("Next");
                time = 0;
                break;
            case "End":
                //GameManager.Instance.LoadAssetBundle(ZGlobal.CurABStatus++);
                break;
            case "Charge":
                break;
            default:
                break;
        }
        
    }


    // Main


}
