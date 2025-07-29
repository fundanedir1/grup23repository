using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCollection : MonoBehaviour
{

    private int Coin = 0;
    public TextMeshProUGUI coinText;

    public PlayerAudioManager audioManager;


    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Coin")
        {
            Coin++;
            coinText.text = Coin.ToString();
            Debug.Log("Coins: " + Coin);
            if (audioManager != null)
                audioManager.OnCoinCollected();
            Destroy(other.gameObject);
        }   

    }  
}
