using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: SINGLE RESPONSIBILITY PRINCIPLE
/// This class has ONE responsibility: spawning enemies at intervals.
/// It doesn't manage enemy behavior, game state, or object pools directly.
/// 
/// OOP PRINCIPLE: DEPENDENCY INVERSION
/// Depends on ObjectPoolManager abstraction rather than concrete enemy creation.
/// This makes the spawner flexible and testable.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private string enemyPoolTag = "Enemy";
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private int maxEnemies = 10;
    
    [Header("References")]
    [SerializeField] private Transform player;
    
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Private fields track internal state without exposing implementation details
    private float _lastSpawnTime;
    private int _currentEnemyCount;
    
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
        
        _lastSpawnTime = -spawnInterval; // Allow immediate first spawn
    }
    
    private void Update()
    {
        // Only spawn if game is playing
        if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing)
        {
            return;
        }
        
        // Check if it's time to spawn and we haven't hit the limit
        if (Time.time >= _lastSpawnTime + spawnInterval && _currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
            _lastSpawnTime = Time.time;
        }
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
            _currentEnemyCount++;
            Debug.Log($"Enemy spawned! Total: {_currentEnemyCount}/{maxEnemies}");
            
            // Subscribe to enemy death to track count (would need event system for production)
            // For now, we'll rely on manual tracking
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
    /// Public method to track when enemies are returned to pool
    /// Call this from enemy death events or decrement counter
    /// </summary>
    public void OnEnemyDestroyed()
    {
        _currentEnemyCount--;
        _currentEnemyCount = Mathf.Max(_currentEnemyCount, 0);
    }
    
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

