using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basit 9 slotluk hızlı çubuk envanteri (Input System bağımsız).<br>
/// Yeni Input System kuruluysa ENABLE_INPUT_SYSTEM makrosu otomatik gelir ve o API kullanılır; değilse eski Input API’sine döner.
/// </summary>
public class QuickBarInventory : MonoBehaviour
{
    #region Item Stack
    [System.Serializable]
    public struct ItemStack
    {
        public ItemData item;
        public int amount;

        public bool IsEmpty => item == null || amount <= 0;
        public void Clear() { item = null; amount = 0; }
    }
    #endregion

    // ───────────────────── Alanlar ─────────────────────
    [Header("Slot Ayarları")]
    [SerializeField] private ItemStack[] slots = new ItemStack[9];
    [SerializeField] private int activeIndex = 0;

    [Header("UI Referansları")]
    [SerializeField] private SlotUI[] slotUIs; // Hierarchy'den 9 adet sürükleyin.
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;

    [Header("Drop Ayarları")]
    [Tooltip("ItemData.worldPrefab yoksa kullanılacak genel ItemWorld prefabı")]
    [SerializeField] private GameObject genericItemWorldPrefab;
    [SerializeField] private float dropForce = 2f;

    // ───────────────────── Unity ─────────────────────
    private void Awake() => RefreshUI();
    private void Update()
    {
        HandleSlotSelection();
        HandleDrop();
    }

    // ───────────────────── Genel API ─────────────────────
    public ItemStack[] Slots => slots;
    public int ActiveIndex => activeIndex;
    public ItemData ActiveItem => slots[activeIndex].item;

    public bool AddItem(ItemData data, int amt = 1)
    {
        if (data == null || amt <= 0) return false;

        // 1) Var olan stack’e ekle
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].amount < data.stackMax)
            {
                int canAdd = Mathf.Min(amt, data.stackMax - slots[i].amount);
                slots[i].amount += canAdd;
                amt -= canAdd;
                if (amt <= 0) { RefreshUI(); return true; }
            }
        }

        // 2) Boş slot bul
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty)
            {
                int add = Mathf.Min(amt, data.stackMax);
                slots[i].item = data;
                slots[i].amount = add;
                amt -= add;
                if (amt <= 0) { RefreshUI(); return true; }
            }
        }
        RefreshUI();
        return false; // Tam sığmadı
    }

    public bool RemoveActive(int amt = 1)
    {
        ref ItemStack s = ref slots[activeIndex];
        if (s.IsEmpty || amt <= 0 || s.amount < amt) return false;
        s.amount -= amt;
        if (s.amount <= 0) s.Clear();
        RefreshUI();
        return true;
    }

    // ───────────────────── Input ─────────────────────
    private void HandleSlotSelection()
    {
#if ENABLE_INPUT_SYSTEM
        // Yeni Input System
        var kb = UnityEngine.InputSystem.Keyboard.current;
        var mouse = UnityEngine.InputSystem.Mouse.current;
        for (int i = 0; i < 9; i++)
            if (kb[(UnityEngine.InputSystem.Key)((int)UnityEngine.InputSystem.Key.Digit1 + i)].wasPressedThisFrame)
                SetActiveSlot(i);
        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f) SetActiveSlot((activeIndex + (scroll > 0 ? -1 : 1) + slots.Length) % slots.Length);
#else
        // Eski Input
        for (int i = 0; i < 9; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) SetActiveSlot(i);
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f) SetActiveSlot((activeIndex + (scroll > 0 ? -1 : 1) + slots.Length) % slots.Length);
#endif
    }

    private void HandleDrop()
    {
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Keyboard.current.gKey.wasPressedThisFrame)
#else
        if (Input.GetKeyDown(KeyCode.G))
#endif
        {
            if (RemoveActive(1))
            {
                ItemData data = ActiveItem; // RemoveActive önce aktif item’i tut
                GameObject prefab = data.worldPrefab ?? genericItemWorldPrefab;
                if (!prefab) return;

                Transform cam = Camera.main.transform;
                Vector3 pos = cam.position + cam.forward * 1.2f + Vector3.down * 0.2f;
                GameObject go = Instantiate(prefab, pos, Quaternion.identity);
                if (go.TryGetComponent(out ItemWorld iw)) iw.Initialize(data, 1);
                if (go.TryGetComponent(out Rigidbody rb)) rb.AddForce(cam.forward * dropForce, ForceMode.Impulse);
            }
        }
    }

    private void SetActiveSlot(int idx)
    {
        if (idx < 0 || idx >= slots.Length || idx == activeIndex) return;
        activeIndex = idx;
        RefreshUI();
    }

    // ───────────────────── UI ─────────────────────
    private void RefreshUI()
    {
        if (slotUIs == null || slotUIs.Length != slots.Length) return;
        for (int i = 0; i < slots.Length; i++)
        {
            slotUIs[i].Set(slots[i].item, slots[i].amount);
            slotUIs[i].SetHighlight(i == activeIndex ? selectedColor : normalColor);
        }
    }
}

// —————————————————————————————————— UI Yardımcısı ——————————————————————————————————
[System.Serializable]
public class SlotUI
{
    public Image iconImg;
    public Text amountTxt; // TextMeshPro kullanıyorsan TMP_Text

    public void Set(ItemData item, int amt)
    {
        if (item == null)
        {
            iconImg.enabled = false;
            amountTxt.text = "";
        }
        else
        {
            iconImg.enabled = true;
            iconImg.sprite = item.icon;
            amountTxt.text = amt > 1 ? amt.ToString() : "";
        }
    }

    public void SetHighlight(Color c) => iconImg.color = c;
}
