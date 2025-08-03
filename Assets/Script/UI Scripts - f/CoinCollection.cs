using UnityEngine;
using TMPro;

public class CoinCollection : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public AudioSource audioSource;          // Ses oynatici
    public AudioClip increaseClip;           // Coin arttiginda calacak ses
    public AudioClip decreaseClip;           // Coin azaldiginda calacak ses

    private int lastMoney;                   // Onceki para degeri
    private void OnEnable()
    {
        lastMoney = MoneyManager.Money; // Baslangicta mevcut parayi al
        MoneyManager.OnMoneyChanged += UpdateCoinDisplay;
        UpdateCoinDisplay(MoneyManager.Money);
    }

    private void OnDisable()
    {
        MoneyManager.OnMoneyChanged -= UpdateCoinDisplay;
    }

    private void UpdateCoinDisplay(int currentMoney)
    {
        // Para artisi veya azalisina göre ses cal
        if (currentMoney > lastMoney && increaseClip != null)
        {
            audioSource.PlayOneShot(increaseClip);
        }
        else if (currentMoney < lastMoney && decreaseClip != null)
        {
            audioSource.PlayOneShot(decreaseClip);
        }
        coinText.text = currentMoney.ToString();
        
        Debug.Log($"Coin Display Updated: {currentMoney}");
        
        // Son degeri guncelle
        lastMoney = currentMoney;
    }
}