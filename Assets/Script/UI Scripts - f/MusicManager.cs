using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Müzik")]
    public AudioSource audioSource;
    public AudioClip backgroundMusic;

    [Header("Fade Ayarlari")]
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 1f; // Baslangic seviyesi

            PlayMusic();
        }
        else
        {
            Destroy(gameObject); // Zaten varsa cift olmasi
                                 
        }
    }

    public void PlayMusic()
    {
        if (audioSource.isPlaying) return;
        StartCoroutine(FadeIn());
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOut());
    }

    public void SetVolume(float v)
    {
        audioSource.volume = Mathf.Clamp01(v);
    }

    private IEnumerator FadeIn()
    {
        float target = 1f;
        audioSource.volume = 0f;
        audioSource.Play();
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, target, elapsed / fadeDuration);
            yield return null;
        }
        audioSource.volume = target;
    }

    private IEnumerator FadeOut()
    {
        float start = audioSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0f, elapsed / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
