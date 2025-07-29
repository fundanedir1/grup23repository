using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] pages;               // UI sayfaları
    public Button nextButton;
    public Button prevButton;
    public Button closeButton;

    private int currentPage = 0;

    private const string TutorialCompletedKey = "TutorialCompleted";
    private const string TutorialSeenBeforeKey = "TutorialSeenBefore";

    void Start()
    {
        // TEST AMAÇLI: Her play'de tutorial sıfırlansın
        PlayerPrefs.DeleteKey(TutorialCompletedKey);
        PlayerPrefs.DeleteKey(TutorialSeenBeforeKey);

        Debug.Log("TutorialManager başlatıldı.");

        // Tüm sayfaları pasifleştir
        foreach (GameObject page in pages)
            page.SetActive(false);

        // Eğer kullanıcı daha önce görmemişse tutorial'ı başlat
        if (PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 0 &&
            PlayerPrefs.GetInt(TutorialSeenBeforeKey, 0) == 0)
        {
            OpenTutorial();
        }
        else
        {
            Debug.Log("Tutorial daha önce tamamlanmış.");
        }

        // Butonlara fonksiyon bağla
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PreviousPage);
        closeButton.onClick.AddListener(CloseTutorial);
    }

    void OpenTutorial()
    {
        Debug.Log("Tutorial başlatılıyor...");
        currentPage = 0;
        pages[currentPage].SetActive(true);
        PlayerPrefs.SetInt(TutorialSeenBeforeKey, 1);
    }

    void NextPage()
    {
        Debug.Log($"NextPage çağrıldı. Şu anki sayfa: {currentPage}");

        if (currentPage < pages.Length - 1)
        {
            pages[currentPage].SetActive(false);
            currentPage++;
            pages[currentPage].SetActive(true);
            Debug.Log($"Yeni sayfa: {currentPage}");
        }
        else
        {
            Debug.Log("Son sayfaya ulaşıldı. Tutorial kapatılıyor...");
            CloseTutorial();
        }
    }

    void PreviousPage()
    {
        Debug.Log($"PreviousPage çağrıldı. Şu anki sayfa: {currentPage}");

        if (currentPage > 0)
        {
            pages[currentPage].SetActive(false);
            currentPage--;
            pages[currentPage].SetActive(true);
            Debug.Log($"Geri gidildi. Yeni sayfa: {currentPage}");
        }
        else
        {
            Debug.Log("İlk sayfadasın, geri gidemezsin.");
        }
    }

    void CloseTutorial()
    {
        Debug.Log("Tutorial kapatılıyor.");
        foreach (GameObject page in pages)
            page.SetActive(false);

        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
    }
}
