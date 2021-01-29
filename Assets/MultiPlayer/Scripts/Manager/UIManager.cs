using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMPro.TextMeshProUGUI HintLabel;
    public TMPro.TextMeshProUGUI CountdownLabel;

    public GameObject PlayerStatusParents;
    public Toggle[] PlayerStatus;

    private void Awake()
    {
        Instance = this;
        PlayerStatusParents.SetActive(false);

    }

    public void SetPlayerStatusUI(int playerCount,bool show)
    {
        PlayerStatusParents.SetActive(show);
        for (int i = 0; i < PlayerStatus.Length; i++)
        {
            PlayerStatus[i].isOn = i < playerCount;
        }
    }

    // sample ui tip
    public void SetHintLabel(string content, bool show = true)
    {
        HintLabel.gameObject.SetActive(show);

        if (!show)
        {
            return;
        }
        else
        {
            HintLabel.text = content;
        }
    }

    public void SetCountdown(string content, bool show = true)
    {
        CountdownLabel.gameObject.SetActive(show);

        if (show)
        {
            CountdownLabel.text = content;
        }
    }
}
