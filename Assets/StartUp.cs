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

class Solution2
{
    public class TreeNode
    {
        public TreeNode(int val)
        {

        }
        public int val;
        public TreeNode left;
        public TreeNode right;
    }
    public TreeNode constructFromPrePost(int[] pre, int[] post)
    {
        return helper(pre, post, 0, pre.Length - 1, 0, post.Length - 1);
    }
    public TreeNode helper(int[] pre, int[] post, int prestart, int preend, int poststart, int postend)
    {
        if (prestart > preend || poststart > postend) return null;
        TreeNode root = new TreeNode(pre[prestart]);
        if (prestart == preend)
            return root;
        int index = 0; 
        while (post[index] != pre[prestart + 1])
        {
            index++;
        }
        root.left = helper(pre, post, prestart + 1, prestart + 1 + index - poststart, poststart, index);
        root.right = helper(pre, post, prestart + 2 + index - poststart, preend, index + 1, preend - 1);
        return root;

    }

    public int[] ConstructRectangle(int area)
    {
        int mid = area / 2;
        while(mid*(area-mid) != area)
        {
            mid--;
        }
        return new int[] { mid, area - mid };
    }

}
