using NetWorkToolkit;
using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerNetObj PlayerPrefab;

    // 如果30s以后有人成为curator
    public bool CanBecomeCurator = true;

    public bool JoinRoom = false;
    public bool BeginGame = false;

    // 防止多次加载模型
    private bool loading = false;


    private HintData m_HintData;
    private ZMarkerHelper m_MarkerHelper;
    private PlayerMe m_PlayerMe;
    private UIManager m_UIPanel;

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
        S2CFuncTable.Add(S2CFuncName.Rotate, S2C_Rotate);
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

        AnimatornProcessing();
        RecenterCOntroller();
        UILookAtOwner();
    }


    #endregion

    float _hoverTime = 0;
    float _hoverInterval = 1;
    private void RecenterCOntroller()
    {
        if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            _hoverTime = 0;
        }
        if (NRInput.GetButton(ControllerButton.TRIGGER))
        {
            _hoverTime += Time.deltaTime;
            if (_hoverTime >= _hoverInterval)
            {
                NRInput.RecenterController();
                _hoverTime = 0;
            }
        }
        if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
        {
            _hoverTime = 0;
        }
    }

    private void AnimatornProcessing()
    {
        switch (ZGlobal.CurGameStatusMode)
        {
            case ZCurGameStatusMode.WAITING_STATUS:

                break;

            case ZCurGameStatusMode.MINI_GAME_STATUS:
                if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
                {
                    SendPlayNextAnim(0);
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

                if (m_ShowModelBehavoir.GetAnimPlayingName() != "End")
                {
                    if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
                    {
                        SendPlayNextAnim(1);
                    }
                }
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
                                SendPlayShowModels(ZCurAssetBundleStatus.S0106);
                            }
                            break;
                        case ZCurAssetBundleStatus.S0106:

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

    private void UILookAtOwner()
    {
        if (UIHint != null)
        {
            var owner = m_PlayerMe.GetOwnerPlayerNetObj.transform;
            foreach (Transform item in UIHint.transform)
            {
                item.rotation = Quaternion.identity;
                //item.localScale = new Vector3(0, 0, -1);
                //item.LookAt(owner);
            }
        }
    }

    public void Initialized()
    {
        m_HintData = new HintData();
        m_MarkerHelper = ZMarkerHelper.Instance;
        m_PlayerMe = new PlayerMe();
        m_UIPanel = UIManager.Instance;

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

    public int GetPlayerCount()
    {
        return m_PlayerMe.PlayerCount;
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
        if (type == 0)
            m_MinigameBehavior.PlayNextAnim();
        else
            m_ShowModelBehavoir.PlayNextAnim();
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

    public void S2C_Rotate(string param)
    {
        m_ShowModelBehavoir.rotate();
    }

    #endregion



    #region Net Relate

    public void FirstPersonWaiting30s()
    {

    }

    public void SendStartGame()
    {
        MessageManager.Instance.SendStartGame();
    }

    public void SendPlayNextAnim(int type)
    {
        MessageManager.Instance.SendFireMsg(m_PlayerMe.GetOwnerPlayerNetObj.entityInfo.owner, type);
    }

    public void SendPlayMiniGame()
    {
        if (loading)
            return;

        loading = true;

        MessageManager.Instance.SendPlayMiniGame();
    }

    public void SendPlayShowModels(ZCurAssetBundleStatus abs)
    {
        if (loading)
            return;

        loading = true;

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
        Debug.Log("[CZLOG] OnScanSuccess -> ready load waiting ab");
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

    public void LoadAssetBundle_UI()
    {
        ResourceManager.LoadAssetAsync<SpriteAtlas>("lgu/ui", "UICollection", (SpriteAtlas sa) =>
          {
              var vc = FindObjectOfType<VirtualControllerView>();
              Debug.Log("loading UI");
              if (vc != null)
              {
                  vc.Loading(sa);
              }
          });
    }

    private Transform UIHint;
    public void LoadAssetBundle(ZCurAssetBundleStatus abs)
    {
        // 删除上一个bundle的资源和数据
        var a = m_PlayerMe.GetAssetBundleGO(ZGlobal.CurABStatus.ToString());
        if (a != null && a.activeInHierarchy)
        {
            m_PlayerMe.RemoveAssetBundleGO(ZGlobal.CurABStatus.ToString(), true);
        }

        // 加载新的bundle
        string curABS = abs.ToString();
        var abgo = m_PlayerMe.GetAssetBundleGO(curABS);
        if (abgo == null)
        {
            if (abs > ZCurAssetBundleStatus.S0101)
                ShowHint(HintType.Loading, true);
            ResourceManager.LoadAssetAsync<GameObject>(string.Format("{0}/{1}", ZConstant.DefaultDir, curABS.ToLower()), curABS, (GameObject prefab) =>
             {
                 if (abs > ZCurAssetBundleStatus.S0101)
                     ShowHint(HintType.Loading, false);
                 loading = false;
                 ChangeABStatusTip(abs);
                 var go = GameObject.Instantiate(prefab);

                 UIHint = null;

                 if (ZGlobal.CurABStatus <= ZCurAssetBundleStatus.S0102)
                 {
                     go.transform.SetParent(m_MinigameBehavior.transform);
                     m_MinigameBehavior.Processing(go);
                 }
                 else
                 {
                     go.transform.SetParent(m_ShowModelBehavoir.transform);
                     m_ShowModelBehavoir.Processing(go);

                     if (ZGlobal.CurABStatus == ZCurAssetBundleStatus.S0105)
                     {
                         Debug.Log("update ui pose");
                         UIHint = GameObject.Find("Dialog").transform;
                     }

                     if(ZGlobal.CurABStatus == ZCurAssetBundleStatus.S0106)
                     {
                         ZCoroutiner.StartCoroutine(() => 
                         {
                             NRDevice.QuitApp();
                         }, 30);
                     }
                 }

                 //ReLoadShader(go);
                 go.transform.position = Vector3.zero;
                 //go.transform.rotation = Quaternion.identity;
                 //go.transform.localScale = Vector3.one;
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

    private void ReLoadShader(GameObject obj)
    {
        Renderer[] meshSkinRenderer = obj.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < meshSkinRenderer.Length; i++)
        {
            meshSkinRenderer[i].material.shader = Shader.Find(meshSkinRenderer[i].material.shader.name);

            Debug.Log("~~~~~~" + meshSkinRenderer[i].gameObject.name + " = " + meshSkinRenderer[i].material.shader.name);

            if (meshSkinRenderer[i].materials.Length > 1)
            {
                for (int j = 0; j < meshSkinRenderer[i].materials.Length; j++)
                {
                    meshSkinRenderer[i].materials[j].shader = Shader.Find(meshSkinRenderer[i].materials[j].shader.name);
                }
            }
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
        Debug.Log("[CZLOG] Cur ABS = " + abs.ToString());

        ZGlobal.CurABStatus = abs;
    }


#endregion



#region UI 

    public void ShowPlayerCountUI(bool show = true)
    {
        m_UIPanel.PlayerStatusParents.SetActive(show);
    }

    public void RefreshPlayerStatusUI()
    {
        m_UIPanel.SetPlayerStatusUI(m_PlayerMe.PlayerCount);
    }

    public void ShowHint(HintType t, bool show = true)
    {
        m_UIPanel.SetHintLabel(m_HintData.GetData(t), show);
    }

    public void OpenScan()
    {
        ShowHint(HintType.ScanMarker);
        m_MarkerHelper.ResetScanStatus();
        m_MarkerHelper.ScanSuccessEvent = OnScanSuccess;
    }

#endregion


}
