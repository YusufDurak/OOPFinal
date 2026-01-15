using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: INHERITANCE & POLYMORPHISM
/// AttackState inherits from EnemyState, demonstrating how different states
/// can share a common interface but implement completely different behaviors.
/// </summary>
public class AttackState : EnemyState
{
    private EnemyController _enemy;
    private Transform _player;
    private IDamageable _playerDamageable;
    private float _attackRange = 2f;
    private float _attackCooldown = 1f;
    private float _lastAttackTime;
    private int _attackDamage = 10;
    
    /// <summary>
    /// OOP PRINCIPLE: POLYMORPHISM (Method Overriding)
    /// Provides specific attack behavior implementation.
    /// </summary>
    public override void EnterState(EnemyController enemy)
    {
        _enemy = enemy;
        _lastAttackTime = -_attackCooldown; // Allow immediate first attack
        
        // Find player and cache IDamageable interface
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            
            /// OOP PRINCIPLE: INTERFACE-BASED PROGRAMMING
            /// Uses GetComponent<IDamageable>() instead of hard-coding player class.
            /// This allows ANY object implementing IDamageable to be attacked,
            /// demonstrating loose coupling and flexibility.
            _playerDamageable = playerObj.GetComponent<IDamageable>();
        }
        
        Debug.Log($"{enemy.gameObject.name} entered Attack State");
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Attack logic is self-contained and hidden from other classes.
    /// </summary>
    public override void UpdateState()
    {
        if (_player == null || _enemy == null) return;
        
        float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _player.position);
        
        // STATE TRANSITION LOGIC
        if (distanceToPlayer > _attackRange)
        {
            // Player moved away, switch back to Chase State
            _enemy.SwitchState(new ChaseState());
            return;
        }
        
        // Perform attack with cooldown
        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            PerformAttack();
            _lastAttackTime = Time.time;
        }
        
        // Face the player while attacking
        Vector3 direction = (_player.position - _enemy.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            _enemy.transform.rotation = Quaternion.Slerp(
                _enemy.transform.rotation,
                lookRotation,
                Time.deltaTime * 5f
            );
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ABSTRACTION & INTERFACE SEGREGATION
    /// This method doesn't need to know what type of object it's attacking.
    /// It only needs to know that the object implements IDamageable.
    /// This is the power of programming to interfaces!
    /// </summary>
    private void PerformAttack()
    {
        if (_playerDamageable != null)
        {
            _playerDamageable.TakeDamage(_attackDamage);
            Debug.Log($"{_enemy.gameObject.name} attacked player for {_attackDamage} damage!");
        }
    }
    
    public override void ExitState()
    {
        Debug.Log($"{_enemy.gameObject.name} exited Attack State");
    }
}

