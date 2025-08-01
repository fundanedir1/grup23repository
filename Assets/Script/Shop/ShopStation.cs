using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShopStation : MonoBehaviour
{
    [SerializeField] private ShopCatalog catalog;

    private void Awake() => GetComponent<Collider>().isTrigger = true;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.E))
            ShopUI.Instance.Open(catalog);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            InteractUI.Instance.ShowPrompt("E – Open shop", transform.position);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            InteractUI.Instance.HidePrompt();
    }
}
