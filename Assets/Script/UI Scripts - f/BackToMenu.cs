using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public AudioSource audioSource;      // sahnedeki AudioSource
    public AudioClip menuClickClip;      // buton sesi
    public string menuSceneName = "MainMenuScene";

    public void GoToMenu()
    {
        if (audioSource != null && menuClickClip != null)
        {
            audioSource.PlayOneShot(menuClickClip);
            StartCoroutine(LoadAfterSound());
        }
        else
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }

    private IEnumerator LoadAfterSound()
    {
        yield return new WaitForSeconds(menuClickClip.length);
        SceneManager.LoadScene(menuSceneName);
    }
}
