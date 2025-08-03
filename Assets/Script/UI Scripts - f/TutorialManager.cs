using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialContainer;    // Parent that wraps all tutorial UI (pages + buttons)
    public GameObject[] pages;
    public Button nextButton;
    public Button prevButton;
    public Button closeButton;

    private int currentPage = 0;

    private const string TutorialCompletedKey = "TutorialCompleted";
    private const string TutorialSeenBeforeKey = "TutorialSeenBefore";

    void Start()
    {
        // FOR TESTING: reset tutorial state on each play
        PlayerPrefs.DeleteKey(TutorialCompletedKey);
        PlayerPrefs.DeleteKey(TutorialSeenBeforeKey);

        Debug.Log("TutorialManager initialized.");

        // Initially disable container and all pages
        tutorialContainer.SetActive(false);
        foreach (GameObject page in pages)
            page.SetActive(false);

        // Start tutorial if user hasn't seen or completed it before
        if (PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 0 &&
            PlayerPrefs.GetInt(TutorialSeenBeforeKey, 0) == 0)
        {
            OpenTutorial();
        }
        else
        {
            Debug.Log("Tutorial was already completed.");
        }

        // Attach listeners with debug wrappers
        nextButton.onClick.AddListener(() => {
            Debug.Log("NextButton clicked.");
            NextPage();
        });
        prevButton.onClick.AddListener(() => {
            Debug.Log("PrevButton clicked.");
            PreviousPage();
        });
        closeButton.onClick.AddListener(() => {
            Debug.Log("CloseButton clicked.");
            CloseTutorial();
        });
    }

    public void OpenTutorial()
    {
        Debug.Log("Starting tutorial...");
        currentPage = 0;
        tutorialContainer.SetActive(true);
        ShowPage(currentPage);
        PlayerPrefs.SetInt(TutorialSeenBeforeKey, 1);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 1f;
    }

    private void ShowPage(int index)
    {
        // Disable all pages, then enable only the target one
        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(i == index);
    }

    public void NextPage()
    {
        Debug.Log($"NextPage called. Current page: {currentPage}");

        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
            Debug.Log($"Moved to page: {currentPage}");
        }
        else
        {
            Debug.Log("Reached last page. Closing tutorial...");
            CloseTutorial();
        }
    }

    public void PreviousPage()
    {
        Debug.Log($"PreviousPage called. Current page: {currentPage}");

        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
            Debug.Log($"Moved back to page: {currentPage}");
        }
        else
        {
            Debug.Log("Already on first page; can't go back.");
        }
    }

    public void CloseTutorial()
    {
        Debug.Log("Closing tutorial.");
        tutorialContainer.SetActive(false);
        foreach (GameObject page in pages)
            page.SetActive(false);

        // If buttons are outside the container and should also hide:
        nextButton.gameObject.SetActive(false);
        prevButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
    }
}
