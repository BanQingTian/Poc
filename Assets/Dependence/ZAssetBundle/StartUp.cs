using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUp : MonoBehaviour
{
    public Button LoadBtn;
    public Text LoadTip;


    IEnumerator Start()
    {
        //Debug.Log(Application.persistentDataPath);

        //ZCoroutiner.SetCoroutiner(this);
        //ZABManager.instance.Init();

        ////GameObject prefab = ZABManager.instance.LoadAsset<GameObject>("test/cube", "cube11");
        //ZABManager.instance.LoadAssetAsync<GameObject>("test/cube", "cube11", (GameObject go) => 
        //{
        //    Debug.Log("finish");
        //    Instantiate(go);
        //});
        //Debug.Log("aaaaaa");

        yield return ResourceManager.Instance.Initialize();

        LoadBtn.onClick.AddListener(LoadScene);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            var go = GameObject.Find("Reflection Probe");
            if (go != null)
            {
                Debug.Log(go.name);
            }
        }
    }

    public void LoadScene()
    {
        Debug.Log("loading");
        LoadTip.text = "Loading...";
        ResourceManager.LoadLevelAsync("test/cube", "LGS", true, () => 
        {
            Debug.Log("Finish");
            LoadTip.text = "Finish!!";
        });
    }

}
