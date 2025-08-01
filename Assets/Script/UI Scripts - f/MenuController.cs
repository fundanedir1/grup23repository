using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 


public class MenuController : MonoBehaviour
{
    public Button playButton;          
    public TMP_Text playButtonText;        

    private const string HasPlayedKey = "HasPlayedBefore";  // // PlayerPrefs key

    void Start()
    {
        if (PlayerPrefs.GetInt(HasPlayedKey, 0) == 1)
        {
            playButtonText.text = "Continue";  
        }
        else
        {
            playButtonText.text = "Play";      
        }
    }

    public void PlayGame()
    {
        Debug.Log("Play butonuna basildi.");

      

        PlayerPrefs.SetInt(HasPlayedKey, 1);   
        PlayerPrefs.Save();

        SceneManager.LoadScene("Map");   
    }

    public void RestartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; 
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene"); 
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("CreditsScene"); 
    }

    public void QuitGame()
    {
        Debug.Log("Oyun kapatiliyor...");
        Application.Quit();
    }
}
