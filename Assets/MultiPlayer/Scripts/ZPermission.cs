using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class ZPermission : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        SceneManager.LoadScene("Main");
    }


}
