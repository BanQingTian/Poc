using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintData 
{
    private const string ConnectingNetwork = "Connecting to the network...";

    private const string ScanMarker = "Please scan the marker on the table.";

    private const string WaitingOthers = "Waiting for other people.";

    private const string Welcome = "Welcome to holeman's world!";

    private const string Course1 = "Touch the smartphone controller button \n many times and give me the Power.";

    private const string WaitingCurator = "Waiting for the curator.";

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
    WaitingCurator,
    WaitingOthers,
    Welcome,
    Course1
}
