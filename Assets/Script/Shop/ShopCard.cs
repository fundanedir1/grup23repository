using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCard : MonoBehaviour
{
    [SerializeField] private Image   icon;
    [SerializeField] private TMP_Text priceTxt;
    private ShopEntry entry;

    public void Init(ShopEntry e, System.Action onClick)
    {
        entry   = e;
        icon.sprite     = e.icon;
        priceTxt.text   = e.price.ToString();
        GetComponent<Button>().onClick.AddListener(() => onClick?.Invoke());
        //GetComponent<TooltipTrigger>()?.SetText(e.displayName, e.GetDescription());
    }
}
