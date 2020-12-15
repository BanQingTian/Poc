using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinigameBehavior : MonoBehaviour
{
    private static MinigameBehavior mb;

    public GameObject MGModel = null;
    public Animator MGModelAnim = null;
    public AnimatorStateInfo MGModelAnimStatusInfo;


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

    private void Processing(GameObject go)
    {
        MGModel = go;
        MGModelAnim = MGModel.GetComponent<Animator>();
        MGModelAnimStatusInfo = MGModelAnim.GetCurrentAnimatorStateInfo(0);
    }

    public string GetAnimPlayingName()
    {
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
            return "Idle";
        }
        else if (MGModelAnimStatusInfo.IsName("End"))
        {
            return "End";
        }
        else
        {
            return "Charge";
        }


    }
    public void PlayNextAnim()
    {
        switch (GetAnimPlayingName())
        {
            case "Idle":
                MGModelAnim.SetTrigger("Next");
                break;
            case "End":
                GameManager.Instance.LoadAssetBundle(ZGlobal.CurABStatus++);
                break;
            case "Charge":
                break;
            default:
                break;
        }
        
    }


    // Main


}
