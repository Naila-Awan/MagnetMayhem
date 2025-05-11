using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    [SerializeField] AudioSource coinFX;

    void OnTriggerEnter(Collider other) // Corrected method name
    {
        coinFX.Play();
        MasterInfo.coinCount++;
        this.gameObject.SetActive(false);
    }
}
