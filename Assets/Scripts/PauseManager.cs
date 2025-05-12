using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject countdownTextGO;
    [SerializeField] TextMeshProUGUI countdownText;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        StartCoroutine(ResumeCountdown());
    }
    IEnumerator ResumeCountdown()
    {
        pauseMenuUI.SetActive(false);
        countdownTextGO.SetActive(true);
        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSecondsRealtime(1f); // use real time because Time.timeScale = 0
            count--;
        }
        countdownTextGO.SetActive(false);
        Time.timeScale = 1f; // Resume game time
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freeze game time
        isPaused = true;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Resume time before quitting
        SceneManager.LoadScene("MainMenu");
    }
}
