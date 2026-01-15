using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: INTERFACE IMPLEMENTATION
/// Implements IDamageable to handle damage from projectiles and other sources.
/// 
/// OOP PRINCIPLE: STATE PATTERN (Context Class)
/// EnemyController acts as the "Context" in the State pattern.
/// It delegates behavior to the current state object, allowing dynamic behavior changes.
/// 
/// OOP PRINCIPLE: COMPOSITION OVER INHERITANCE
/// Instead of creating EnemyChase, EnemyAttack subclasses, we use state composition.
/// This provides more flexibility and avoids deep inheritance hierarchies.
/// </summary>
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int scoreValue = 10;
    
    [Header("Pool Settings")]
    [SerializeField] private string poolTag = "Enemy";
    
    /// OOP PRINCIPLE: ENCAPSULATION
    /// State is private but behavior is public through controlled methods
    private EnemyState _currentState;
    private int _currentHealth;
    
    public int CurrentHealth => _currentHealth;
    public Transform PlayerTransform { get; private set; }
    
    private void Awake()
    {
        _currentHealth = maxHealth;
    }
    
    private void OnEnable()
    {
        // Reset health when retrieved from pool
        _currentHealth = maxHealth;
        
        // Find player reference
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerTransform = player.transform;
        }
        
        // Initialize starting state
        SwitchState(new ChaseState());
    }
    
    private void Update()
    {
        /// OOP PRINCIPLE: DELEGATION & STATE PATTERN
        /// The enemy delegates its behavior to the current state.
        /// This is a form of behavioral delegation - the enemy doesn't implement
        /// chase/attack logic itself, it delegates to specialized state objects.
        if (_currentState != null)
        {
            _currentState.UpdateState();
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: STATE PATTERN (State Transition)
    /// Public method to switch between states.
    /// This demonstrates how the Context (EnemyController) manages state transitions
    /// while states themselves can also trigger transitions.
    /// </summary>
    public void SwitchState(EnemyState newState)
    {
        // Exit current state
        if (_currentState != null)
        {
            _currentState.ExitState();
        }
        
        // Switch to new state
        _currentState = newState;
        
        // Enter new state
        if (_currentState != null)
        {
            _currentState.EnterState(this);
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: INTERFACE IMPLEMENTATION (IDamageable)
    /// Fulfills the IDamageable contract.
    /// ANY object with an IDamageable reference can damage this enemy.
    /// </summary>
    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        
        Debug.Log($"{gameObject.name} took {amount} damage! Health: {_currentHealth}/{maxHealth}");
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Death logic is private and can only be triggered through TakeDamage.
    /// Handles cleanup, scoring, and object pooling.
    /// 
    /// OOP PRINCIPLE: EVENT-DRIVEN ARCHITECTURE
    /// Notifies GameManager of death BEFORE returning to pool.
    /// This allows the wave system to track enemy count accurately.
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        
        // Notify GameManager of death for wave tracking
        /// OOP PRINCIPLE: MEDIATOR PATTERN
        /// EnemyController doesn't know about EnemySpawner.
        /// It only communicates with GameManager (the mediator),
        /// which handles forwarding the event to interested parties.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
            GameManager.Instance.OnEnemyKilled(); // NEW: Wave system tracking
        }
        
        // Return to pool instead of destroying
        /// OOP PRINCIPLE: OBJECT POOLING
        /// Reuses objects for better performance
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    /// <summary>
    /// Visualization helper for debugging (optional)
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw a line to the player when selected
        if (PlayerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, PlayerTransform.position);
        }
    }
}

