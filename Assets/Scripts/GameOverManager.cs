using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] TMP_Text finalScoreText;

    void Start()
    {
        // Show the final score
        finalScoreText.text = "FINAL SCORE: " + MasterInfo.coinCount;
    }

    public void ReplayGame()
    {
        // Reset score if needed
        MasterInfo.coinCount = 0;
        SceneManager.LoadScene("SampleScene");
    }

    public void MainMenu()
    {
        // Reset score if needed
        MasterInfo.coinCount = 0;
        SceneManager.LoadScene("MainMenu");
    }
}
