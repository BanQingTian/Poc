using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMPro.TextMeshProUGUI HintLabel;

    public Toggle[] PlayerStatus;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerStatusUI(int playerCount)
    {
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

}
