using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUp : MonoBehaviour
{
    public Button LoadBtn;
    public Button LoadBtn2;
    public Button LoadBtn3;

    private Shader shader;

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
        LoadBtn2.onClick.AddListener(LoadScene2);
        LoadBtn3.onClick.AddListener(LoadScene3);


    }

    private void Update()
    {

    }

    public void LoadScene3()
    {
        ResourceManager.LoadAssetAsync("lgu/shader", "allshader", (ShaderVariantCollection prefab) =>
        {
            prefab.WarmUp();
            Debug.Log("warmup success");
        });

    }

    public void LoadScene2()
    {

        ResourceManager.LoadAssetAsync("lgu/shader", "MatCap_TextureAdd", (Shader prefab) =>
        {
            shader = prefab;
            Debug.Log(shader.name);
        });
    }

    public void LoadScene()
    {
        Debug.Log("loading");
        //ResourceManager.LoadLevelAsync("test/cube", "LGS", true, () =>
        //{
        //    Debug.Log("Finish");
        //    LoadTip.text = "Finish!!";
        //});


        ResourceManager.LoadAssetAsync("lgu/s0101", "s0101", (GameObject prefab) =>
          {
              var go = GameObject.Instantiate(prefab);
              go.transform.SetParent(transform);
              go.transform.localScale = Vector3.one;
              go.transform.localPosition = Vector3.zero;
              go.transform.localRotation = Quaternion.identity;

              //ReLoadShader(go);
          });
    }

    private void ReLoadShader(GameObject obj)
    {
        Renderer[] meshSkinRenderer = obj.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < meshSkinRenderer.Length; i++)
        {
            meshSkinRenderer[i].material.shader = Shader.Find(meshSkinRenderer[i].material.shader.name);

            Debug.Log("~~~~~~" + meshSkinRenderer[i].gameObject.name + " = " + meshSkinRenderer[i].material.shader.name);

            //if (meshSkinRenderer[i].materials.Length > 1)
            //{
            //    for (int j = 0; j < meshSkinRenderer[i].materials.Length; j++)
            //    {
            //        meshSkinRenderer[i].materials[j].shader = Shader.Find(meshSkinRenderer[i].materials[j].shader.name);
            //    }
            //}
        }
    }

}
