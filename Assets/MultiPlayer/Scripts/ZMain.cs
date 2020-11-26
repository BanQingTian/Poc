using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMain : MonoBehaviour
{
    [Space(20)]
    public ZClientMode ClientMode = ZClientMode.Visiter;
    [Space(20)]
    public ZServiceMode m_ServiceMode = ZServiceMode.TEST;

    private static List<string> IPAdress = new List<string>()
    {
        "127.0.0.1",
        "191.168.69.112",
        "",
    };

    IEnumerator Start()
    {
        ZGlobal.ClientMode = ClientMode;
        yield return null;
        Init();
        GameManager.Instance.ShowHint(HintType.ConnectNetwork);
        ConnectServer(m_ServiceMode);

    }

    private void Init()
    {
        InitGameManager();
        InitMessageManager();
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
        string ip = IPAdress[(int)sm];
        MessageManager.Instance.SendConnectServerMsg(ip, "2567");
    }
}
