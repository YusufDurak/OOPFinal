using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: COMPOSITION
/// Wave class is a data structure that composes the wave configuration.
/// Serializable allows Inspector editing while maintaining encapsulation.
/// </summary>
[System.Serializable]
public class Wave
{
    public string waveName = "Wave 1";
    public int enemyCount = 5;
    public float spawnRate = 2f;
    
    /// <summary>
    /// Constructor for easy wave creation in code
    /// </summary>
    public Wave(string name, int count, float rate)
    {
        waveName = name;
        enemyCount = count;
        spawnRate = rate;
    }
}

/// <summary>
/// OOP PRINCIPLE: SINGLE RESPONSIBILITY PRINCIPLE
/// This class has ONE responsibility: spawning enemies in waves.
/// It doesn't manage enemy behavior, game state, or object pools directly.
/// 
/// OOP PRINCIPLE: DEPENDENCY INVERSION
/// Depends on ObjectPoolManager abstraction rather than concrete enemy creation.
/// This makes the spawner flexible and testable.
/// 
/// OOP PRINCIPLE: EVENT-DRIVEN ARCHITECTURE
/// Uses callbacks from GameManager to track enemy deaths instead of polling.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    
    [Header("Spawn Settings")]
    [SerializeField] private string enemyPoolTag = "Enemy";
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float timeBetweenWaves = 5f;
    
    [Header("References")]
    [SerializeField] private Transform player;
    
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Private fields track internal state without exposing implementation details
    private int _currentWaveIndex = 0;
    private int _enemiesSpawnedThisWave = 0;
    private int _enemiesAliveThisWave = 0;
    private float _lastSpawnTime;
    private bool _isWaveActive = false;
    private float _waveEndTime;
    
    private void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Player not found! EnemySpawner needs a player reference.");
            }
        }
        
        // Create default waves if none configured
        if (waves == null || waves.Count == 0)
        {
            CreateDefaultWaves();
        }
        
        // Register with GameManager for enemy death notifications
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterSpawner(this);
        }
        
        // Start first wave
        StartWave();
    }
    
    private void Update()
    {
        // Only operate if game is playing
        if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing)
        {
            return;
        }
        
        /// OOP PRINCIPLE: STATE MACHINE (Simple)
        /// Wave spawning follows a state: spawning -> waiting for clear -> next wave
        if (_isWaveActive)
        {
            UpdateWaveSpawning();
        }
        else
        {
            // Wait between waves
            if (Time.time >= _waveEndTime && _currentWaveIndex < waves.Count)
            {
                StartWave();
            }
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Wave spawning logic encapsulated in dedicated method
    /// </summary>
    private void UpdateWaveSpawning()
    {
        Wave currentWave = waves[_currentWaveIndex];
        
        // Check if we've spawned all enemies for this wave
        if (_enemiesSpawnedThisWave >= currentWave.enemyCount)
        {
            // Check if all enemies are defeated
            if (_enemiesAliveThisWave <= 0)
            {
                EndWave();
            }
            return;
        }
        
        // Spawn enemies at the wave's spawn rate
        if (Time.time >= _lastSpawnTime + currentWave.spawnRate)
        {
            SpawnEnemy();
            _lastSpawnTime = Time.time;
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ABSTRACTION
    /// Hides the complexity of starting a new wave
    /// </summary>
    private void StartWave()
    {
        if (_currentWaveIndex >= waves.Count)
        {
            // All waves completed!
            Debug.Log("All waves completed! Victory!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnAllWavesComplete();
            }
            return;
        }
        
        Wave wave = waves[_currentWaveIndex];
        _isWaveActive = true;
        _enemiesSpawnedThisWave = 0;
        _enemiesAliveThisWave = 0;
        _lastSpawnTime = Time.time;
        
        Debug.Log($"Starting {wave.waveName} - Enemies: {wave.enemyCount}");
        
        // Notify GameManager of new wave
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveStarted(_currentWaveIndex + 1, wave.waveName);
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ABSTRACTION
    /// Encapsulates wave completion logic
    /// </summary>
    private void EndWave()
    {
        _isWaveActive = false;
        _waveEndTime = Time.time + timeBetweenWaves;
        
        Debug.Log($"{waves[_currentWaveIndex].waveName} completed!");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveCompleted(_currentWaveIndex + 1);
        }
        
        _currentWaveIndex++;
    }
    
    /// <summary>
    /// Creates default wave progression if none configured
    /// </summary>
    private void CreateDefaultWaves()
    {
        waves = new List<Wave>
        {
            new Wave("Wave 1", 5, 2f),
            new Wave("Wave 2", 8, 1.5f),
            new Wave("Wave 3", 12, 1.2f),
            new Wave("Wave 4", 15, 1f),
            new Wave("Wave 5 - BOSS WAVE", 20, 0.8f)
        };
        Debug.Log("Default waves created");
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ABSTRACTION
    /// Spawning logic is abstracted into its own method.
    /// Hides complexity of position calculation and pool retrieval.
    /// </summary>
    private void SpawnEnemy()
    {
        if (ObjectPoolManager.Instance == null)
        {
            Debug.LogWarning("ObjectPoolManager not found! Cannot spawn enemy.");
            return;
        }
        
        if (player == null)
        {
            Debug.LogWarning("Player reference is null! Cannot spawn enemy.");
            return;
        }
        
        // Calculate random spawn position around the player
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        /// OOP PRINCIPLE: DEPENDENCY INVERSION & ABSTRACTION
        /// We depend on the ObjectPoolManager interface, not concrete instantiation.
        /// This decouples spawning from object creation.
        GameObject enemy = ObjectPoolManager.Instance.GetFromPool(
            enemyPoolTag,
            spawnPosition,
            Quaternion.identity
        );
        
        if (enemy != null)
        {
            _enemiesSpawnedThisWave++;
            _enemiesAliveThisWave++;
            
            Debug.Log($"Enemy spawned! Wave progress: {_enemiesSpawnedThisWave}/{waves[_currentWaveIndex].enemyCount}");
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION & ABSTRACTION
    /// Position calculation logic is hidden in a private method.
    /// Makes the code more readable and maintainable.
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        // Generate random angle
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        
        // Calculate position on a circle around the player
        float x = Mathf.Cos(angle) * spawnRadius;
        float z = Mathf.Sin(angle) * spawnRadius;
        
        Vector3 spawnPos = player.position + new Vector3(x, 0f, z);
        
        return spawnPos;
    }
    
    /// <summary>
    /// OOP PRINCIPLE: EVENT-DRIVEN ARCHITECTURE
    /// Called by GameManager when an enemy dies.
    /// This is better than FindObjectsOfType() - uses manager communication pattern.
    /// </summary>
    public void OnEnemyKilled()
    {
        _enemiesAliveThisWave--;
        _enemiesAliveThisWave = Mathf.Max(_enemiesAliveThisWave, 0);
        
        Debug.Log($"Enemy killed! Remaining: {_enemiesAliveThisWave}");
        
        // Check if wave is complete (all spawned and all killed)
        if (_enemiesSpawnedThisWave >= waves[_currentWaveIndex].enemyCount && _enemiesAliveThisWave <= 0)
        {
            EndWave();
        }
    }
    
    /// <summary>
    /// Public getter for current wave information (Encapsulation)
    /// </summary>
    public int CurrentWaveNumber => _currentWaveIndex + 1;
    public string CurrentWaveName => _currentWaveIndex < waves.Count ? waves[_currentWaveIndex].waveName : "Complete";
    public bool IsWaveActive => _isWaveActive;
    
    /// <summary>
    /// Visualization helper for debugging
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        
        // Draw spawn radius
        Gizmos.color = Color.yellow;
        DrawCircle(player.position, spawnRadius, 32);
    }
    
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}

