﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualControllerView : MonoBehaviour
{

    public Button LogoBtn;
    public Button MiniGameBtn;
    public Button ModelsBtn;
    public Button CaptureBtn;
    public Button ModelRotateBtn;

    #region Unity_Internal
    private void OnEnable()
    {
        LogoBtn.onClick.AddListener(LogoBtnClk);
        MiniGameBtn.onClick.AddListener(MiniBtnClk);
        ModelsBtn.onClick.AddListener(ModelsBtnClk);
        CaptureBtn.onClick.AddListener(CaptureBtnClk);
        ModelRotateBtn.onClick.AddListener(ModelRotate);
    }
    private void Start()
    {
#if UNITY_EDITOR
        GetComponentInChildren<CanvasScaler>().enabled = false;
#endif
    }

    private void Update()
    {
        pentaKill();
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.B))
        {
            clkcount = 5;
        }
#endif
    }

    private void OnDisable()
    {
        LogoBtn.onClick.RemoveListener(LogoBtnClk);
        MiniGameBtn.onClick.RemoveListener(MiniBtnClk);
        ModelsBtn.onClick.RemoveListener(ModelsBtnClk);
        CaptureBtn.onClick.RemoveListener(CaptureBtnClk);
        ModelRotateBtn.onClick.RemoveListener(ModelRotate);
    }
    #endregion

    int clkcount = 0;
    float timer = 0;
    float penta_interval = 0.7f;
    private void pentaKill()
    {
        if (clkcount >= 5)
        {
            //ZGlobal.ClientMode = ZGlobal.ClientMode == ZClientMode.Curator ? ZClientMode.Visitor : ZClientMode.Curator;
            ZGlobal.ClientMode = ZClientMode.Curator;
            SetMode(ZGlobal.ClientMode);
            clkcount = 0;
            timer = 0;
            return;
        }

        if (timer < penta_interval)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            clkcount = 0;
        }
    }
    public void SetMode(ZClientMode mode)
    {
        Debug.Log("cur mode : " + mode);
        switch (mode)
        {
            case ZClientMode.Curator:

                MiniGameBtn.gameObject.SetActive(true);
                ModelsBtn.gameObject.SetActive(true);
                CaptureBtn.gameObject.SetActive(true);
                ModelRotateBtn.gameObject.SetActive(true);

                break;


            case ZClientMode.Visitor:

                MiniGameBtn.gameObject.SetActive(false);
                ModelsBtn.gameObject.SetActive(false);
                CaptureBtn.gameObject.SetActive(true);
                ModelRotateBtn.gameObject.SetActive(false);

                break;


            default:
                break;
        }
    }

    private void LogoBtnClk()
    {
        clkcount++;
    }
    private void MiniBtnClk()
    {
        GameManager.Instance.ShowHint(HintType.WaitingOthers, false);

        if (ZGlobal.CurGameStatusMode == ZCurGameStatusMode.MINI_GAME_STATUS
            || ZGlobal.CurGameStatusMode == ZCurGameStatusMode.MODELS_SHOW_STATUS)
        {
            return;
        }

        GameManager.Instance.ChangeGameStatuTip(ZCurGameStatusMode.MINI_GAME_STATUS);
        GameManager.Instance.SendPlayMiniGame();
    }
    private void ModelsBtnClk()
    {
        GameManager.Instance.ShowHint(HintType.WaitingOthers, false);


        if (ZGlobal.CurGameStatusMode == ZCurGameStatusMode.MODELS_SHOW_STATUS)
        {
            return;
        }

        GameManager.Instance.ChangeGameStatuTip(ZCurGameStatusMode.MODELS_SHOW_STATUS);
        GameManager.Instance.SendPlayShowModels(ZCurAssetBundleStatus.S0103);
    }
    private void CaptureBtnClk()
    {
        EventCenter.Instance.DispatchEvent(ZConstant.Event__Capture__);
    }
    private void ModelRotate()
    {
        EventCenter.Instance.DispatchEvent(ZConstant.Event__Rotate__);
    }


}
