using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.Networking;

public class AllCerKey : CertificateHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //protected override bool ValidateCertificate(byte[] certificateData)
    //{
    //    X509Certificate2 cer = new X509Certificate2(certificateData);
    //    string pk = cer.GetPublicKeyString();
    //    return true;
    //}
}
