using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: ABSTRACTION (Abstract Class)
/// This abstract class defines the structure for all enemy states without implementation.
/// Forces derived classes to implement specific behavior while providing a common interface.
/// 
/// OOP PRINCIPLE: STATE PATTERN (Behavioral Design Pattern)
/// Allows an object (Enemy) to alter its behavior when its internal state changes.
/// The enemy will appear to change its class, encapsulating state-specific behavior.
/// </summary>
public abstract class EnemyState
{
    /// <summary>
    /// OOP PRINCIPLE: POLYMORPHISM
    /// Each derived state class will provide its own implementation of these methods.
    /// This allows different behaviors while maintaining a consistent interface.
    /// </summary>
    
    /// <summary>
    /// Called when entering this state
    /// </summary>
    public abstract void EnterState(EnemyController enemy);
    
    /// <summary>
    /// Called every frame while in this state
    /// </summary>
    public abstract void UpdateState();
    
    /// <summary>
    /// Called when exiting this state
    /// </summary>
    public abstract void ExitState();
}

