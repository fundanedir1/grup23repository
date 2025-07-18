using UnityEngine;

public class SeedPlanter : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public float interactDistance = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.CompareTag("Soil"))
                {
                    SeedData selected = inventoryManager.GetSelectedSeed();
                    Vector3 position = hit.point;
                    Quaternion rotation = Quaternion.identity;

                    Instantiate(selected.plantPrefab, position, rotation);
                    Debug.Log(selected.seedName + " ekildi.");
                }
            }
        }
    }
}
