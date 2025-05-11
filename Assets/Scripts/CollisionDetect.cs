using UnityEngine;
using System.Collections;

public class CollisionDetect : MonoBehaviour
{
    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject playerAnim;
    [SerializeField] AudioSource thudSound;
    [SerializeField] AudioSource fallToGroundSound;
    [SerializeField] GameObject fadeOut;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("nonmetallic")) 
        {
            thePlayer.GetComponent<PlayerMovement>().enabled = false;
            playerAnim.GetComponent<Animator>().Play("Falling Back Death");
            StartCoroutine(PlaySoundsWithDelay(0.3f)); // Delay of 100 milliseconds
        }
    }

    IEnumerator PlaySoundsWithDelay(float delay)
    {
        thudSound.Play(); // Play the thud sound
        yield return new WaitForSeconds(delay); // Wait for 100 milliseconds
        fallToGroundSound.Play(); // Play the fall to the ground sound
        fadeOut.SetActive(true); // Activate the fade out effect
    }

}