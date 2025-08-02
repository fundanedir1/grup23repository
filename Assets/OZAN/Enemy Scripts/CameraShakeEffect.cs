using System.Collections;
using UnityEngine;

public class CameraShakeEffect : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeIntensity = 0.1f;
    public float shakeSpeed = 10f;
    public AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    
    [Header("Darkness Settings")]
    public float maxDarkness = 0.6f;
    
    [Header("Rainbow Settings")]
    public float rainbowSpeed = 2f;
    public float rainbowIntensity = 0.3f;
    
    private Vector3 originalPosition;
    private Camera playerCamera;
    private bool isShaking = false;
    
    private GameObject effectCanvas;
    private UnityEngine.UI.Image darknessOverlay;
    private UnityEngine.UI.Image rainbowOverlay;
    
 
    public static CameraShakeEffect Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        originalPosition = transform.localPosition;
        
        playerCamera = GetComponentInChildren<Camera>();
        
        CreateEffectOverlays();
    }
    
    private void CreateEffectOverlays()
    {
        effectCanvas = new GameObject("CameraEffectCanvas");
        Canvas canvas = effectCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        
        GameObject darknessGO = new GameObject("DarknessOverlay");
        darknessGO.transform.SetParent(effectCanvas.transform);
        
        var darknessRect = darknessGO.AddComponent<RectTransform>();
        darknessRect.anchorMin = Vector2.zero;
        darknessRect.anchorMax = Vector2.one;
        darknessRect.offsetMin = Vector2.zero;
        darknessRect.offsetMax = Vector2.zero;
        
        darknessOverlay = darknessGO.AddComponent<UnityEngine.UI.Image>();
        darknessOverlay.color = new Color(0, 0, 0, 0);
     
        GameObject rainbowGO = new GameObject("RainbowOverlay");
        rainbowGO.transform.SetParent(effectCanvas.transform);
        
        var rainbowRect = rainbowGO.AddComponent<RectTransform>();
        rainbowRect.anchorMin = Vector2.zero;
        rainbowRect.anchorMax = Vector2.one;
        rainbowRect.offsetMin = Vector2.zero;
        rainbowRect.offsetMax = Vector2.zero;
        
        rainbowOverlay = rainbowGO.AddComponent<UnityEngine.UI.Image>();
        rainbowOverlay.color = new Color(1, 1, 1, 0);
        
        effectCanvas.SetActive(false);
    }
    
  
    public void StartMushroomEffect(float duration)
    {
        if (!isShaking)
        {
            StartCoroutine(MushroomEffectCoroutine(duration));
        }
    }
    
    private IEnumerator MushroomEffectCoroutine(float duration)
    {
        isShaking = true;
        float elapsedTime = 0f;
        
        if (effectCanvas != null)
        {
            effectCanvas.SetActive(true);
        }
        
        Debug.Log("🍄 Kamera efekti başladı!");
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float intensity = shakeCurve.Evaluate(progress);
            
       
            ApplyShake(elapsedTime, intensity);
            
            ApplyDarknessEffect(intensity);
            
            ApplyRainbowEffect(elapsedTime, intensity);
            
            yield return null;
        }
        
        yield return StartCoroutine(RestoreEffects());
        
        isShaking = false;
        Debug.Log("✨ Kamera efekti bitti!");
    }
    
    private void ApplyShake(float time, float intensity)
    {
        float shakeX = (Mathf.PerlinNoise(time * shakeSpeed, 0f) - 0.5f) * 2f;
        float shakeY = (Mathf.PerlinNoise(0f, time * shakeSpeed) - 0.5f) * 2f;
        float shakeZ = (Mathf.PerlinNoise(time * shakeSpeed, time * shakeSpeed) - 0.5f) * 2f;
        
        Vector3 shakeOffset = new Vector3(shakeX, shakeY, shakeZ) * shakeIntensity * intensity;
        transform.localPosition = originalPosition + shakeOffset;
    }
    
    private void ApplyDarknessEffect(float intensity)
    {
        if (darknessOverlay == null) return;
        
        float alpha = intensity * maxDarkness;
        darknessOverlay.color = new Color(0, 0, 0, alpha);
    }
    
    private void ApplyRainbowEffect(float time, float intensity)
    {
        if (rainbowOverlay == null) return;
        
        float hue = Mathf.Repeat(time * rainbowSpeed * 0.1f, 1f);
        Color rainbowColor = Color.HSVToRGB(hue, 0.8f, 1f);
        
        rainbowColor.a = intensity * rainbowIntensity;
        
        rainbowOverlay.color = rainbowColor;
        
        if (playerCamera != null && playerCamera.clearFlags == CameraClearFlags.SolidColor)
        {
            Color bgColor = Color.HSVToRGB(hue, 0.4f, 0.2f);
            playerCamera.backgroundColor = bgColor;
        }
    }
    
    private IEnumerator RestoreEffects()
    {
        float restoreTime = 2f;
        float elapsedTime = 0f;
        
        Vector3 currentPos = transform.localPosition;
        Color currentDarkness = darknessOverlay != null ? darknessOverlay.color : Color.clear;
        Color currentRainbow = rainbowOverlay != null ? rainbowOverlay.color : Color.clear;
        Color originalBgColor = playerCamera != null ? playerCamera.backgroundColor : Color.black;
        
        while (elapsedTime < restoreTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / restoreTime;
            
            transform.localPosition = Vector3.Lerp(currentPos, originalPosition, progress);
            
            if (darknessOverlay != null)
            {
                float alpha = Mathf.Lerp(currentDarkness.a, 0f, progress);
                darknessOverlay.color = new Color(0, 0, 0, alpha);
            }
            
            if (rainbowOverlay != null)
            {
                Color fadeColor = Color.Lerp(currentRainbow, Color.clear, progress);
                rainbowOverlay.color = fadeColor;
            }
            
            if (playerCamera != null)
            {
                playerCamera.backgroundColor = Color.Lerp(playerCamera.backgroundColor, originalBgColor, progress);
            }
            
            yield return null;
        }
        
        transform.localPosition = originalPosition;
        
        if (darknessOverlay != null)
        {
            darknessOverlay.color = new Color(0, 0, 0, 0);
        }
        
        if (rainbowOverlay != null)
        {
            rainbowOverlay.color = new Color(1, 1, 1, 0);
            effectCanvas.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        if (effectCanvas != null)
        {
            DestroyImmediate(effectCanvas);
        }
    }
    
    [ContextMenu("Test Mushroom Effect")]
    public void TestEffect()
    {
        StartMushroomEffect(5f);
    }
}