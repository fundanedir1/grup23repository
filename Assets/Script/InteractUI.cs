using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// Singleton UI controller for interaction prompts and progress bar.
/// Attach this to a central Screen-Space Canvas in your scene.
/// </summary>
public class InteractUI : MonoBehaviour
{
    public static InteractUI Instance { get; private set; }

    [Header("Prompt")]
    [SerializeField] private RectTransform promptContainer;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private float  promptOffsetY = 100f;

    [Header("Progress Bar")]
    [SerializeField] private RectTransform progressContainer;
    [SerializeField] private Slider         progressBar;


   [Header("Toast")]
    [SerializeField] private TMP_Text toastText;
    [SerializeField] private float toastDuration = 1.5f;

    private Coroutine toastRoutine;

    // … Awake, Prompt/Progress kodunuz …

    public void ShowToast(string msg)
    {
        if (toastRoutine != null) StopCoroutine(toastRoutine);
        toastRoutine = StartCoroutine(ToastRoutine(msg));
    }
    private IEnumerator ToastRoutine(string msg)
    {
        toastText.text = msg;
        toastText.gameObject.SetActive(true);
        float t = 0;
        Color c = toastText.color;
        while (t < toastDuration)
        {
            t += Time.deltaTime;
            toastText.color = new Color(c.r, c.g, c.b, 1 - t / toastDuration);
            yield return null;
        }
        toastText.gameObject.SetActive(false);
        toastText.color = c;
    }

    public void HideToast() => toastText.gameObject.SetActive(false);



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Hide on start
        HidePrompt();
        HideProgress();
    }

    /// <summary>Show the prompt with given text at a world position above the target.</summary>
    public void ShowPrompt(string text, Vector3 worldPos)
    {
        promptText.text = text;
        promptContainer.gameObject.SetActive(true);

        // Convert world to screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        promptContainer.position = screenPos + Vector3.up * promptOffsetY;
    }

    /// <summary>Hide the prompt.</summary>
    public void HidePrompt()
    {
        promptContainer.gameObject.SetActive(false);
    }

    /// <summary>Initialize and show a progress bar for duration seconds.</summary>
    public void ShowProgress(float duration)
    {
        progressBar.minValue = 0f;
        progressBar.maxValue = 1f;
        progressBar.value    = 0f;
        progressContainer.gameObject.SetActive(true);
    }

    /// <summary>Update the current progress (0..1).</summary>
    public void SetProgress(float normalized)
    {
        progressBar.value = Mathf.Clamp01(normalized);
    }

    /// <summary>Hide the progress bar.</summary>
    public void HideProgress()
    {
        progressContainer.gameObject.SetActive(false);
    }
}
