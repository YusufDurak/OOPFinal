using UnityEngine;
using TMPro;

/// <summary>
/// OOP PRINCIPLE: SINGLETON PATTERN (Creational Design Pattern)
/// Ensures only one instance of GameManager exists throughout the game lifecycle.
/// Provides global access point while maintaining encapsulation.
/// This prevents multiple conflicting game states and centralizes game-level logic.
/// 
/// OOP PRINCIPLE: MEDIATOR PATTERN
/// GameManager acts as a mediator between EnemySpawner and EnemyController,
/// coordinating communication without tight coupling between them.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager _instance;
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Public property with private setter ensures controlled access to the singleton instance.
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager instance is null! Make sure there's a GameManager in the scene.");
            }
            return _instance;
        }
    }
    
    // Game state enum
    public enum GameState
    {
        Playing,
        GameOver,
        Victory
    }
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    // Private fields with public properties (Encapsulation)
    private int _score;
    private float _currentTime;
    private GameState _gameState;
    private int _currentWave = 1;
    
    /// OOP PRINCIPLE: DEPENDENCY INVERSION
    /// GameManager depends on EnemySpawner interface, not implementation details
    private EnemySpawner _spawner;
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Properties provide controlled access to private fields.
    /// Allows validation and side effects when values change.
    /// </summary>
    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            UpdateScoreUI();
            Debug.Log($"Score updated: {_score}");
        }
    }
    
    public int CurrentWave
    {
        get { return _currentWave; }
        private set { _currentWave = value; }
    }
    
    public float CurrentTime
    {
        get { return _currentTime; }
        private set { _currentTime = value; }
    }
    
    public GameState State
    {
        get { return _gameState; }
        set
        {
            _gameState = value;
            OnGameStateChanged(_gameState);
        }
    }
    
    private void Awake()
    {
        // Singleton initialization
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        
        // Initialize game state
        _score = 0;
        _currentTime = 0f;
        _gameState = GameState.Playing;
    }
    
    private void Update()
    {
        if (_gameState == GameState.Playing)
        {
            _currentTime += Time.deltaTime;
        }
    }
    
    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                Debug.Log("Game Started!");
                Time.timeScale = 1f;
                break;
            case GameState.GameOver:
                Debug.Log("Game Over!");
                Time.timeScale = 0f;
                UpdateWaveUI("GAME OVER");
                break;
            case GameState.Victory:
                Debug.Log("Victory! All waves completed!");
                Time.timeScale = 0f;
                UpdateWaveUI("VICTORY!");
                break;
        }
    }
    
    public void AddScore(int points)
    {
        Score += points;
    }
    
    public void EndGame()
    {
        State = GameState.GameOver;
    }
    
    /// <summary>
    /// OOP PRINCIPLE: MEDIATOR PATTERN
    /// GameManager mediates communication between EnemySpawner and EnemyController.
    /// This prevents direct coupling between spawner and individual enemies.
    /// </summary>
    public void RegisterSpawner(EnemySpawner spawner)
    {
        _spawner = spawner;
        Debug.Log("EnemySpawner registered with GameManager");
    }
    
    /// <summary>
    /// OOP PRINCIPLE: EVENT-DRIVEN ARCHITECTURE
    /// Called by EnemyController when an enemy dies.
    /// Forwards the notification to the spawner without tight coupling.
    /// This is the Observer pattern - GameManager observes enemy deaths.
    /// </summary>
    public void OnEnemyKilled()
    {
        if (_spawner != null)
        {
            _spawner.OnEnemyKilled();
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Wave progression logic is handled through controlled public methods
    /// </summary>
    public void OnWaveStarted(int waveNumber, string waveName)
    {
        _currentWave = waveNumber;
        UpdateWaveUI(waveName);
        Debug.Log($"Wave {waveNumber} started: {waveName}");
    }
    
    public void OnWaveCompleted(int waveNumber)
    {
        Debug.Log($"Wave {waveNumber} completed!");
        UpdateWaveUI($"Wave {waveNumber} Complete!");
    }
    
    public void OnAllWavesComplete()
    {
        State = GameState.Victory;
    }
    
    /// <summary>
    /// OOP PRINCIPLE: SINGLE RESPONSIBILITY
    /// UI update logic separated into dedicated methods
    /// </summary>
    private void UpdateWaveUI(string text)
    {
        if (waveText != null)
        {
            waveText.text = text;
        }
    }
    
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {_score}";
        }
    }
}

