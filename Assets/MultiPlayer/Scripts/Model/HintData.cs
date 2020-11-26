using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintData 
{
    private string ConnectingNetwork = "Connecting to the network...";

    private string ScanMarker = "Please scan the marker on the table";

    private string WaitingOthers = "Waiting for other people";

    private string Welcome = "Welcome to holeman's world!";

    private string Course1 = "Touch the smartphone controller button \n many times and give me the Power";

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
            case HintType.WaitingOthers:
                ret = WaitingOthers;
                break;
            case HintType.Welcome:
                ret = Welcome;
                break;
            case HintType.Course1:
                ret = Course1;
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
    WaitingOthers,
    Welcome,
    Course1
}
