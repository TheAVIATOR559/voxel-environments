using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DEV_TOOLS : MonoBehaviour
{
    [SerializeField] private KeyCode RegenWorldKey = KeyCode.T;
    [SerializeField] private string LoadingScene;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(RegenWorldKey))
        {
            SceneManager.LoadScene(LoadingScene);
        }
    }
}
