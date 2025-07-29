using UnityEngine;

public class SimpleFps : MonoBehaviour
{
    // Hareket Ayarlar?
    public float moveSpeed = 5.0f;
    public float mouseSensitivity = 2.0f;

    // Kamera referans? (Inspector'da atanacak)
    public Transform playerCamera;

    private float verticalRotation = 0f; // Dikey kamera dönü?ü için

    // Bu scriptin aktif olup olmad???n? d??ar?dan kontrol etmek için bir anahtar
    // TutorialManager bu özelli?i kullanarak scripti etkinle?tirecek/devre d??? b?rakacak.
    private bool isInputEnabled = true;

    void Start()
    {
        // SADECE BA?LANGIÇTA fareyi kilitler ve gizler (oyun ba?larken)
        // Sonraki tüm fare ayarlar?n? TutorialManager yönetecek.
        SetCursorState(true);
    }

    void Update()
    {
        // E?er giri? aktif de?ilse, hiçbir ?ey yapma (hareket ve kamera kontrolü pasif)
        if (!isInputEnabled)
        {
            return;
        }

        // Kamera Kontrolü (Fare)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Yatay dönü? (oyuncunun kendisi döner)
        transform.Rotate(Vector3.up * mouseX);

        // Dikey dönü? (sadece kamera döner)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); // Kameray? yukar?/a?a?? s?n?rlama
        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }

        // Hareket Kontrolü (WASD)
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        transform.position += moveDirection;
    }

    // Fare durumunu ayarlamak için d??ar?dan ça?r?lacak metod (Sadece TutorialManager ça??r?r)
    public void SetCursorState(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Giri?in aktif olup olmad???n? ayarlamak için d??ar?dan ça?r?lacak metod
    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled = enabled;
        // BURADAN SetCursorState(!enabled); SATIRINI S?LD?K VEYA YORUM SATIRI YAPTIK!
    }
}