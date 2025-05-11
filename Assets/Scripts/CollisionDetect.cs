using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject segmentGenerator;   
    [SerializeField] GameObject playerAnim;
    [SerializeField] AudioSource thudSound;
    [SerializeField] AudioSource fallToGroundSound;
    [SerializeField] GameObject fadeOut;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("nonmetallic")) 
        {
            thePlayer.GetComponent<PlayerMovement>().enabled = false;
            segmentGenerator.GetComponent<SegmentGenerator>().enabled = false;
            playerAnim.GetComponent<Animator>().Play("Falling Back Death");
            StartCoroutine(HandleGameOver(0.3f));
        }
    }

    IEnumerator HandleGameOver(float delay)
    {
        thudSound.Play();
        yield return new WaitForSeconds(delay);
        fallToGroundSound.Play();
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(1.5f); // Let the fade animation play
        SceneManager.LoadScene("GameOverScene");
    }
}
