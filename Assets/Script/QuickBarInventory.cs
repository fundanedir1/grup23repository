using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickBarInventory : MonoBehaviour
{
    #region ItemStack Struct
    [System.Serializable]
    public struct ItemStack
    {
        public ItemData item;
        public int amount;
        public bool IsEmpty => item == null || amount <= 0;
        public void Clear() { item = null; amount = 0; }
    }
    #endregion

    //───────────────── Inspector Fields ─────────────────
    [Header("Slot Ayarları")]
    [SerializeField] private ItemStack[] slots = new ItemStack[9];
    [SerializeField] private int activeIndex = 0;

    [Header("UI Referansları")]
    [SerializeField] private SlotUI[] slotUIs;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor  = Color.white;

    [Header("Throw Charge Slider UI")]
    [SerializeField] private Slider throwChargeSlider;

    [Header("Drop Ayarları (G)")]
    [SerializeField] private GameObject genericItemWorldPrefab;
    [SerializeField] private float dropForce = 2f;

    [Header("Throw Ayarları (Sağ‑Tık)")]
    [SerializeField] private float throwMinForce    = 4f;
    [SerializeField] private float throwMaxForce    = 12f;
    [SerializeField] private float maxChargeSeconds = 1.5f;

    [Header("Pickup Ayarları (F)")]
    [SerializeField] private float pickupRadius = 2f;

    [Header("Elde Tutma (Hold Display)")]
    [SerializeField] private Transform holdItemParent;

    //───────────────── Private State ─────────────────
    private GameObject currentHeldObject;
    private bool  isChargingThrow;
    private float throwChargeTimer;

    //───────────────── Unity ─────────────────
    private void Awake()
    {
        RefreshUI();
        if (throwChargeSlider)
        {
            throwChargeSlider.minValue = 0f;
            throwChargeSlider.maxValue = 1f;
            throwChargeSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleSlotSelection();
        HandleDrop();
        HandleThrow();
        HandlePickup();
        UpdateHeldItem();
    }

    //───────────────── Public Helpers ─────────────────
    public ItemData ActiveItem => slots[activeIndex].item;

    /// <summary>Adds item, returns remaining amount that could NOT be added.</summary>
    public int AddItem(ItemData data, int amt = 1)
    {
        if (data == null || amt <= 0) return amt;
        for (int i = 0; i < slots.Length && amt > 0; i++)
        {
            if (slots[i].item == data && slots[i].amount < data.stackMax)
            {
                int canAdd = Mathf.Min(amt, data.stackMax - slots[i].amount);
                slots[i].amount += canAdd;
                amt -= canAdd;
            }
        }
        for (int i = 0; i < slots.Length && amt > 0; i++)
        {
            if (slots[i].IsEmpty)
            {
                int add = Mathf.Min(amt, data.stackMax);
                slots[i].item = data;
                slots[i].amount = add;
                amt -= add;
            }
        }
        RefreshUI();
        return amt;
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

    //───────────────── Input: Slot Selection ─────────────────
    private void HandleSlotSelection()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = UnityEngine.InputSystem.Keyboard.current;
        var mouse = UnityEngine.InputSystem.Mouse.current;
        for (int i = 0; i < 9; i++)
            if (kb[(UnityEngine.InputSystem.Key)((int)UnityEngine.InputSystem.Key.Digit1 + i)].wasPressedThisFrame)
                SetActiveSlot(i);
        float scroll = mouse.scroll.ReadValue().y;
#else
        for (int i = 0; i < 9; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) SetActiveSlot(i);
        float scroll = Input.mouseScrollDelta.y;
#endif
        if (Mathf.Abs(scroll) > 0.01f)
            SetActiveSlot((activeIndex + (scroll > 0 ? -1 : 1) + slots.Length) % slots.Length);
    }

    private void SetActiveSlot(int idx)
    {
        if (idx < 0 || idx >= slots.Length || idx == activeIndex) return;
        activeIndex = idx;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (slotUIs == null || slotUIs.Length != slots.Length) return;
        for (int i = 0; i < slots.Length; i++)
        {
            slotUIs[i].Set(slots[i].item, slots[i].amount);
            slotUIs[i].SetHighlight(i == activeIndex ? selectedColor : normalColor);
        }
    }

    //───────────────── Input: Drop (G) ─────────────────
    private void HandleDrop()
    {
#if ENABLE_INPUT_SYSTEM
        bool dropKey = UnityEngine.InputSystem.Keyboard.current.gKey.wasPressedThisFrame;
#else
        bool dropKey = Input.GetKeyDown(KeyCode.G);
#endif
        if (dropKey) DropItem();
    }

    private void DropItem()
    {
        ItemData data = ActiveItem;
        if (data == null) return;
        if (SpawnWorldItem(data, dropForce))
            RemoveActive(1);
    }

    //───────────────── Input: Throw (Right Click) ─────────────────
    private void HandleThrow()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (ActiveItem == null) return;
            isChargingThrow = true;
            throwChargeTimer = 0f;
            if (throwChargeSlider)
            {
                throwChargeSlider.value = 0;
                throwChargeSlider.gameObject.SetActive(true);
            }
        }

        if (isChargingThrow)
        {
            throwChargeTimer += Time.deltaTime;
            if (throwChargeTimer > maxChargeSeconds) throwChargeTimer = maxChargeSeconds;
            if (throwChargeSlider) throwChargeSlider.value = throwChargeTimer / maxChargeSeconds;
        }

        if (Input.GetMouseButtonUp(1) && isChargingThrow)
        {
            float pct = throwChargeTimer / maxChargeSeconds;
            float force = Mathf.Lerp(throwMinForce, throwMaxForce, pct);
            if (SpawnWorldItem(ActiveItem, force)) RemoveActive(1);

            isChargingThrow = false;
            if (throwChargeSlider) throwChargeSlider.gameObject.SetActive(false);
        }
    }

    //───────────────── Input: Pickup (F) ─────────────────
    private void HandlePickup()
    {
#if ENABLE_INPUT_SYSTEM
        bool pickKey = UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame;
#else
        bool pickKey = Input.GetKeyDown(KeyCode.F);
#endif
        if (!pickKey) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius);
        foreach (var col in hits)
        {
            if (!col.TryGetComponent(out ItemWorld iw)) continue;
            // Eldeki görsel item ise atla
            if (currentHeldObject && iw.gameObject == currentHeldObject) continue;
            if (currentHeldObject && iw.transform.IsChildOf(currentHeldObject.transform)) continue;

            int remaining = AddItem(iw.itemData, iw.amount);
            if (remaining <= 0)
                Destroy(col.gameObject);
            else
                iw.amount = remaining;
        }
    }

    //───────────────── Spawn Helper ─────────────────
    private bool SpawnWorldItem(ItemData data, float force)
    {
        GameObject prefab = data.worldPrefab ?? genericItemWorldPrefab;
        if (!prefab) return false;
        Transform cam = Camera.main.transform;
        Vector3 pos = cam.position + cam.forward * 1.2f + Vector3.down * 0.2f;
        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        if (go.TryGetComponent(out ItemWorld iw)) iw.Initialize(data, 1);
        if (go.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(cam.forward * force, ForceMode.Impulse);
        }
        return true;
    }

    //───────────────── Held Item Display ─────────────────
    private void UpdateHeldItem()
    {
        ItemData current = ActiveItem;
        if (current == null || current.worldPrefab == null)
        {
            if (currentHeldObject != null) Destroy(currentHeldObject);
            return;
        }

        if (currentHeldObject == null || currentHeldObject.name != current.worldPrefab.name + "(Clone)")
        {
            if (currentHeldObject != null) Destroy(currentHeldObject);
            currentHeldObject = Instantiate(current.worldPrefab, holdItemParent);
            currentHeldObject.transform.localPosition = Vector3.zero;
            currentHeldObject.transform.localRotation = Quaternion.identity;

            // Disable physics & colliders so it’s not picked up again
            foreach (var col in currentHeldObject.GetComponentsInChildren<Collider>())
                col.enabled = false;
            if (currentHeldObject.TryGetComponent(out Rigidbody heldRb))
            {
                heldRb.isKinematic = true;
                heldRb.useGravity = false;
            }
        }
    }
}

//───────────────── UI Helper ─────────────────
[System.Serializable]
public class SlotUI
{
    public Image iconImg;
    public TMP_Text amountTxt;
    public void Set(ItemData item, int amt)
    {
        if (item == null)
        {
            iconImg.enabled = false;
            amountTxt.text = string.Empty;
        }
        else
        {
            iconImg.enabled = true;
            iconImg.sprite = item.icon;
            amountTxt.text = amt > 1 ? amt.ToString() : string.Empty;
        }
    }

    public void SetHighlight(Color c) => iconImg.color = c;
}
