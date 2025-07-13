using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCollection : MonoBehaviour
{

    private int Coin = 0;
    public TextMeshProUGUI coinText;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Coin")
        {
            Coin++;
            coinText.text = Coin.ToString();
            Debug.Log("Coins: " + Coin);
            Destroy(other.gameObject);
        }   

    }
}
