using NetWorkToolkit;
using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerNetObj PlayerPrefab;

    public bool JoinRoom = false;
    public bool BeginGame = false;

    private HintData m_HintData;
    private ZMarkerHelper m_MarkerHelper;
    private PlayerMe m_PlayerMe;

    private MinigameBehavior m_MinigameBehavior;
    private ModelShowBehavior m_ShowModelBehavoir;


    public delegate void S2CFuncAction<T>(T info);
    public Dictionary<string, S2CFuncAction<string>> S2CFuncTable = new Dictionary<string, S2CFuncAction<string>>();

    #region Unity_Internal


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        S2CFuncTable.Add(S2CFuncName.Fire, S2C_Fire);
        S2CFuncTable.Add(S2CFuncName.PlayMiniGame, S2C_PlayMiniGame);
        S2CFuncTable.Add(S2CFuncName.PlayShowModels, S2C_PlayShowModels);
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.F))
        {
            Time.timeScale = 5;
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            Time.timeScale = 1;
        }

#endif
        switch (ZGlobal.CurGameStatusMode)
        {
            case ZCurGameStatusMode.WAITING_STATUS:

                break;

            case ZCurGameStatusMode.MINI_GAME_STATUS:
                if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
                {
                    SendPlayNextAnim();
                }
                if (ZGlobal.ClientMode == ZClientMode.Curator && m_MinigameBehavior.GetAnimPlayingName() == "End")
                {
                    // 动画播放完
                    if (m_MinigameBehavior.MGModelAnimStatusInfo.normalizedTime >= 1)
                    {
                        Debug.Log("~~~~SendPlayShowModels(ZCurAssetBundleStatus.S0103);");
                        SendPlayShowModels(ZCurAssetBundleStatus.S0103);
                    }
                }

                break;

            case ZCurGameStatusMode.MODELS_SHOW_STATUS:


                if (ZGlobal.ClientMode == ZClientMode.Curator && m_ShowModelBehavoir.GetAnimPlayingName() == "End")
                {
                    switch (ZGlobal.CurABStatus)
                    {
                        case ZCurAssetBundleStatus.S0103:
                            // 动画播放完
                            if (m_ShowModelBehavoir.MGModelAnimStatusInfo.normalizedTime >= 1)
                            {
                                SendPlayShowModels(ZCurAssetBundleStatus.S0104);
                            }
                            break;
                        case ZCurAssetBundleStatus.S0104:
                            if (m_ShowModelBehavoir.MGModelAnimStatusInfo.normalizedTime >= 1)
                            {
                                SendPlayShowModels(ZCurAssetBundleStatus.S0105);
                            }
                            break;
                        case ZCurAssetBundleStatus.S0105:
                            if (m_ShowModelBehavoir.MGModelAnimStatusInfo.normalizedTime >= 1)
                            {
                                //SendPlayShowModels(ZCurAssetBundleStatus.S0106);
                            }
                            break;
                        case ZCurAssetBundleStatus.S0106:

                            Debug.Log("~~~~end!!!!!#*&^#&$&$        ");

                            break;
                        default:
                            Debug.LogError("Cur Game Status Mode != ZCurAssetBundleStatus ^%^$^$%#@@$%");
                            break;
                    }






                }

                break;
            default:
                break;
        }
    }


    #endregion

    public void Initialized()
    {
        m_HintData = new HintData();
        m_MarkerHelper = ZMarkerHelper.Instance;
        m_PlayerMe = new PlayerMe();

        ShowHint(HintType.ConnectNetwork);

        m_MinigameBehavior = FindObjectOfType<MinigameBehavior>();
        m_MinigameBehavior.Init();

        m_ShowModelBehavoir = FindObjectOfType<ModelShowBehavior>();
        m_ShowModelBehavoir.Init();
    }


    #region Data opera

    public void AddPlayerData(string playerid, PlayerNetObj obj)
    {
        m_PlayerMe.AddPlayer(playerid, obj);
        RefreshPlayerStatusUI();
    }
    public void RemovePlayerData(string playerid)
    {
        m_PlayerMe.RemovePlayer(playerid);
        RefreshPlayerStatusUI();
    }
    public void ClearPlayerData()
    {
        m_PlayerMe.ClearPlayerData();
        RefreshPlayerStatusUI();
    }

    #endregion


    #region S2CFunc

    public void S2C_Fire(string param)
    {
        var arr = param.Split(',');
        string pid = arr[0];
        int type = int.Parse(arr[1]);

        Fire(pid, type);
    }
    private void Fire(string pid, int type)
    {
        var obj = m_PlayerMe.GetPlayerNetObj(pid);
        //obj.Shoot();
        m_MinigameBehavior.PlayNextAnim();
    }

    public void S2C_PlayMiniGame(string param)
    {
        ShowHint(HintType.WaitingOthers, false);
        ChangeGameStatuTip(ZCurGameStatusMode.MINI_GAME_STATUS);
        LoadAssetBundle((ZCurAssetBundleStatus)int.Parse(param));
    }
    public void S2C_PlayShowModels(string param)
    {
        ShowHint(HintType.WaitingOthers, false);
        ChangeGameStatuTip(ZCurGameStatusMode.MODELS_SHOW_STATUS);
        LoadAssetBundle((ZCurAssetBundleStatus)int.Parse(param));
    }

    #endregion



    #region Net Relate

    public void SendPlayNextAnim()
    {
        MessageManager.Instance.SendFireMsg(m_PlayerMe.GetOwnerPlayerNetObj.entityInfo.owner, 0);
    }

    public void SendPlayMiniGame()
    {
        MessageManager.Instance.SendPlayMiniGame();
    }

    public void SendPlayShowModels(ZCurAssetBundleStatus abs)
    {
        MessageManager.Instance.SendPlayShowModels(abs);
    }

    public void CreateRoom()
    {
        if (ZGlobal.ClientMode == ZClientMode.Curator)
        {
            MessageManager.Instance.SendCreateRoomMsg();
        }
    }

    public void VisitModeSearchRoom()
    {
        ShowHint(HintType.WaitingCurator);
        ZCoroutiner.StartCoroutine(CorSearchRoom());
    }

    private IEnumerator CorSearchRoom()
    {
        while (!JoinRoom)
        {
            yield return new WaitForSeconds(1f);
            if (JoinRoom)
                yield break;

            Debug.Log("------ Search Room ------");
            if (ZGlobal.ClientMode == ZClientMode.Curator)
            {
                CreateRoom();
                yield break;
            }
            MessageManager.Instance.SendRefreshRoomList();
        }
    }

    // 扫秒成功 开始等待房间创建
    public void OnScanSuccess()
    {

        ShowHint(HintType.WaitingOthers);
        LoadAssetBundle(ZGlobal.CurABStatus);

        if (ZGlobal.ClientMode == ZClientMode.Visitor)
        {

        }
        else
        {

        }
    }

    #endregion

    #region AssetBundle 


    public void LoadAssetBundle(ZCurAssetBundleStatus abs)
    {
        var a = m_PlayerMe.GetAssetBundleGO(ZGlobal.CurABStatus.ToString());
        if (a != null && a.activeInHierarchy)
        {
            m_PlayerMe.RemoveAssetBundleGO(ZGlobal.CurABStatus.ToString(), true);
        }

        string curABS = abs.ToString();

        var abgo = m_PlayerMe.GetAssetBundleGO(curABS);
        if (abgo == null)
        {
            ResourceManager.LoadAssetAsync<GameObject>(string.Format("{0}", curABS.ToLower()), curABS, (GameObject prefab) =>
            {
                ChangeABStatusTip(abs);
                var go = GameObject.Instantiate(prefab);

                if (ZGlobal.CurABStatus <= ZCurAssetBundleStatus.S0102)
                {
                    go.transform.SetParent(m_MinigameBehavior.transform);
                    m_MinigameBehavior.Processing(go);
                }
                else
                {
                    go.transform.SetParent(m_ShowModelBehavoir.transform);
                    m_ShowModelBehavoir.Processing(go);
                }

                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                m_PlayerMe.AddAssetBundleGO(curABS, go);
            });
        }
        else
        {
            abgo.SetActive(true);
            // 重置动画
            abgo.GetComponent<Animator>().Play(0);
        }



    }

    #endregion

    #region Status

    public void ChangeGameStatuTip(ZCurGameStatusMode gs)
    {
        ZGlobal.CurGameStatusMode = gs;
    }

    public void ChangeABStatusTip(ZCurAssetBundleStatus abs)
    {
        Debug.Log("~~~~~ Cur ABS = " + abs.ToString());

        ZGlobal.CurABStatus = abs;
    }

    #endregion


    #region UI 

    public void RefreshPlayerStatusUI()
    {
        UIManager.Instance.SetPlayerStatusUI(m_PlayerMe.PlayerCount);
    }

    public void ShowHint(HintType t, bool show = true)
    {
        UIManager.Instance.SetHintLabel(m_HintData.GetData(t), show);
    }

    public void OpenScan()
    {
        ShowHint(HintType.ScanMarker);
        m_MarkerHelper.ResetScanStatus();
        m_MarkerHelper.ScanSuccessEvent = OnScanSuccess;
    }

    #endregion


}
