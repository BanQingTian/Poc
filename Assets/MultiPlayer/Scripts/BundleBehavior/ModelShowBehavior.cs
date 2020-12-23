using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelShowBehavior : BaseBehaviour
{

    protected bool Initialized = false;

    private bool isRotate = false;

    private const float waitTime = 3f; // 动画间隔时间
    private float time = 0;


    public void Init()
    {
        if (Initialized)
            return;





        Initialized = true;
    }

    public void Processing(GameObject go)
    {
        MGModel = go;
        MGModelAnim = go.GetComponent<Animator>();
        MGModelAnimStatusInfo = MGModelAnim.GetCurrentAnimatorStateInfo(0);
    }

    private void Update()
    {
        if (isRotate)
        {
            if (MGModel != null)
                MGModel.transform.Rotate(Vector3.up, 0.2f);
        }

        if (time < waitTime)
        {
            time += Time.deltaTime;
        }
    }

    float _freshTime;
    public string GetAnimPlayingName()
    {
        _freshTime += Time.deltaTime;
        if (_freshTime > 1)
        {
            if (MGModelAnim != null)
            {
                MGModelAnimStatusInfo = MGModelAnim.GetCurrentAnimatorStateInfo(0);
            }
            _freshTime = 0;
        }

        if (MGModelAnimStatusInfo.IsName("End"))
        {
            return "End";
        }
        return "";
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

    public void rotate()
    {
        isRotate = !isRotate;
    }
}
