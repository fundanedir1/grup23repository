using UnityEngine;
using UnityEngine.UI;
using EasyPeasyFirstPersonController;  

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [SerializeField] private GameObject panel;          // ShopPanel
    [SerializeField] private Transform  gridParent;     // Content objesi
    [SerializeField] private ShopCard   cardPrefab;
    [SerializeField] private Button     seedTab, itemTab, upgradeTab, closeBtn;

    private ShopCatalog current;
    private string activeCat = "Seed";

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        seedTab.onClick.AddListener(() => SwitchCat("Seed"));
        itemTab.onClick.AddListener(() => SwitchCat("Item"));
        upgradeTab.onClick.AddListener(() => SwitchCat("Upgrade"));
        closeBtn.onClick.AddListener(Close);
    }

    //–––– PUBLIC API ––––
    public void Open(ShopCatalog catalog)
    {
        current = catalog;
        panel.SetActive(true);
        SwitchCat("Seed");
        TogglePlayer(false);                   
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;        
        FirstPersonController.Instance.SetCursorVisibility(true);
    }
    public void Close()
    {
        TogglePlayer(true);                  
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        FirstPersonController.Instance.SetCursorVisibility(false);
    }

    private void TogglePlayer(bool enable)
    {
        if (FirstPersonController.Instance)
            FirstPersonController.Instance.SetControl(enable);


        if (Shooting.Instance)
            Shooting.Instance.SetShootEnabled(enable);
    }

    //–––– Internal ––––
    private void SwitchCat(string cat)
    {
        activeCat = cat;
        foreach (Transform c in gridParent) Destroy(c.gameObject);

        foreach (var e in current.entries)
        {
            if (e.category.ToString() != cat) continue;
            var card = Instantiate(cardPrefab, gridParent);
            card.Init(e, () => TryPurchase(e));
        }
    }

    private void TryPurchase(ShopEntry e)
    {
        if (!MoneyManager.Spend(e.price))
        {
            InteractUI.Instance.ShowToast("Need more money!");
            return;
        }

        switch (e.category)
        {
            case ShopEntry.Category.Seed:
            case ShopEntry.Category.Item:
                FindObjectOfType<QuickBarInventory>().AddItem(e.giveItem, e.giveAmount);
                break;
            case ShopEntry.Category.Upgrade:
                UpgradeManager.Unlock(e.upgradeKey, e.unlockPrefab);
                break;
        }
        InteractUI.Instance.ShowToast($"Bought {e.displayName}");
        SwitchCat(activeCat);               // refresh grid
    }
}
