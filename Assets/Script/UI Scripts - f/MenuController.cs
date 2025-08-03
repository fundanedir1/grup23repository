using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    public Button playButton;
    public TMP_Text playButtonText;

    [Header("Buton Sesi")]
    public AudioSource audioSource;      // Sahneye atanms AudioSource
    public AudioClip buttonClip;         // calmak istedigin ses

    private const string HasPlayedKey = "HasPlayedBefore";

    void Start()
    {
        if (PlayerPrefs.GetInt(HasPlayedKey, 0) == 1)
            playButtonText.text = "Continue";
        else
            playButtonText.text = "Play";

        // Butonlara listener eklemek (istege bagli, Inspector uzerinden de yapilabilir)
        playButton.onClick.AddListener(PlayGame);
    }

    private void PlayClickSound()
    {
        if (audioSource != null && buttonClip != null)
            audioSource.PlayOneShot(buttonClip);
    }

    public void PlayGame()
    {
        PlayClickSound();
        Debug.Log("Play butonuna basildi.");

        PlayerPrefs.SetInt(HasPlayedKey, 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Map");
    }

    public void RestartGame()
    {
        PlayClickSound();

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
            nextSceneIndex = 0;

        SceneManager.LoadScene(nextSceneIndex);
    }

    public void OpenSettings()
    {
        PlayClickSound();
        SceneManager.LoadScene("SettingsScene");
    }

    public void OpenCredits()
    {
        PlayClickSound();
        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Oyun kapatiliyor...");
        Application.Quit();
    }
}
