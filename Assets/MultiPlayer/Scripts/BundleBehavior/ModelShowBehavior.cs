using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelShowBehavior : MonoBehaviour
{

    protected bool Initialized = false;

    protected GameObject CurModel = null;
    private bool isRotate = false;

    public void Init()
    {
        if (Initialized)
            return;

        Initialized = true;


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
            CurModel.transform.Rotate(Vector3.up, 0.1f);
        }
    }

    private void rotate(object sender, EventCenter.Args args)
    {
        CurModel = GameObject.CreatePrimitive(PrimitiveType.Cube);

        isRotate = !isRotate;
    }
}
