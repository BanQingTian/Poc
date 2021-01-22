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
    [HideInInspector] public Transform WeapoonPrefab;
    [HideInInspector] public Transform BulletPrefab;


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
        S2CFuncTable.Add(S2CFuncName.Countdown, S2C_CurGameCountdownTime);
        S2CFuncTable.Add(S2CFuncName.BeginScanMaker, S2C_BeginScanMarker);
        S2CFuncTable.Add(S2CFuncName.ScanMaker, S2C_ScanMarkerFinish);
        S2CFuncTable.Add(S2CFuncName.PlayNextAnim, S2C_PlayNectAnim);
        S2CFuncTable.Add(S2CFuncName.ChargeOnce, S2C_ChargeOnce);
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

        AnimatorProcessing();
        RecenterCOntroller();
        //DoubleClkRecenter();
        //FirstPersonWaiting30s();
    }
    private void LateUpdate()
    {
        UILookAtOwner();
    }

    #endregion



    bool _clkReady = false;
    float ttt = 0;
    private void DoubleClkRecenter()
    {
        if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
        {
            _clkReady = true;
            ttt = 0;
        }

        if (ttt < 0.8f)
            ttt += Time.fixedDeltaTime;

        if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            if (_clkReady && ttt < 0.7f)
            {
                Debug.Log("run");

                NRInput.RecenterController();
            }
            else
            {
                _clkReady = false;
                ttt = 0;
            }
        }
    }

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
                Debug.Log("RecenterController");
                _hoverTime = 0;
            }
        }
        if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
        {
            _hoverTime = 0;
        }
    }

    private void AnimatorProcessing()
    {
        switch (ZGlobal.CurGameStatusMode)
        {
            case ZCurGameStatusMode.WAITING_STATUS:

                break;

            case ZCurGameStatusMode.MINI_GAME_STATUS:
                if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
                {

#if UNITY_EDITOR
                    Debug.Log("Phone Shake!!!!");
#else
                    Handheld.Vibrate();
#endif

                    SendShootFire(0);
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
                        SendShootFire(1);
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

    Vector3 tmpOwner1;
    Vector3 tmpOwner2;
    private void UILookAtOwner()
    {
        if (UIHint != null)
        {
            tmpOwner1 = m_PlayerMe.GetOwnerPlayerNetObj.transform.position;

            UIHint.LookAt(tmpOwner1);
        }
        if (UICanvas != null)
        {
            tmpOwner2 = m_PlayerMe.GetOwnerPlayerNetObj.transform.position;

            //foreach (Transform item in UICanvas.transform)
            //{
            //    item.rotation = Quaternion.identity;
            //}
            UICanvas.LookAt(tmpOwner2);
        }
    }

    private void ResetLookAt()
    {
        if (UIHint != null)
        {
            foreach (Transform item in UIHint.transform)
            {
                item.localRotation = Quaternion.identity;
            }

            var sss = UIHint.transform.localScale;
            UIHint.transform.localScale = new Vector3(-sss.x, sss.y, sss.z);
        }
        if(UICanvas != null)
        {
            var ssss = UICanvas.transform.localScale;
            UICanvas.transform.localScale = new Vector3(-ssss.x, ssss.y, ssss.z);
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

    public bool GetAllPlayerStart()
    {
        return m_PlayerMe.GetAllPlayerReadyStatus();
    }

    public void WaitAllScanReady()
    {
        StartCoroutine(WaitAllScanReadyCor());
    }

    private IEnumerator WaitAllScanReadyCor()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (m_PlayerMe.GetAllPlayerReadyStatus())
            {
                SendStartGame();

                yield return new WaitForSeconds(ZConstant.AllScanReadyWaitTime);

                SendPlayMiniGame();

                yield break;
            }
        }
    }

    public void PentaClkRun()
    {
        CloseCountdown();
        SendStartScanMarker();
    }

    private void CloseCountdown()
    {
        StopCoroutine("BeginCountdownCor");
    }

    // 创建房间后开始倒计时
    public void BeginCountdown()
    {
        StartCoroutine("BeginCountdownCor");
    }

    private IEnumerator BeginCountdownCor()
    {
        int GameCountdown = ZConstant.WaitToTurnOffRoomPerissionTime;
        SendCurGameCountdownTime(GameCountdown);
        while (true)
        {
            yield return new WaitForSeconds(1);

            GameCountdown -= 1;

            SendCurGameCountdownTime(GameCountdown);

            if (GameCountdown <= 0)
            {
                SendStartScanMarker();
                WaitAllScanReady();
                yield break;
            }

            yield return null;
        }
    }





    #region Data opera

    public PlayerNetObj GetSelfPlayerData()
    {
        return m_PlayerMe.GetOwnerPlayerNetObj;
    }
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


    #region S2CFunc 服务器转发消息

    public void S2C_CurGameCountdownTime(string param)
    {
        // update time ui
        Debug.Log("cur time param : " + param);

        SetCountdown(param, true);
    }

    bool openscanYet = false;
    public void S2C_BeginScanMarker(string param)
    {
        if (!openscanYet)
        {
            openscanYet = true;
            OpenScan();
        }

        // hide countdown ui
        SetCountdown("", false);
    }

    public void S2C_ScanMarkerFinish(string param)
    {
        m_PlayerMe.SetPlayerReadyStatus(param);
    }

    public void S2C_Fire(string param)
    {
        var arr = param.Split(',');
        string pid = arr[0];
        int type = int.Parse(arr[1]);

        if (ZGlobal.CurABStatus == ZCurAssetBundleStatus.S0102)
            Fire(pid, type);
    }

    public int curType = 0;
    private void Fire(string pid, int type)
    {
        var obj = m_PlayerMe.GetPlayerNetObj(pid);

        curType = type;

        obj.Shoot(pid);

        //if (type == 0)
        //    m_MinigameBehavior.PlayNextAnim();
        //else
        //    m_ShowModelBehavoir.PlayNextAnim();
    }

    public void S2C_PlayNectAnim(string param)
    {
        m_MinigameBehavior.PlayNextAnimResponce();
    }

    public void S2C_ChargeOnce(string param)
    {
        if (m_PlayerMe.GetOwnerPlayerNetObj.IsRoomOwner())
        {
            m_MinigameBehavior.ChargeCount++;
            Debug.Log("Cur Charge Count : " + m_MinigameBehavior.ChargeCount);

            if (m_MinigameBehavior.ChargeCount >= m_MinigameBehavior.ChargeRate * GetPlayerCount())
            {
                SendPlayNextAnim();
            }
        }
    }

    public void ShootTarget()
    {
        if (curType == 0)
            m_MinigameBehavior.ShootedTarget();
        else
            m_ShowModelBehavoir.PlayNextAnim();
    }

    public void S2C_PlayMiniGame(string param)
    {
        ShowHint(HintType.WaitingOthers, false);
        ShowPlayerCountUI(false);
        ChangeGameStatuTip(ZCurGameStatusMode.MINI_GAME_STATUS);
        LoadAssetBundle((ZCurAssetBundleStatus)int.Parse(param));
    }
    public void S2C_PlayShowModels(string param)
    {
        ShowHint(HintType.WaitingOthers, false);
        ShowPlayerCountUI(false);
        ChangeGameStatuTip(ZCurGameStatusMode.MODELS_SHOW_STATUS);
        LoadAssetBundle((ZCurAssetBundleStatus)int.Parse(param));
    }

    public void S2C_Rotate(string param)
    {
        m_ShowModelBehavoir.rotate();
    }

    #endregion



    #region Net Relate

    public void SendCreateRoom()
    {
        MessageManager.Instance.SendCreateRoomMsg();
    }

    public void SendCurGameCountdownTime(int curTime)
    {
        MessageManager.Instance.SendCurGameCountdownTime(curTime);
    }


    public void SendStartScanMarker()
    {
        MessageManager.Instance.SendStartScanMarker();
    }

    public void SendStartGame()
    {
        MessageManager.Instance.SendStartGame();
    }

    public void SendScanMakerFinish(string playerid)
    {
        MessageManager.Instance.SendScanMakerFinish(playerid);
    }

    public void SendPlayNextAnim()
    {
        MessageManager.Instance.SendPlayNextAnim();
    }

    public void SendShootFire(int type)
    {
        MessageManager.Instance.SendFireMsg(m_PlayerMe.GetOwnerPlayerNetObj.entityInfo.owner, type);
    }

    public void SendPlayAddChargeCount()
    {
        MessageManager.Instance.SendAddChargeCount();
    }

    public void SendPlayMiniGame()
    {
        if (loading)
            return;

        if (ZGlobal.CurGameStatusMode == ZCurGameStatusMode.MINI_GAME_STATUS
           || ZGlobal.CurGameStatusMode == ZCurGameStatusMode.MODELS_SHOW_STATUS)
        {
            return;
        }

        loading = true;

        ShowHint(HintType.WaitingOthers, false);
        ChangeGameStatuTip(ZCurGameStatusMode.MINI_GAME_STATUS);

        MessageManager.Instance.SendPlayMiniGame();
    }

    public void SendPlayShowModels()
    {
        if (loading)
            return;

        if (ZGlobal.CurGameStatusMode == ZCurGameStatusMode.MODELS_SHOW_STATUS)
        {
            return;
        }

        loading = true;

        ShowHint(HintType.WaitingOthers, false);
        ShowPlayerCountUI(false);
        ChangeGameStatuTip(ZCurGameStatusMode.MODELS_SHOW_STATUS);

        MessageManager.Instance.SendPlayShowModels(ZCurAssetBundleStatus.S0103);
    }
    public void SendPlayShowModels(ZCurAssetBundleStatus abs)
    {
        if (loading)
            return;

        loading = true;

        ShowHint(HintType.WaitingOthers, false);
        ShowPlayerCountUI(false);

        MessageManager.Instance.SendPlayShowModels(abs);
    }

    public void VisitModeSearchRoom()
    {
        ShowHint(HintType.WaitingCurator);
        MessageManager.Instance.SendRefreshRoomList();
    }

    // 扫秒成功 开始等待房间创建
    public void OnScanSuccess()
    {
        ShowHint(HintType.WaitingOthers);
        Debug.Log("[CZLOG] OnScanSuccess -> ready load waiting ab");
        LoadAssetBundle(ZGlobal.CurABStatus);
        SendScanMakerFinish(m_PlayerMe.GetOwnerPlayerNetObj.entityInfo.owner);
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

    public int finish = 0;
    public void LoadAssetBundle_Weapon_Bullet()
    {
        ResourceManager.LoadAssetAsync<GameObject>("lgu/weapon", "ChargerUser", (GameObject go) =>
        {
            Debug.Log("load weapon");
            WeapoonPrefab = GameObject.Instantiate(go).transform;
            WeapoonPrefab.GetComponent<Collider>().enabled = false;
            WeapoonPrefab.name = "WeaponPrefab";
            WeapoonPrefab.gameObject.SetActive(false);



            //PlayerPrefab.WeaponTarget = Instantiate(go);
            //PlayerPrefab.WeaponTarget.GetComponent<Collider>().enabled = false;
            //PlayerPrefab.WeaponTarget.SetActive(false);
            //PlayerPrefab.WeaponTarget.transform.localPosition = Vector3.zero;
            ////PlayerPrefab.ShootPoint.transform.SetParent(PlayerPrefab.WeaponTarget.transform);
            ////PlayerPrefab.ShootPoint.transform.localPosition = new Vector3(0, 0, 0.12f);
            finish++;
        });

        ResourceManager.LoadAssetAsync<GameObject>("lgu/bullet", "ChargeBullet", (GameObject go) =>
        {
            Debug.Log("load bullet");
            //BulletPrefab = GameObject.Instantiate(go).transform;
            //BulletPrefab.gameObject.SetActive(false);

            PlayerPrefab.Bullet = Instantiate(go);
            PlayerPrefab.Bullet.SetActive(false);
            finish++;
        });
    }

    private Transform UIHint;
    private Transform UICanvas;
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
            {
                ShowHint(HintType.Loading, true);
            }
            ResourceManager.LoadAssetAsync<GameObject>(string.Format("{0}/{1}", ZConstant.DefaultDir, curABS.ToLower()), curABS, (GameObject prefab) =>
             {
                 if (abs > ZCurAssetBundleStatus.S0101)
                 {
                     ShowHint(HintType.Loading, false);
                 }
                 loading = false;
                 ChangeABStatusTip(abs);
                 var go = GameObject.Instantiate(prefab);

                 UIHint = null;

                 if (ZGlobal.CurABStatus <= ZCurAssetBundleStatus.S0102)
                 {
                     go.transform.SetParent(m_MinigameBehavior.transform);
                     m_MinigameBehavior.Processing(go);

                     if (ZGlobal.CurABStatus == ZCurAssetBundleStatus.S0101)
                     {
                         var obj = GameObject.Find("UI_Canvas"); // 这一块使用自己的UI，隐藏模型中带的UI
                         if (obj != null)
                         {
                             obj.SetActive(false);
                         }
                     }
                     else if (ZGlobal.CurABStatus == ZCurAssetBundleStatus.S0102)
                     {
                         m_PlayerMe.SetAllPlayerWeaponStatus(true);

                         FindUIFollower();
                     }
                 }
                 else
                 {
                     m_PlayerMe.SetAllPlayerWeaponStatus(false);


                     go.transform.SetParent(m_ShowModelBehavoir.transform);
                     m_ShowModelBehavoir.Processing(go);

                     if (ZGlobal.CurABStatus == ZCurAssetBundleStatus.S0106)
                     {
                         ZCoroutiner.StartCoroutine(() =>
                         {
                             NRDevice.QuitApp();
                         }, 30);
                     }

                     FindUIFollower();
                 }

                 ResetLookAt();

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

    private void FindUIFollower()
    {
        var u1 = GameObject.Find("Dialog");
        UIHint = u1 == null ? null : u1.transform;
        var u2 = GameObject.Find("UI_Canvas");
        UICanvas = u2 == null ? null : u2.transform;
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

    public void SetCountdown(string content, bool show)
    {
        m_UIPanel.SetCountdown(content, show);
    }

    public void OpenScan()
    {
        ShowHint(HintType.ScanMarker);
        m_MarkerHelper.ResetScanStatus();
        m_MarkerHelper.ScanSuccessEvent = OnScanSuccess;
    }

    #endregion


}
