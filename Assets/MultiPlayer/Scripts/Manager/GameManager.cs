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


    public delegate void S2CFuncAction<T>(T info);
    public Dictionary<string, S2CFuncAction<string>> S2CFuncTable = new Dictionary<string, S2CFuncAction<string>>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        S2CFuncTable.Add(S2CFuncName.Fire, S2C_Fire);
    }

    public void Initialized()
    {
        m_HintData = new HintData();
        m_MarkerHelper = ZMarkerHelper.Instance;
        m_PlayerMe = new PlayerMe();

        //m_MinigameBehavior = FindObjectOfType<MinigameBehavior>();
        //m_MinigameBehavior.Init();
    }


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
        obj.Shoot();
    }

    #endregion


    #region Data opera

    public void AddPlayerData(string playerid , PlayerNetObj obj)
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

    #region Net Logic

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

        if (ZGlobal.ClientMode == ZClientMode.Visitor)
        {

        }
        else
        {

        }
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
