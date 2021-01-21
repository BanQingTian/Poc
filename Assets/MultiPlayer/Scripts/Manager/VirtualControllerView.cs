using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualControllerView : MonoBehaviour
{
    public NRButton TriggerBtn;
    public Button LogoBtn;
    public Button MiniGameBtn;
    public Button ModelsBtn;
    public Button CaptureBtn;
    public Button ModelRotateBtn;
    public Button FirstBtn;
    public Button SecondBtn;
    public Button ThirdBtn;

    public Image BgImage;
    public Sprite CuratorBgSprite;
    public Sprite VisitorBgSprite;

    public Image LoadingImage;
    public Image maskImage;

    #region Unity_Internal
    private void OnEnable()
    {
        LogoBtn.onClick.AddListener(LogoBtnClk);
        MiniGameBtn.onClick.AddListener(MiniBtnClk);
        ModelsBtn.onClick.AddListener(ModelsBtnClk);
        CaptureBtn.onClick.AddListener(CaptureBtnClk);
        ModelRotateBtn.onClick.AddListener(ModelRotate);
    }
    private void Start()
    {
#if UNITY_EDITOR
        GetComponentInChildren<CanvasScaler>().enabled = false;
#endif
    }

    private void Update()
    {
        pentaKill();
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.B))
        {
            clkcount = 5;
        }
#endif
    }

    private void OnDisable()
    {
        LogoBtn.onClick.RemoveListener(LogoBtnClk);
        MiniGameBtn.onClick.RemoveListener(MiniBtnClk);
        ModelsBtn.onClick.RemoveListener(ModelsBtnClk);
        CaptureBtn.onClick.RemoveListener(CaptureBtnClk);
        ModelRotateBtn.onClick.RemoveListener(ModelRotate);
    }
    #endregion

    public void Loading(UnityEngine.U2D.SpriteAtlas sa = null)
    {
        if (sa != null)
            SetABUI(sa);
        maskImage.gameObject.SetActive(false);
        LoadingImage.gameObject.SetActive(true);
        StartCoroutine(LoadingCor());
    }
    private IEnumerator LoadingCor()
    {
        yield return new WaitForSeconds(2);
        LoadingImage.gameObject.SetActive(false);
        SetMode(ZClientMode.Visitor);
    }

    public void SetABUI(UnityEngine.U2D.SpriteAtlas sa)
    {
        MiniGameBtn.image.sprite = sa.GetSprite(ZConstant.Minigame);
        MiniGameBtn.spriteState = SetSpriteState(MiniGameBtn.image.sprite, sa.GetSprite(ZConstant.MinigamePress));

        ModelsBtn.image.sprite = sa.GetSprite(ZConstant.Model);
        ModelsBtn.spriteState = SetSpriteState(ModelsBtn.image.sprite, sa.GetSprite(ZConstant.ModelPress));

        CaptureBtn.image.sprite = sa.GetSprite(ZConstant.Photo);
        CaptureBtn.spriteState = SetSpriteState(CaptureBtn.image.sprite, sa.GetSprite(ZConstant.PhotoPress));

        ModelRotateBtn.image.sprite = sa.GetSprite(ZConstant.Rotate);
        ModelRotateBtn.spriteState = SetSpriteState(ModelRotateBtn.image.sprite, sa.GetSprite(ZConstant.RotatePress));

        FirstBtn.image.sprite = sa.GetSprite(ZConstant.First);
        FirstBtn.spriteState = SetSpriteState(FirstBtn.image.sprite, sa.GetSprite(ZConstant.FirstPress));

        SecondBtn.image.sprite = sa.GetSprite(ZConstant.Second);
        SecondBtn.spriteState = SetSpriteState(SecondBtn.image.sprite, sa.GetSprite(ZConstant.SecondPress));

        ThirdBtn.image.sprite = sa.GetSprite(ZConstant.Third);
        ThirdBtn.spriteState = SetSpriteState(ThirdBtn.image.sprite, sa.GetSprite(ZConstant.ThirdPress));

        LoadingImage.sprite = sa.GetSprite(ZConstant.Back);

        CuratorBgSprite = sa.GetSprite(ZConstant.Bg1);
        VisitorBgSprite = sa.GetSprite(ZConstant.Bg2);

        TriggerBtn.GetComponent<Image>().sprite = sa.GetSprite(ZConstant.TouchScreen);
        TriggerBtn.ImageNormal = sa.GetSprite(ZConstant.TouchScreen);
        TriggerBtn.ImageHover = sa.GetSprite(ZConstant.TouchScreen);
    }

    private SpriteState SetSpriteState(Sprite h, Sprite p)
    {
        SpriteState ss;
        ss.highlightedSprite = h;
        ss.pressedSprite = p;
        return ss;
    }

    int clkcount = 0;
    float timer = 0;
    float penta_interval = 1.5f;
    int totalClkCount = 5;
    private void pentaKill()
    {
        if (clkcount >= totalClkCount)
        {
            //ZGlobal.ClientMode = ZGlobal.ClientMode == ZClientMode.Curator ? ZClientMode.Visitor : ZClientMode.Curator;
            ZGlobal.ClientMode = ZClientMode.Curator;
            GameManager.Instance.PentaClkRun();
            SetMode(ZClientMode.Curator);
            clkcount = 0;
            timer = 0;
            return;
        }

        if (timer < penta_interval)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            clkcount = 0;
        }
    }
    public void SetMode(ZClientMode mode)
    {
        Debug.Log("cur mode : " + mode);
        switch (mode)
        {
            case ZClientMode.Curator:

                MiniGameBtn.gameObject.SetActive(true);
                ModelsBtn.gameObject.SetActive(true);
                CaptureBtn.gameObject.SetActive(true);
                ModelRotateBtn.gameObject.SetActive(true);
                FirstBtn.gameObject.SetActive(true);
                SecondBtn.gameObject.SetActive(true);
                ThirdBtn.gameObject.SetActive(true);

                BgImage.sprite = CuratorBgSprite;

                break;


            case ZClientMode.Visitor:

                MiniGameBtn.gameObject.SetActive(false);
                ModelsBtn.gameObject.SetActive(false);
                CaptureBtn.gameObject.SetActive(true);
                ModelRotateBtn.gameObject.SetActive(false);
                FirstBtn.gameObject.SetActive(false);
                SecondBtn.gameObject.SetActive(false);
                ThirdBtn.gameObject.SetActive(false);

                BgImage.sprite = VisitorBgSprite;

                break;


            default:
                break;
        }
    }

    private void LogoBtnClk()
    {
        clkcount++;
        Debug.Log(clkcount);
    }
    private void MiniBtnClk()
    {
        if (!ZMarkerHelper.find && GameManager.Instance.GetAllPlayerStart()) return;
       
        GameManager.Instance.SendPlayMiniGame();
    }
    private void ModelsBtnClk()
    {
        if (!ZMarkerHelper.find && GameManager.Instance.GetAllPlayerStart()) return;

        GameManager.Instance.SendPlayShowModels();
    }
    private void CaptureBtnClk()
    {
        EventCenter.Instance.DispatchEvent(ZConstant.Event__Capture__);
    }
    private void ModelRotate()
    {
        MessageManager.Instance.SendRotate();
    }


}
