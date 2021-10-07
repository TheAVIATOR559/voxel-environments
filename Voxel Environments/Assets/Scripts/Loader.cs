using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] Image loadingBar;
    private void Start()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    private IEnumerator LoadAsyncOperation()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        while(asyncLoad.progress < 1)
        {
            loadingBar.fillAmount = asyncLoad.progress;
            //Debug.Log(asyncLoad.progress);
            yield return new WaitForEndOfFrame();
        }
    }
}
