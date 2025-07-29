using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] pages;        // Tutorial sayfalari (UI Panelleri)
    private int currentPage = 0;

    // BURAYI EKLE: SimpleFPS scriptine referans
    public SimpleFps fpsControllerScript; // Inspector'dan atanacak

    private const string TutorialCompletedKey = "TutorialCompleted";
    private const string TutorialSeenBeforeKey = "TutorialSeenBefore";

    public GameObject ileriButton;
    public GameObject geriButton;
    public GameObject closeButton;


    void Start()
    {


        // Tutorial daha önce tamamlanmamış ve daha önce gösterilmemişse aç
        if (PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 0 &&
            PlayerPrefs.GetInt(TutorialSeenBeforeKey, 0) == 0)
        {
            OpenTutorial();
            PlayerPrefs.SetInt(TutorialSeenBeforeKey, 1);
            PlayerPrefs.Save();
        }
        else
        {
            // Tutorial açılmayacak, sayfaları kapat
            foreach (var page in pages)
            {
                if (page != null)
                    page.SetActive(false);
            }
            Time.timeScale = 1f; // Oyun devam etsin
        }


    }

    public void OpenTutorial()
    {
        currentPage = 0;
        // Sayfaları ilk sayfa haric kapat, sadece ilk sayfayı aç
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(i == currentPage);
        }

        Time.timeScale = 0f; // Oyunu durdur

        // Cursor ayarları
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // FPS controller kapat
        if (fpsControllerScript != null)
            fpsControllerScript.SetInputEnabled(false);

        // İleri, geri ve kapatma butonlarını aç
        ileriButton.SetActive(true);
        geriButton.SetActive(true);
        closeButton.SetActive(true);
    }

    // Sonraki sayfaya gecer
    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            if (pages[currentPage] != null)
                pages[currentPage].SetActive(false);

            currentPage++;

            if (pages[currentPage] != null)
                pages[currentPage].SetActive(true);
        }
       
    }

    // Onceki sayfaya doner
    public void PrevPage()
    {
        if (currentPage > 0)
        {
            if (pages[currentPage] != null)
                pages[currentPage].SetActive(false);

            currentPage--;

            if (pages[currentPage] != null)
                pages[currentPage].SetActive(true);
        }
    }

    // Tutorial'i kapatir ve oyunu devam ettirir
    public void CloseTutorial()
    {
        // Sayfaları kapat
        foreach (var page in pages)
        {
            if (page != null)
                page.SetActive(false);
        }

        Time.timeScale = 1f; // Oyunu devam ettir

        // Cursor ayarları
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // FPS controller aç
        if (fpsControllerScript != null)
            fpsControllerScript.SetInputEnabled(true);

        // Tutorial tamamlandı olarak işaretle
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
        PlayerPrefs.Save();

        ileriButton.SetActive(false);
        geriButton.SetActive(false);
        closeButton.SetActive(false);
    }
}