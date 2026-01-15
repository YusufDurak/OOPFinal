using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: INHERITANCE & POLYMORPHISM
/// ChaseState inherits from EnemyState and provides concrete implementation.
/// This demonstrates polymorphism - different state objects can be treated uniformly
/// through the base EnemyState reference, but execute different behaviors.
/// </summary>
public class ChaseState : EnemyState
{
    private EnemyController _enemy;
    private Transform _player;
    private float _moveSpeed = 3f;
    private float _attackRange = 2f;
    
    /// <summary>
    /// OOP PRINCIPLE: POLYMORPHISM (Method Overriding)
    /// Overrides the abstract method from base class with specific chase behavior.
    /// </summary>
    public override void EnterState(EnemyController enemy)
    {
        _enemy = enemy;
        
        // Find player reference
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
        
        Debug.Log($"{enemy.gameObject.name} entered Chase State");
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// State logic is encapsulated within this class, hiding implementation details.
    /// </summary>
    public override void UpdateState()
    {
        if (_player == null || _enemy == null) return;
        
        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _player.position);
        
        // STATE TRANSITION LOGIC
        if (distanceToPlayer <= _attackRange)
        {
            // Switch to Attack State when close enough
            _enemy.SwitchState(new AttackState());
        }
        else
        {
            // Move towards player
            Vector3 direction = (_player.position - _enemy.transform.position).normalized;
            Vector3 newPosition = Vector3.MoveTowards(
                _enemy.transform.position,
                _player.position,
                _moveSpeed * Time.deltaTime
            );
            
            _enemy.transform.position = newPosition;
            
            // Optional: Rotate to face player
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
    }
    
    public override void ExitState()
    {
        Debug.Log($"{_enemy.gameObject.name} exited Chase State");
    }
}

