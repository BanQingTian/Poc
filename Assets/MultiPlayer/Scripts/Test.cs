using NRKernal;
using System;
using System.Collections;
using System.Collections.Generic;
using TriLib;
using UnityEngine;

public class Test : MonoBehaviour
{
    private UnityEngine.UI.Button LoadBtn;
    // Start is called before the first frame update
    void Start()
    {
        //try
        //{
        //    LoadBtn = GameObject.Find("LoadBtn").GetComponent<UnityEngine.UI.Button>();
        //    LoadBtn.onClick.AddListener(run);
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError(e);
        //}
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void run()
    {
        loadModel("Bouncing.FBX");
    }

    private void loadModel(string modelName)
    {
        var p = PathHelper.GetModelPath(modelName);

        using (var assetLoader = new AssetLoader())
        {
            try
            {
                var assetLoaderOptions = AssetLoaderOptions.CreateInstance();
                assetLoaderOptions.RotationAngles = new Vector3(90f, 180f, 0f);
                assetLoaderOptions.AutoPlayAnimations = true;
                var loadedGameObject = assetLoader.LoadFromFile(p, assetLoaderOptions);
                loadedGameObject.transform.position = new Vector3(0f, 0f, 300f);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

}

public static class PathHelper
{
    public const string DirPath = "/storage/emulated/0/MultiDemo/Models/";
    public static string GetModelPath(string modelName)
    {
        if (!System.IO.Directory.Exists(DirPath))
        {
            System.IO.Directory.CreateDirectory(DirPath);
        }
        return string.Format("{0}{1}", DirPath, modelName);
    }
}





public class Solution
{
    public int[] TwoSum(int[] nums, int target)
    {
        int[] ret = new int[2];
        if (nums.Length == 0)
            return ret;
        for (int i = 0; i < nums.Length; i++)
        {
            for (int j = i+1; j < nums.Length; j++)
            {
                if(nums[i]+nums[j] == target)
                {
                    ret[0] = i;
                    ret[1] = j;

                    return ret;
                }
            }
        }
        return ret;
    }


    private Vector2 _lastTouchPos = Vector2.zero;
    private Vector2 initTouchPos;
    // swipe change weapon
    private void SwipeTouch()
    {
        if (_lastTouchPos != NRInput.GetTouch())
        {
            if (NRInput.GetTouch() != Vector2.zero && _lastTouchPos == Vector2.zero)    //手指触碰touchPad
            {
                initTouchPos = NRInput.GetTouch();
            }
            if (NRInput.GetTouch() == Vector2.zero && _lastTouchPos != Vector2.zero)    //手指离开touchPad
            {
                Vector2 delPos = _lastTouchPos - initTouchPos;
                if (Mathf.Abs(delPos.x) >= Mathf.Abs(delPos.y))     //左右滑动
                {
                    if (Mathf.Abs(delPos.x) > 0.3)
                    {
                        if (delPos.x > 0) // right
                        {

                        }
                        else
                        {

                        }
                    }
                }
                else     //上下滑动
                {
                    if (Mathf.Abs(delPos.y) > 0.5)
                    {
                        if (delPos.y > 0) // up
                        {

                        }
                        else
                        {

                        }
                    }
                }
            }
            _lastTouchPos = NRInput.GetTouch();
        }
    }


    Vector2 lastPos = new Vector2();
    public virtual void MouseMove()
    {
        Vector2 curPos = NRInput.GetTouch();
        if (curPos.x != 0 || curPos.y != 0)
        {
            Vector2 moved = lastPos == Vector2.zero ? Vector2.zero : curPos - lastPos;

            //move.x  左右移动的位置 
            // todo something

            lastPos = curPos;
        }
        else
        {
            lastPos = curPos;
        }
    }

}