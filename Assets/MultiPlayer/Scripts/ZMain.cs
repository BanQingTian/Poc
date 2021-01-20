using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

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
        //StartCoroutine(AskPermission(Permission.Camera));
        StartCoroutine(WaitingLoadRes());
    }


    // 等待资源加载
    private IEnumerator WaitingLoadRes()
    {

        ZGlobal.ClientMode = ClientMode;
        ZGlobal.ServiceMode = m_ServiceMode;

        Init();

        yield return new WaitForSeconds(1);

        GameManager.Instance.LoadAssetBundle_Weapon_Bullet();

        while (GameManager.Instance.finish < 2)  // 加载完新加的资源
        {
            yield return null;
        }


        ConnectServer(m_ServiceMode);
    }

    private IEnumerator AskPermission(string permission)
    {
        while (true)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
            else
            {
                yield break;
            }
            yield return new WaitForSeconds(3);
        }
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
