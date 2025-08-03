using UnityEngine;

public class QuitButton : MonoBehaviour
{
    [Header("Buton Sesi")]
    public AudioSource audioSource;
    public AudioClip buttonClip;

    public void QuitGame()
    {
        if (audioSource != null && buttonClip != null)
        {
            StartCoroutine(PlaySoundThenQuit());
        }
        else
        {
            // Ses tanimli degilse hemen cikis
            DoQuit();
        }
    }

    private System.Collections.IEnumerator PlaySoundThenQuit()
    {
        audioSource.PlayOneShot(buttonClip);
        yield return new WaitForSeconds(buttonClip.length);
        DoQuit();
    }

    private void DoQuit()
    {
        // Build'te calisir: oyunu kapatir
        Application.Quit();

        // Eger editorde test ediyorsan asagidakini devre disi birakma (sadece editorde  calissin):
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
