using UnityEngine;
using TMPro;

/// <summary>
/// Attach this to the TMP_Text UI element that shows current money.
/// No singleton HUD needed—MoneyManager calls UpdateDisplay when money changes.
/// </summary>
public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private string prefix = "$ ";

    private void Awake()
    {
        if (moneyText == null) moneyText = GetComponent<TMP_Text>();
        MoneyManager.OnMoneyChanged += UpdateDisplay;
        UpdateDisplay(MoneyManager.Money);
    }
    private void OnDestroy()
    {
        MoneyManager.OnMoneyChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(int amount)
    {
        moneyText.text = prefix + amount.ToString();
    }
}
