using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMPro.TextMeshProUGUI HintLabel;

    private void Awake()
    {
        Instance = this;
    }

    public void SetHintLabel(string content,bool show = true)
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
