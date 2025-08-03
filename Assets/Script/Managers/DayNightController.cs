using System.Collections;
using UnityEngine;

/// <summary>
/// Gün ↔ Gece geçişinde:
///     • Directional Light & ambient rengi yumuşakça lerp’ler
///     • İstenirse skybox materyalini **cross‑fade** ile karartır/açar (Skybox/Blended shader’ı gerekir)
/// 
/// Gereksinim:
///     1. Built‑in "Skybox/Blended" (veya _Blend isimli float alanı olan custom shader_) içeren bir template material.
///     2. Gündüz & gece skybox materyalleri (cubemap veya procedural). Shader _Tex özelliğine sahip olmalı.
/// </summary>
[RequireComponent(typeof(Light))]
public class DayNightController : MonoBehaviour
{
    // ───────────────────── Directional Light ─────────────────────
    private Light _sun;

    [Header("Işık Yoğunluğu & Renk")]
    [SerializeField] private float dayIntensity = 1.3f;
    [SerializeField] private float nightIntensity = 0.05f;
    [SerializeField] private Color dayColor = new(1f, 0.956f, 0.839f);
    [SerializeField] private Color nightColor = new(0.184f, 0.286f, 0.396f);

    // ───────────────────── Ambient ─────────────────────
    [Header("Ambient Renk (RenderSettings)")]
    [SerializeField] private Color dayAmbient = new(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color nightAmbient = new(0.02f, 0.02f, 0.05f);

    // ───────────────────── Skybox Materyalleri ─────────────────────
    [Header("Skybox")]
    [Tooltip("Gündüz Skybox Material (Cubemap veya Procedural)")]
    [SerializeField] private Material daySkybox;
    [Tooltip("Gece Skybox Material")]
    [SerializeField] private Material nightSkybox;

    [Header("Yumuşak Skybox Geçişi (Opsiyonel)")]
    [Tooltip("Skybox/Blended (veya _Blend parametreli) template material.")]
    [SerializeField] private Material blendedTemplate;
    [SerializeField] private bool blendSkybox = true;
    [SerializeField] private float skyboxBlendSeconds = 2f;

    // ───────────────────── Geçiş Hızı ─────────────────────
    [Header("Işık & Ambient Lerp Saniyesi")]
    [SerializeField] private float transitionSeconds = 2f;

    private Coroutine _lightRoutine;
    private Coroutine _skyboxRoutine;

    // Shader property ID’leri hız için cache’lenir
    private static readonly int BlendProp = Shader.PropertyToID("_Blend");
    private static readonly int Tex1Prop = Shader.PropertyToID("_Tex");
    private static readonly int Tex2Prop = Shader.PropertyToID("_Tex2");

    private void Awake() => _sun = GetComponent<Light>();

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += HandleStateChanged;
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
    }

    // ───────────────────── State değişiminde tetiklenen ana metot ─────────────────────
    private void HandleStateChanged(GameManager.GameState newState)
    {
        if (newState is GameManager.GameState.Pause or GameManager.GameState.GameOver)
            return;

        bool toDay = newState == GameManager.GameState.Day;

        // Işık & ambient yumuşak geçişi
        if (_lightRoutine != null) StopCoroutine(_lightRoutine);
        _lightRoutine = StartCoroutine(LerpLighting(toDay));

        // Skybox geçişi
        if (blendSkybox && blendedTemplate != null)
        {
            if (_skyboxRoutine != null) StopCoroutine(_skyboxRoutine);
            _skyboxRoutine = StartCoroutine(BlendSkyboxCoroutine(toDay));
        }
        else // anlık değiştir
        {
            RenderSettings.skybox = toDay ? daySkybox : nightSkybox;
            DynamicGI.UpdateEnvironment();
        }
    }

    // ───────────────────── Işık Lerp Coroutine ─────────────────────
    private IEnumerator LerpLighting(bool toDay)
    {
        float startInt = _sun.intensity;
        Color startCol = _sun.color;
        Color startAmb = RenderSettings.ambientLight;

        float targetInt = toDay ? dayIntensity : nightIntensity;
        Color targetCol = toDay ? dayColor : nightColor;
        Color targetAmb = toDay ? dayAmbient : nightAmbient;

        for (float t = 0; t < transitionSeconds; t += Time.deltaTime)
        {
            float p = t / transitionSeconds;
            _sun.intensity = Mathf.Lerp(startInt, targetInt, p);
            _sun.color = Color.Lerp(startCol, targetCol, p);
            RenderSettings.ambientLight = Color.Lerp(startAmb, targetAmb, p);
            yield return null;
        }
        _sun.intensity = targetInt;
        _sun.color = targetCol;
        RenderSettings.ambientLight = targetAmb;
    }

    // ───────────────────── Skybox Blend Coroutine ─────────────────────
    private IEnumerator BlendSkyboxCoroutine(bool toDay)
    {
        // Geçiş materyalini instantiate et, böylece orijinal template değişmez
        Material blendMat = new(blendedTemplate);

        Material fromMat = toDay ? nightSkybox : daySkybox;
        Material toMat   = toDay ? daySkybox  : nightSkybox;

        // Cubemap veya 6‑sided tex fark etmez, shader _Tex & _Tex2 varsa çalışır
        if (fromMat.HasProperty(Tex1Prop))
            blendMat.SetTexture(Tex1Prop, fromMat.GetTexture(Tex1Prop));
        if (blendMat.HasProperty(Tex2Prop))
            blendMat.SetTexture(Tex2Prop, toMat.GetTexture(Tex1Prop));

        RenderSettings.skybox = blendMat;

        for (float t = 0; t < skyboxBlendSeconds; t += Time.deltaTime)
        {
            float p = t / skyboxBlendSeconds;
            float blendVal = toDay ? p : 1f - p;
            blendMat.SetFloat(BlendProp, blendVal);
            yield return null;
        }

        RenderSettings.skybox = toDay ? daySkybox : nightSkybox; // geçiş bitti
        DynamicGI.UpdateEnvironment();
    }
}
