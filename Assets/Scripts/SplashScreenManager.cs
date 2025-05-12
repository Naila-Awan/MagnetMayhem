using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SplashScreenManager : MonoBehaviour
{
    public float splashDuration = 3f; 
    public string nextSceneName = "MainMenu"; 

    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(splashDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}
