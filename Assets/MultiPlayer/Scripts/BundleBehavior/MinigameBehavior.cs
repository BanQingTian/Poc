using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameBehavior : MonoBehaviour
{
    private static MinigameBehavior mb;
    public List<GameObject> ChargePoint = new List<GameObject>();



    void Start()
    {

    }

    void Update()
    {
        
    }

    public void Init()
    {
        InitResource();
    }

    public void InitResource()
    {
        StartCoroutine(loadMinigameScene());

    }
    private IEnumerator loadMinigameScene()
    {
        yield return ResourceManager.Instance.Initialize();

        Debug.Log("loading");
        ResourceManager.LoadLevelAsync("test/cube", "LGS", true, () =>
        {
            Debug.Log("Finish");
            // init pose and 
            
        });
    }
}
