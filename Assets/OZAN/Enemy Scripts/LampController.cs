using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampController : MonoBehaviour
{
    [Header("Lamba Ayarları")]
    [SerializeField] private Light[] lampLights; 
    [SerializeField] private bool findLightsAutomatically = true; 
    [SerializeField] private bool useUniformIntensity = false; 
    [SerializeField] private float uniformIntensity = 1f; 
    
    [Header("Geçiş Efektleri")]
    [SerializeField] private bool useFadeEffect = true;
    [SerializeField] private float fadeSpeed = 2f;
    
    private bool isLampOn = false;
    private float[] targetIntensities;
    private float[] originalIntensities;

    private void Start()
    {
        if (findLightsAutomatically)
        {
            lampLights = GetComponentsInChildren<Light>();
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleStateChange;
        }
        
        if (lampLights != null && lampLights.Length > 0)
        {
            originalIntensities = new float[lampLights.Length];
            targetIntensities = new float[lampLights.Length];
            
            for (int i = 0; i < lampLights.Length; i++)
            {
                if (lampLights[i] != null)
                {
                    originalIntensities[i] = useUniformIntensity ? uniformIntensity : lampLights[i].intensity;
                    targetIntensities[i] = originalIntensities[i];
                }
            }
        }
        
        if (GameManager.Instance?.CurrentState == GameManager.GameState.Day)
        {
            SetLampState(false, false); 
        }
    }

    private void Update()
    {
        if (useFadeEffect && lampLights != null)
        {
            for (int i = 0; i < lampLights.Length; i++)
            {
                if (lampLights[i] != null && i < targetIntensities.Length)
                {
                    lampLights[i].intensity = Mathf.Lerp(lampLights[i].intensity, targetIntensities[i], Time.deltaTime * fadeSpeed);
                }
            }
        }
    }

    private void HandleStateChange(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Day:
                SetLampState(false, useFadeEffect);
                break;
            case GameManager.GameState.Night:
                SetLampState(true, useFadeEffect);
                break;
            case GameManager.GameState.Pause:
                break;
            case GameManager.GameState.GameOver:
                break;
        }
    }

    private void SetLampState(bool turnOn, bool useAnimation)
    {
        isLampOn = turnOn;

        if (lampLights != null)
        {
            for (int i = 0; i < lampLights.Length; i++)
            {
                if (lampLights[i] != null && i < originalIntensities.Length)
                {
                    if (useAnimation)
                    {
                        targetIntensities[i] = turnOn ? originalIntensities[i] : 0f;
                    }
                    else
                    {
                        lampLights[i].intensity = turnOn ? originalIntensities[i] : 0f;
                        if (i < targetIntensities.Length)
                            targetIntensities[i] = lampLights[i].intensity;
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChange;
        }
    }
    
    [ContextMenu("Test - Turn On")]
    public void TestTurnOn()
    {
        SetLampState(true, true);
    }

    [ContextMenu("Test - Turn Off")]
    public void TestTurnOff()
    {
        SetLampState(false, true);
    }
}
