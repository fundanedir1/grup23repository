using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WaveUI : MonoBehaviour
{
    private static WaveUI _instance;
    [SerializeField] private TMP_Text waveTxt;

    private void Awake()
    {
        _instance = this;
        if (!waveTxt) waveTxt = GetComponent<TMP_Text>();
    }

    public static void UpdateWave(int current, int total)
    {
        if (_instance && _instance.waveTxt)
            _instance.waveTxt.text = $"Wave {current}/{total}";
    }
}

