using UnityEngine;
using TMPro;

public class CoinCollection : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    private void OnEnable()
    {
        MoneyManager.OnMoneyChanged += UpdateCoinDisplay;
        UpdateCoinDisplay(MoneyManager.Money);
    }

    private void OnDisable()
    {
        MoneyManager.OnMoneyChanged -= UpdateCoinDisplay;
    }

    private void UpdateCoinDisplay(int currentMoney)
    {
        coinText.text = currentMoney.ToString();
    }
}