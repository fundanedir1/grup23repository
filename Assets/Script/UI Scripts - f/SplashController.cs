using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SplashController : MonoBehaviour
{
    //public UnityEngine.UI.Image teamLogo;
    //public UnityEngine.UI.Image gameLogo;
    public float fadeDuration = 1f;
    public float displayDuration = 1f;
    public float maxScale = 1.2f; // etkili zoom icin
    public string nextSceneName = "MainMenuScene";

    public AudioSource audioSource;
    //public AudioClip teamLogoSound;
    //public AudioClip gameLogoSound;

    private bool canProceed = false;
    public GameObject menuButton;


    void Start()
    {
        menuButton.SetActive(false); // menu basta gorunmesin
        StartCoroutine(PlaySplashSequence());
        //Debug.Log("Ekip logosu gosteriliyor...");

    }

    void Update()
    {
        if (canProceed && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator PlaySplashSequence()
    {
        //yield return StartCoroutine(ShowLogo(teamLogo, teamLogoSound));
        //yield return StartCoroutine(ShowLogo(gameLogo, gameLogoSound));

        canProceed = true;
        Debug.Log("Tiklayinca devam edecek...");

        // Menu butonunu goster
        menuButton.SetActive(true);
        yield break; // Coroutine burada sonlanir
    }

    IEnumerator ShowLogo(UnityEngine.UI.Image logo, AudioClip sound)
    {
        logo.gameObject.SetActive(true);
        Color c = logo.color;
        Vector3 originalScale = logo.rectTransform.localScale;
        Vector3 targetScale = originalScale * maxScale;

        // baslangic
        c.a = 0;
        logo.color = c;
        logo.rectTransform.localScale = originalScale;

        // ses cal
        audioSource.clip = sound;
        audioSource.Play();

        float t = 0;

        // Fade In + scale up
        while (t < fadeDuration)
        {
            float ratio = t / fadeDuration;
            c.a = Mathf.Lerp(0, 1, ratio);
            logo.color = c;
            logo.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, ratio);
            t += Time.deltaTime;
            yield return null;
        }
        logo.color = c = new Color(c.r, c.g, c.b, 1);
        logo.rectTransform.localScale = targetScale;

        yield return new WaitForSeconds(displayDuration);

        // Fade Out + scale down
        t = 0;
        while (t < fadeDuration)
        {
            float ratio = t / fadeDuration;
            c.a = Mathf.Lerp(1, 0, ratio);
            logo.color = c;
            logo.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, ratio);
            t += Time.deltaTime;
            yield return null;
        }
        c.a = 0;
        logo.color = c;
        logo.rectTransform.localScale = originalScale;

        logo.gameObject.SetActive(false);

        
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene"); // Sahne adini birebir dogru yaz
    }
}
