using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintData 
{
    private const string ConnectingNetwork = " ";//"네트워크 연결 대기 중 ...";

    private const string ScanMarker = "마커를 스캔하세요 ...";

    private const string WaitingOthers = "플레이어 접속 대기 중...";

    private const string WaitingCurator = "접속 중입니다...";

    private const string Loading = "로드 중 ...";

    public string GetData(HintType t)
    {
        string ret = ConnectingNetwork;
        switch (t)
        {
            case HintType.ConnectNetwork:
                ret = ConnectingNetwork;
                break;
            case HintType.ScanMarker:
                ret = ScanMarker;
                break;
            case HintType.WaitingCurator:
                ret = WaitingCurator;
                break;
            case HintType.WaitingOthers:
                ret = WaitingOthers;
                break;
            case HintType.Loading:
                ret = Loading;
                break;
            default:
                break;
        }
        return ret;
    }
}

public enum HintType
{
    ConnectNetwork,
    ScanMarker,
    WaitingCurator,
    WaitingOthers,
    Loading,
}
