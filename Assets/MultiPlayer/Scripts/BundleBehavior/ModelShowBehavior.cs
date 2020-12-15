using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelShowBehavior : BaseBehaviour
{

    protected bool Initialized = false;

    private bool isRotate = false;

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


    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener(ZConstant.Event__Rotate__, rotate);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener(ZConstant.Event__Rotate__, rotate);
    }

    private void Update()
    {
        if (isRotate)
        {
            MGModel.transform.Rotate(Vector3.up, 0.1f);
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

    private void rotate(object sender, EventCenter.Args args)
    {
        isRotate = !isRotate;
    }
}
