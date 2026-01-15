using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: SINGLE RESPONSIBILITY PRINCIPLE
/// This class has ONE job: manage projectile behavior (movement, collision, pooling).
/// It doesn't handle player input, enemy AI, or game state - maintaining clean separation.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private int damage = 25;
    [SerializeField] private float lifetime = 5f;
    
    private float _spawnTime;
    private Rigidbody _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    private void OnEnable()
    {
        // Reset spawn time when retrieved from pool
        _spawnTime = Time.time;
    }
    
    private void Update()
    {
        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;
        
        // Return to pool after lifetime expires
        if (Time.time >= _spawnTime + lifetime)
        {
            ReturnToPool();
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: INTERFACE-BASED PROGRAMMING & POLYMORPHISM
    /// Uses GetComponent<IDamageable>() to check if the colliding object can take damage.
    /// This is MUCH better than using tags because:
    /// 1. It's type-safe
    /// 2. Works with ANY object implementing IDamageable
    /// 3. Doesn't rely on string comparisons (error-prone)
    /// 4. Demonstrates loose coupling - projectile doesn't need to know specific class types
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Try to get IDamageable component from the collided object
        IDamageable damageable = other.GetComponent<IDamageable>();
        
        if (damageable != null)
        {
            // Object can take damage - apply it
            damageable.TakeDamage(damage);
            Debug.Log($"Projectile hit {other.gameObject.name} for {damage} damage");
            
            // Return projectile to pool on hit
            ReturnToPool();
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION & ABSTRACTION
    /// Hides the complexity of object pooling from other classes.
    /// External code doesn't need to know about ObjectPoolManager.
    /// </summary>
    private void ReturnToPool()
    {
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(this.gameObject);
        }
        else
        {
            // Fallback if pool manager doesn't exist
            Destroy(this.gameObject);
        }
    }
}

