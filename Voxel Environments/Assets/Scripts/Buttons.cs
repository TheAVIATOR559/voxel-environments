using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void LoadDesert()
    {
        SceneManager.LoadScene("Loading Desert");
    }

    public void LoadBoreal()
    {
        SceneManager.LoadScene("Loading Boreal");
    }

    public void LoadDecidous()
    {
        SceneManager.LoadScene("Loading Decidous");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
