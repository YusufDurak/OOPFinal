using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: SINGLETON PATTERN (Creational Design Pattern)
/// Ensures only one instance of GameManager exists throughout the game lifecycle.
/// Provides global access point while maintaining encapsulation.
/// This prevents multiple conflicting game states and centralizes game-level logic.
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
        GameOver
    }
    
    // Private fields with public properties (Encapsulation)
    private int _score;
    private float _currentTime;
    private GameState _gameState;
    
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
            // Could trigger UI updates here
            Debug.Log($"Score updated: {_score}");
        }
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
}

