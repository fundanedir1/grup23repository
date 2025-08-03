using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public void PauseAndReturnToMenu()
    {
        Debug.Log("Pause butonuna basildi, MainMenuScene'ye geciliyor...");

        SceneManager.LoadScene("MainMenuScene");
    }
}
