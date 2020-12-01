using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ZCoroutiner.SetCoroutiner(this);
        ZABManager.instance.Init();

        //GameObject prefab = ZABManager.instance.LoadAsset<GameObject>("test/cube", "cube11");
        ZABManager.instance.LoadAssetAsync<GameObject>("test/cube", "cube11", (GameObject go) => 
        {
            Debug.Log("finish");
            Instantiate(go);
        });
        Debug.Log("aaaaaa");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
