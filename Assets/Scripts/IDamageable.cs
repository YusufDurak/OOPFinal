using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: ABSTRACTION & INTERFACE SEGREGATION
/// This interface defines a contract for any object that can take damage.
/// By using an interface, we achieve loose coupling - any class can implement this
/// without inheriting from a specific base class.
/// </summary>
public interface IDamageable
{
    void TakeDamage(int amount);
}

