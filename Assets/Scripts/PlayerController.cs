using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: INTERFACE IMPLEMENTATION
/// PlayerController implements IDamageable, making it compatible with any damage system.
/// This demonstrates the power of interfaces - the player can be damaged by bullets,
/// enemies, traps, or any other damage source without tight coupling.
/// 
/// OOP PRINCIPLE: SINGLE RESPONSIBILITY
/// Handles ONLY player-specific logic: movement, aiming, shooting, and taking damage.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Combat Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private string bulletPoolTag = "Bullet";
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Private fields with controlled access through methods/properties
    private int _currentHealth;
    private Rigidbody _rb;
    private float _lastFireTime;
    
    public int CurrentHealth => _currentHealth; // Read-only property
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _currentHealth = maxHealth;
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (firePoint == null)
        {
            Debug.LogWarning("FirePoint not assigned! Creating default at player position.");
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(this.transform);
            firePointObj.transform.localPosition = Vector3.forward;
            firePoint = firePointObj.transform;
        }
    }
    
    private void Update()
    {
        HandleAiming();
        HandleShooting();
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Movement logic is encapsulated in its own method, hiding implementation details.
    /// </summary>
    private void HandleMovement()
    {
        // WASD input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        
        // Apply velocity to Rigidbody
        _rb.linearVelocity = movement * moveSpeed;
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Aiming logic is self-contained and reusable.
    /// </summary>
    private void HandleAiming()
    {
        if (mainCamera == null) return;
        
        // Cast ray from camera to mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Raycast on a specific layer or use a ground plane
        if (Physics.Raycast(ray, out hit, 100f))
        {
            // Calculate direction to look at
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0f; // Keep player upright
            
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation;
            }
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: DEPENDENCY INVERSION & ABSTRACTION
    /// This method depends on the ObjectPoolManager abstraction, not concrete implementations.
    /// It doesn't care HOW bullets are created, just that they come from the pool.
    /// </summary>
    private void HandleShooting()
    {
        // Check for mouse click and fire rate
        if (Input.GetMouseButton(0) && Time.time >= _lastFireTime + fireRate)
        {
            Shoot();
            _lastFireTime = Time.time;
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ABSTRACTION
    /// Shooting logic is abstracted away from input handling.
    /// Could be called from different sources (mouse, button, AI, etc.)
    /// </summary>
    private void Shoot()
    {
        if (ObjectPoolManager.Instance == null)
        {
            Debug.LogWarning("ObjectPoolManager not found! Cannot shoot.");
            return;
        }
        
        // Get bullet from pool
        GameObject bullet = ObjectPoolManager.Instance.GetFromPool(
            bulletPoolTag,
            firePoint.position,
            firePoint.rotation
        );
        
        if (bullet != null)
        {
            Debug.Log("Player fired a bullet!");
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: INTERFACE IMPLEMENTATION (IDamageable)
    /// This method fulfills the contract defined by IDamageable interface.
    /// ANY object with a reference to IDamageable can damage the player,
    /// demonstrating loose coupling and flexibility.
    /// </summary>
    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0); // Clamp to 0
        
        Debug.Log($"Player took {amount} damage! Health: {_currentHealth}/{maxHealth}");
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Death logic is private and controlled, can only be triggered through TakeDamage.
    /// </summary>
    private void Die()
    {
        Debug.Log("Player died!");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }
        
        // Could trigger death animation, respawn, etc.
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Public method for healing (demonstrates encapsulation with validation)
    /// </summary>
    public void Heal(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, maxHealth); // Clamp to max
        Debug.Log($"Player healed {amount}! Health: {_currentHealth}/{maxHealth}");
    }
}

