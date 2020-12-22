using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMain : MonoBehaviour
{
    [Space(20)]
    public ZClientMode ClientMode = ZClientMode.Visitor;
    [Space(20)]
    public ZServiceMode m_ServiceMode = ZServiceMode.LOCAL_HTTP;

    private static Dictionary<ZServiceMode, string> IPAddressDict = new Dictionary<ZServiceMode, string>()
    {
        {ZServiceMode.LOCAL_HTTP,"192.168.68.187" },
        {ZServiceMode.LOCAL_HTTPS,"multiplay.nreal.ai" },
        {ZServiceMode.CLOUD,"" }
    };

    void Start()
    {
        ZGlobal.ClientMode = ClientMode;
        ZGlobal.ServiceMode = m_ServiceMode;

        Init();
        ConnectServer(m_ServiceMode);

    }

    private void Init()
    {
        DontDestroyOnLoad(this);

        InitNRInputModule();
        InitGameManager();
        InitMessageManager();
        InitGameCoroutiner();
    }
    private void InitNRInputModule()
    {
        NRInput.ControllerVisualActive = false;
        NRInput.LaserVisualActive = false;
        NRInput.ReticleVisualActive = false;
        NRVirtualDisplayer.RunInBackground = true;
    }
    private void InitGameCoroutiner()
    {
        ZCoroutiner.SetCoroutiner(this);
    }
    private void InitGameManager()
    {
        GameManager.Instance.Initialized();
    }
    private void InitMessageManager()
    {
        MessageManager.Instance.InitializeMessage(() => { Debug.Log("Zlog ------- Connect Success !!!"); });
    }
    public void ConnectServer(ZServiceMode sm)
    {
        string ip = IPAddressDict[sm];
        ip = ZUtils.GetIPAdress(ip);
        MessageManager.Instance.SendConnectServerMsg(ip, "443");
    }
}
