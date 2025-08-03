using System;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public enum GameState { Day, Night, Pause, GameOver }

    public static GameManager Instance { get; private set; }

    [Header("Süreler (saniye)")]
    [SerializeField] private float dayLength = 10f;
    [SerializeField] private float nightLength = 5f;

    // Güncel durum & sayaç
    public GameState CurrentState { get; private set; } = GameState.Day;
    private float stateTimer;
    private bool isPaused;

    // Olaylar
    public event Action<GameState> OnStateChanged;
    public event Action<float> OnTimeTick; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        EnterState(GameState.Day);
    }

    private void Update()
    {
        if (isPaused || CurrentState == GameState.GameOver) return;

        stateTimer -= Time.deltaTime;
        OnTimeTick?.Invoke(stateTimer / GetStateLength(CurrentState));

        if (stateTimer <= 0f)
        {
            // Gece → Gündüz, Gündüz → Gece geçişi
            if (CurrentState == GameState.Day)
                EnterState(GameState.Night);
            else if (CurrentState == GameState.Night)
                EnterState(GameState.Day);
        }
    }

    private void EnterState(GameState newState)
    {
        CurrentState = newState;
        stateTimer = GetStateLength(newState);
        OnStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.Day:
                // Gündüz başladı – farm interaction aktif, düşman spawn kapalı
                break;
            case GameState.Night:
                // Gece başladı – düşman dalgası tetikle
                //FindObjectOfType<ZombieSpawner>()?.StartNightWave();
                break;
            case GameState.GameOver:
                // Oyun bitti ekranı vs.
                break;
        }
    }

    public void PauseGame(bool pause)
    {
        isPaused = pause;
        Time.timeScale = pause ? 0f : 1f;
        if (pause)
            EnterState(GameState.Pause);
        else
            EnterState(CurrentState == GameState.Pause ? GameState.Day : CurrentState);
    }

    public void TriggerGameOver()
    {
        EnterState(GameState.GameOver);
    }

    private float GetStateLength(GameState state)
    {
        return state switch
        {
            GameState.Day => dayLength,
            GameState.Night => nightLength,
            _ => 0f,
        };
    }
}
