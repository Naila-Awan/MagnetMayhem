using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SplashScreenManager : MonoBehaviour
{
    public float splashDuration = 3f; // seconds to wait before loading next scene
    public string nextSceneName = "MainMenu"; // replace with your main scene name

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
