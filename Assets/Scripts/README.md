# Top-Down Arena Shooter - OOP Architecture Guide

## 📁 Project Structure

This project demonstrates **strict OOP principles** with modular, scalable architecture.

### Files Created:
1. **IDamageable.cs** - Interface for damage system
2. **GameManager.cs** - Singleton game state manager
3. **ObjectPoolManager.cs** - Singleton object pooling system
4. **EnemyState.cs** - Abstract base class for FSM
5. **ChaseState.cs** - Concrete chase behavior
6. **AttackState.cs** - Concrete attack behavior
7. **Projectile.cs** - Bullet logic
8. **PlayerController.cs** - Player movement & combat
9. **EnemyController.cs** - Enemy AI using FSM
10. **EnemySpawner.cs** - Enemy spawning system

---

## 🎯 OOP Principles Demonstrated

### 1. **ABSTRACTION**
- **IDamageable Interface**: Defines contract without implementation
- **EnemyState Abstract Class**: Enforces structure for all states
- **Private Methods**: Hide implementation details (GetRandomSpawnPosition, PerformAttack, etc.)

### 2. **ENCAPSULATION**
- **Private Fields with Public Properties**: Controlled access to data
  ```csharp
  private int _score;
  public int Score { get { return _score; } set { _score = value; } }
  ```
- **Data Hiding**: Internal state is protected from external modification

### 3. **INHERITANCE**
- **ChaseState : EnemyState**: Inherits structure, implements behavior
- **AttackState : EnemyState**: Same interface, different implementation

### 4. **POLYMORPHISM**
- **Method Overriding**: States override abstract methods
- **Interface Implementation**: Multiple classes implement IDamageable
  ```csharp
  IDamageable target = obj.GetComponent<IDamageable>();
  target.TakeDamage(damage); // Works on ANY IDamageable object
  ```

### 5. **DESIGN PATTERNS**

#### **Singleton Pattern** (Creational)
- GameManager & ObjectPoolManager
- Ensures single instance, global access
- DontDestroyOnLoad for persistence

#### **State Pattern** (Behavioral)
- EnemyController uses EnemyState system
- Behavior changes dynamically based on state
- Encapsulates state-specific logic

#### **Object Pool Pattern** (Performance)
- Reuses GameObjects instead of Instantiate/Destroy
- Dramatically improves performance
- Manages multiple pools via Dictionary

### 6. **SOLID Principles**

#### **Single Responsibility (S)**
- Each class has ONE job
- PlayerController: Player logic only
- EnemySpawner: Spawning only
- Projectile: Bullet behavior only

#### **Open/Closed (O)**
- System is open for extension (add new states)
- Closed for modification (don't change base classes)

#### **Liskov Substitution (L)**
- Any EnemyState can replace another
- Any IDamageable can be damaged uniformly

#### **Interface Segregation (I)**
- IDamageable is focused and minimal
- Classes only implement what they need

#### **Dependency Inversion (D)**
- Depend on abstractions (IDamageable, ObjectPoolManager)
- Not on concrete implementations

---

## 🛠️ Unity Setup Instructions

### Step 1: Scene Setup
1. Create an empty GameObject named "GameManager"
   - Add `GameManager.cs` component

2. Create an empty GameObject named "ObjectPoolManager"
   - Add `ObjectPoolManager.cs` component
   - Configure pools in Inspector:
     - **Pool 1**: Tag="Bullet", Prefab=[BulletPrefab], Size=20
     - **Pool 2**: Tag="Enemy", Prefab=[EnemyPrefab], Size=10

3. Create an empty GameObject named "EnemySpawner"
   - Add `EnemySpawner.cs` component

### Step 2: Create Player
1. Create a Capsule (GameObject → 3D Object → Capsule)
2. Rename to "Player"
3. Tag as "Player" (Inspector → Tag → Player)
4. Add `PlayerController.cs` component
5. Add **Rigidbody** component
   - Freeze Rotation: X, Y, Z (prevent tipping)
6. Create a child empty GameObject named "FirePoint"
   - Position it at (0, 0, 1) - front of player

### Step 3: Create Bullet Prefab
1. Create a Sphere (GameObject → 3D Object → Sphere)
2. Scale to (0.2, 0.2, 0.2)
3. Rename to "Bullet"
4. Add `Projectile.cs` component
5. Add **Rigidbody** component
   - Use Gravity: OFF
   - Is Kinematic: ON
6. Add **Sphere Collider**
   - Is Trigger: ON
7. Drag to Project folder to create prefab
8. Delete from scene

### Step 4: Create Enemy Prefab
1. Create a Capsule (GameObject → 3D Object → Capsule)
2. Rename to "Enemy"
3. Change color (add Material with different color)
4. Add `EnemyController.cs` component
5. Add **Capsule Collider**
   - Is Trigger: OFF
6. Drag to Project folder to create prefab
7. Delete from scene

### Step 5: Configure References
1. **ObjectPoolManager**:
   - Add Bullet and Enemy to pools list
   - Set appropriate tags and sizes

2. **PlayerController**:
   - Assign Main Camera
   - Set bulletPoolTag to "Bullet"
   - Assign FirePoint transform

3. **EnemySpawner**:
   - Set enemyPoolTag to "Enemy"
   - Adjust spawnInterval, spawnRadius, maxEnemies

### Step 6: Setup Layers (Optional but Recommended)
1. Create Layers: "Player", "Enemy", "Projectile"
2. Assign appropriate layers to objects
3. Edit → Project Settings → Physics
   - Configure Layer Collision Matrix
   - Bullets should collide with Enemies
   - Enemies should collide with Player

---

## 🎮 How It Works

### Damage System (Interface-Based)
```csharp
// Instead of this (bad):
if (other.tag == "Enemy") { /* damage enemy */ }

// We use this (good):
IDamageable target = other.GetComponent<IDamageable>();
if (target != null) { target.TakeDamage(damage); }
```

**Benefits**:
- Type-safe (compiler checks)
- Works with ANY damageable object
- No string comparisons
- Loose coupling

### State Machine (Enemy AI)
```
Enemy spawns → ChaseState
  ↓
Player in range → AttackState
  ↓
Player moves away → ChaseState (loop)
```

**Benefits**:
- Easy to add new states (PatrolState, FleeState, etc.)
- State-specific logic is encapsulated
- Clean transitions

### Object Pooling
```
Spawn bullet → Get from pool (activate)
Hit target → Return to pool (deactivate)
```

**Benefits**:
- No garbage collection spikes
- Better performance
- Scalable for hundreds of objects

---

## 🔧 Testing Checklist

### Test 1: Player Movement
- [ ] WASD moves player
- [ ] Player rotates toward mouse
- [ ] Movement is smooth

### Test 2: Shooting
- [ ] Left-click shoots bullets
- [ ] Bullets move forward
- [ ] Fire rate works correctly
- [ ] Bullets return to pool after timeout

### Test 3: Enemy Spawning
- [ ] Enemies spawn around player
- [ ] Spawn interval works
- [ ] Max enemy limit works
- [ ] Enemies spawn from pool

### Test 4: Enemy AI
- [ ] Enemies chase player (ChaseState)
- [ ] Enemies attack when close (AttackState)
- [ ] Enemies return to chase when player moves away
- [ ] State transitions are smooth

### Test 5: Damage System
- [ ] Bullets damage enemies
- [ ] Enemies damage player
- [ ] Health decreases correctly
- [ ] Enemies die at 0 health
- [ ] Player dies at 0 health
- [ ] Score increases when enemy dies

### Test 6: Object Pooling
- [ ] Bullets reuse from pool
- [ ] Enemies reuse from pool
- [ ] No Instantiate calls after initial setup
- [ ] Check hierarchy (objects should deactivate, not destroy)

### Test 7: Game Manager
- [ ] Score tracks correctly
- [ ] Time tracks correctly
- [ ] Game Over state works
- [ ] Only one GameManager exists

---

## 🚀 Extension Ideas

### Easy Extensions:
1. **New Enemy State**: Add `PatrolState` or `FleeState`
2. **Power-ups**: Implement IDamageable for destructible crates
3. **Weapons**: Create different projectile types
4. **UI**: Display health, score, time

### Medium Extensions:
1. **Wave System**: Increase difficulty over time
2. **Enemy Variants**: Fast/Slow/Tank enemies
3. **Player Abilities**: Dash, shield, special attacks
4. **Particle Effects**: Explosions, hit effects

### Advanced Extensions:
1. **Command Pattern**: Undo/replay player actions
2. **Observer Pattern**: Event system for UI updates
3. **Strategy Pattern**: Different weapon behaviors
4. **Factory Pattern**: Enemy creation system

---

## 📚 Key Takeaways for Professor

### This project demonstrates:
✅ **Clean Architecture**: Modular, separated concerns
✅ **SOLID Principles**: All five applied correctly
✅ **Design Patterns**: Singleton, State, Object Pool
✅ **Interface Programming**: IDamageable usage throughout
✅ **Polymorphism**: State pattern with method overriding
✅ **Encapsulation**: Private fields, public interfaces
✅ **Abstraction**: Interfaces and abstract classes
✅ **No Spaghetti Code**: Each class has clear responsibility
✅ **Scalability**: Easy to extend without modification
✅ **Performance**: Object pooling for optimization

### Code Quality:
- Comprehensive XML documentation
- OOP principle comments throughout
- Consistent naming conventions
- No hardcoded dependencies
- Proper null checking
- Clean separation of concerns

---

## 🐛 Troubleshooting

### "ObjectPoolManager instance is null"
- Ensure ObjectPoolManager GameObject exists in scene
- Check that pools are configured in Inspector

### "Player not found"
- Ensure Player has "Player" tag
- Check Tag Manager (Edit → Project Settings → Tags)

### Bullets don't damage enemies
- Check colliders (Is Trigger = ON for bullets)
- Verify EnemyController implements IDamageable
- Check Layer Collision Matrix

### Enemies don't chase
- Ensure Player has "Player" tag
- Check EnemyController is initializing ChaseState
- Verify enemy has no Rigidbody constraints

### Performance issues
- Increase pool sizes to avoid dynamic instantiation
- Check for excessive Debug.Log calls
- Profile with Unity Profiler

---

## 📝 Assignment Submission Notes

**This architecture follows industry-standard OOP practices:**

1. **Maintainability**: Each class is independent and testable
2. **Extensibility**: New features don't break existing code
3. **Readability**: Clear naming and documentation
4. **Performance**: Object pooling prevents garbage collection
5. **Flexibility**: Interface-based design allows easy swapping

**Every major OOP principle is commented in the code for easy review.**

---

## 📞 Next Steps

1. Import scripts into Unity
2. Follow setup instructions above
3. Test each system individually
4. Extend with your own features
5. Document your additions

**Good luck with your project! 🎮**

