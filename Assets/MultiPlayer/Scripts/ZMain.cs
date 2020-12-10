using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMain : MonoBehaviour
{
    [Space(20)]
    public ZClientMode ClientMode = ZClientMode.Visitor;
    [Space(20)]
    public ZServiceMode m_ServiceMode = ZServiceMode.TEST;

    private static List<string> IPAdress = new List<string>()
    {
        "127.0.0.1",
        "191.168.68.187",
        "",
    };

    void Start()
    {
        ZGlobal.ClientMode = ClientMode;
        Init();
        GameManager.Instance.ShowHint(HintType.ConnectNetwork);
        ConnectServer(m_ServiceMode);

    }

    private void Init()
    {
        DontDestroyOnLoad(this);

        InitGameManager();
        InitMessageManager();
        InitGameCoroutiner();
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
        string ip = IPAdress[(int)sm];
        MessageManager.Instance.SendConnectServerMsg(ip, "2567");
    }
}



public static class Constant
{
    public const string Event__Capture__ = "event_capture";
    public const string Event__MiniGame__ = "event_minigame";
    public const string Event__ModelShow__ = "event_modelshow";
    public const string Event__Rotate__ = "event_rotate";
}