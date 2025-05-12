using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    private AudioSource coinFX;

    private void Start()
    {
        GameObject fxGO = GameObject.Find("CoinCollect"); //---- get audio object
        if (fxGO != null)
        {
            coinFX = fxGO.GetComponent<AudioSource>();
        }
    }

    void OnTriggerEnter(Collider other) // colliding with coin
    {
        coinFX.Play();
        MasterInfo.coinCount++;
        Destroy(gameObject);
    }
}