using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMPro.TextMeshProUGUI HintLabel;

    public GameObject PlayerStatusParents;
    public Toggle[] PlayerStatus;

    private void Awake()
    {
        Instance = this;
        PlayerStatusParents.SetActive(false);

    }

    public void SetPlayerStatusUI(int playerCount)
    {
        PlayerStatusParents.SetActive(true);
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
