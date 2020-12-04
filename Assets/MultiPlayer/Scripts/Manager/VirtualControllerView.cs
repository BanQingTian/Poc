using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualControllerView : MonoBehaviour
{
    public GameObject MenuBtns_visitor;
    public GameObject MenuBtns_curator;

    public Button MiniGameBtn;
    public Button ModelsBtn;
    public Button CaptureBtn;
    public Button ModelRotateBtn;


    public void AddListen()
    {
        //MiniGameBtn.onClick.AddListener();
        //ModelsBtn.onClick.AddListener();
        //CaptureBtn.onClick.AddListener();
        //ModelRotateBtn.onClick.AddListener();
    }

    public void SetMode(ZClientMode mode)
    {
        switch (mode)
        {
            case ZClientMode.Curator:

                MiniGameBtn.gameObject.SetActive(true);
                ModelsBtn.gameObject.SetActive(true);
                CaptureBtn.gameObject.SetActive(true);
                ModelRotateBtn.gameObject.SetActive(true);

                break;


            case ZClientMode.Visitor:

                MiniGameBtn.gameObject.SetActive(false);
                ModelsBtn.gameObject.SetActive(false);
                CaptureBtn.gameObject.SetActive(true);
                ModelRotateBtn.gameObject.SetActive(false);

                break;


            default:
                break;
        }
    }
}
