using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using Ping = UnityEngine.Ping;

public static class ZUtils
{
    public static string GetIPAdress(string defaultAdress)
    {
        string path = Application.persistentDataPath + "/MP_IPAddress.txt";
        if (System.IO.File.Exists(path))
        {
            return System.IO.File.ReadAllText(path);
        }
        else
        {
            return defaultAdress;
        }
    }

    public static string LoadDefaultServerConfig()
    {
        TextAsset txt = Resources.Load("Config/defaultServerConfig") as TextAsset;
        string str = txt.text;
        Debug.Log(str);
        return str;
    }
    public static string GetLocalIP()
    {
        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adater in adapters)
        {
            if (adater.Supports(NetworkInterfaceComponent.IPv4))
            {
                UnicastIPAddressInformationCollection UniCast = adater.GetIPProperties().UnicastAddresses;
                if (UniCast.Count > 0)
                {
                    foreach (UnicastIPAddressInformation uni in UniCast)
                    {
                        if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            Debug.Log(uni.Address.ToString());
                            return uni.Address.ToString();
                        }
                    }
                }
            }
        }
        return null;
    }

    public static string Vector2String(Vector3 v)
    {
        return string.Format("{0}#{1}#{2}", v.x, v.y, v.z);
    }
    public static string Quaternion2String(Quaternion q)
    {
        return string.Format("{0}#{1}#{2}#{3}", q.x, q.y, q.z, q.w);
    }

    public static string GetUUID()
    {
        return Guid.NewGuid().ToString();
    }

    public static long GetTimeStamp(bool bflag = true)
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret;
        if (bflag)
            ret = Convert.ToInt64(ts.TotalSeconds);
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    }


    public static Matrix4x4 GetTMatrix(Vector3 position, Quaternion rotation)
    {
        return Matrix4x4.TRS(position, rotation, Vector3.one);
    }

    public static Matrix4x4 GetTMatrix(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        return Matrix4x4.TRS(position, rotation, scale);
    }

    public static Vector3 GetPositionFromTMatrix(Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;

        return position;
    }

    public static Quaternion GetRotationFromTMatrix(Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static void LongPoidoom(string s)
    {
        char[] arr = s.ToCharArray();

        int start = 0;
        string pit;

        for (int i = 0; i < s.Length; i++)
        {

        }

        while (start < s.Length)
        {

        }
    }
}