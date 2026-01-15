# Architecture Diagram & Relationships

## 🏗️ System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         GAME SCENE                              │
│                                                                 │
│  ┌──────────────┐  ┌────────────────────┐  ┌───────────────┐  │
│  │ GameManager  │  │ ObjectPoolManager  │  │ EnemySpawner  │  │
│  │ (Singleton)  │  │    (Singleton)     │  │               │  │
│  └──────────────┘  └────────────────────┘  └───────────────┘  │
│         │                    │                      │          │
│         └────────┬───────────┴──────────────────────┘          │
│                  │                                              │
│                  ▼                                              │
│  ┌───────────────────────────────────────────────────────────┐ │
│  │                    GAME OBJECTS                           │ │
│  │                                                           │ │
│  │  ┌──────────────┐         ┌──────────────┐              │ │
│  │  │    Player    │         │    Enemy     │              │ │
│  │  │              │◄────────┤              │              │ │
│  │  │ implements   │ targets │ implements   │              │ │
│  │  │ IDamageable  │         │ IDamageable  │              │ │
│  │  └──────────────┘         └──────────────┘              │ │
│  │         │                        │                       │ │
│  │         │ shoots                 │ uses                  │ │
│  │         ▼                        ▼                       │ │
│  │  ┌──────────────┐         ┌──────────────┐              │ │
│  │  │  Projectile  │         │ State Machine│              │ │
│  │  │              │         │  - ChaseState│              │ │
│  │  │ damages      │         │  - AttackState              │ │
│  │  │ IDamageable  │         └──────────────┘              │ │
│  │  └──────────────┘                                       │ │
│  └───────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔗 Class Relationships

### Inheritance Hierarchy

```
Object (Unity)
    │
    ├── MonoBehaviour
    │       │
    │       ├── GameManager (Singleton)
    │       ├── ObjectPoolManager (Singleton)
    │       ├── PlayerController (implements IDamageable)
    │       ├── EnemyController (implements IDamageable)
    │       ├── EnemySpawner
    │       └── Projectile
    │
    └── EnemyState (Abstract)
            │
            ├── ChaseState
            └── AttackState
```

### Interface Implementation

```
IDamageable (Interface)
    │
    ├── PlayerController
    └── EnemyController
```

---

## 🎯 Interaction Flow

### 1. Game Initialization Flow

```
Unity Scene Load
    │
    ├──► GameManager.Awake()
    │       └──► Initialize Singleton
    │       └──► Set GameState = Playing
    │
    ├──► ObjectPoolManager.Awake()
    │       └──► Initialize Singleton
    │       └──► Create Pools (Bullets, Enemies)
    │
    ├──► Player.Awake()
    │       └──► Initialize Components
    │       └──► Set Health = MaxHealth
    │
    └──► EnemySpawner.Start()
            └──► Find Player Reference
            └──► Start Spawn Timer
```

### 2. Player Shooting Flow

```
Player Input (Mouse Click)
    │
    ├──► PlayerController.HandleShooting()
    │       │
    │       └──► Check Fire Rate
    │               │
    │               └──► PlayerController.Shoot()
    │                       │
    │                       └──► ObjectPoolManager.GetFromPool("Bullet")
    │                               │
    │                               └──► Return Activated Bullet GameObject
    │                                       │
    │                                       └──► Projectile.OnEnable()
    │                                               └──► Reset Spawn Time
    │                                               └──► Move Forward
```

### 3. Bullet Collision Flow

```
Projectile Collision (OnTriggerEnter)
    │
    ├──► Get IDamageable Component
    │       │
    │       ├──► Found Enemy?
    │       │       │
    │       │       └──► EnemyController.TakeDamage()
    │       │               │
    │       │               ├──► Reduce Health
    │       │               │
    │       │               └──► Health <= 0?
    │       │                       │
    │       │                       └──► EnemyController.Die()
    │       │                               │
    │       │                               ├──► GameManager.AddScore()
    │       │                               │
    │       │                               └──► ObjectPoolManager.ReturnToPool()
    │       │
    │       └──► Found Player?
    │               │
    │               └──► PlayerController.TakeDamage()
    │                       │
    │                       ├──► Reduce Health
    │                       │
    │                       └──► Health <= 0?
    │                               │
    │                               └──► PlayerController.Die()
    │                                       └──► GameManager.EndGame()
    │
    └──► ObjectPoolManager.ReturnToPool(bullet)
```

### 4. Enemy Spawning Flow

```
Time.deltaTime (Every Frame)
    │
    └──► EnemySpawner.Update()
            │
            └──► Check Spawn Interval
                    │
                    └──► EnemySpawner.SpawnEnemy()
                            │
                            ├──► Calculate Random Position
                            │
                            └──► ObjectPoolManager.GetFromPool("Enemy")
                                    │
                                    └──► EnemyController.OnEnable()
                                            │
                                            ├──► Reset Health
                                            ├──► Find Player
                                            └──► SwitchState(ChaseState)
```

### 5. Enemy AI State Machine Flow

```
EnemyController.Update()
    │
    └──► currentState.UpdateState()
            │
            ├──► In ChaseState?
            │       │
            │       ├──► Calculate Distance to Player
            │       │
            │       ├──► Distance > AttackRange?
            │       │       │
            │       │       └──► Move Towards Player
            │       │
            │       └──► Distance <= AttackRange?
            │               │
            │               └──► EnemyController.SwitchState(AttackState)
            │                       │
            │                       ├──► ChaseState.ExitState()
            │                       └──► AttackState.EnterState()
            │
            └──► In AttackState?
                    │
                    ├──► Calculate Distance to Player
                    │
                    ├──► Distance <= AttackRange?
                    │       │
                    │       └──► PerformAttack()
                    │               │
                    │               └──► playerDamageable.TakeDamage()
                    │
                    └──► Distance > AttackRange?
                            │
                            └──► EnemyController.SwitchState(ChaseState)
                                    │
                                    ├──► AttackState.ExitState()
                                    └──► ChaseState.EnterState()
```

---

## 📦 Object Pooling Lifecycle

```
┌─────────────────────────────────────────────────────────────┐
│                    OBJECT POOL LIFECYCLE                    │
└─────────────────────────────────────────────────────────────┘

INITIALIZATION
──────────────
ObjectPoolManager.Awake()
    │
    └──► For Each Pool Configuration:
            │
            └──► Instantiate X Objects
                    │
                    ├──► SetActive(false)
                    └──► Enqueue to Pool


SPAWN (Get from Pool)
──────────────────────
Request: GetFromPool("Bullet", position, rotation)
    │
    ├──► Queue Empty?
    │       │
    │       ├──► YES: Instantiate New Object
    │       └──► NO: Dequeue from Pool
    │
    └──► SetActive(true)
    └──► Set Position & Rotation
    └──► Return GameObject


DESPAWN (Return to Pool)
─────────────────────────
Call: ReturnToPool(gameObject)
    │
    ├──► SetActive(false)
    └──► Enqueue back to Pool
    └──► Ready for Reuse


MEMORY DIAGRAM
──────────────
Initial State:
Pool: [○][○][○][○][○]  (5 inactive bullets)

After Spawning 3:
Pool: [○][○]            (2 inactive)
Active: [●][●][●]       (3 active bullets)

After Returning 2:
Pool: [○][○][○][○]      (4 inactive)
Active: [●]             (1 active bullet)
```

---

## 🎮 State Machine Detailed

```
┌─────────────────────────────────────────────────────────────┐
│               ENEMY STATE MACHINE DIAGRAM                   │
└─────────────────────────────────────────────────────────────┘

                    ┌──────────────┐
                    │ Enemy Spawns │
                    └──────┬───────┘
                           │
                           ▼
                 ┌─────────────────┐
         ┌───────│   CHASE STATE   │◄──────┐
         │       └─────────────────┘       │
         │                                  │
         │  Behavior:                       │
         │  • Move towards player           │
         │  • Calculate distance            │
         │  • Rotate to face player         │
         │                                  │
         │  Transition Condition:           │
         │  Distance <= AttackRange         │
         │                                  │
         ▼                                  │
┌─────────────────┐                         │
│  ATTACK STATE   │                         │
└─────────────────┘                         │
                                            │
  Behavior:                                 │
  • Stop moving                             │
  • Face player                             │
  • Deal damage (cooldown)                  │
  • Call IDamageable.TakeDamage()           │
                                            │
  Transition Condition:                     │
  Distance > AttackRange ─────────────────┘


STATE LIFECYCLE
───────────────
1. EnterState(enemy)
   • Cache references
   • Initialize state variables
   • Log entry (debug)

2. UpdateState() [Called Every Frame]
   • Execute state logic
   • Check transition conditions
   • Perform actions

3. ExitState()
   • Cleanup
   • Reset variables
   • Log exit (debug)
```

---

## 🔄 Dependency Graph

```
┌─────────────────────────────────────────────────────────────┐
│                    DEPENDENCY GRAPH                         │
└─────────────────────────────────────────────────────────────┘

IDamageable (Interface)
    ▲
    │ implements
    │
    ├──────────────┬──────────────┐
    │              │              │
PlayerController   EnemyController   (implements)
    ▲                   ▲
    │                   │
    │ uses              │ uses
    │                   │
Projectile          ChaseState
    │               AttackState
    │                   │
    │ uses              │ uses
    │                   │
    └──────┬────────────┘
           │
           ▼
ObjectPoolManager (Singleton)
           │
           │ uses
           │
           ▼
    GameManager (Singleton)


DEPENDENCY DIRECTIONS
─────────────────────
High-Level → Low-Level (Good)
Concrete → Interface (Good)
Many → Singleton (Acceptable)
```

---

## 🏛️ Design Pattern Relationships

### Singleton Pattern

```
┌────────────────────────────────────────┐
│         SINGLETON PATTERN              │
├────────────────────────────────────────┤
│                                        │
│  GameManager ◄──┐                      │
│     │           │                      │
│     │           │ Same Instance        │
│     ▼           │                      │
│  [Instance] ────┘                      │
│                                        │
│  • Only ONE instance exists            │
│  • Global access point                 │
│  • DontDestroyOnLoad                   │
│  • Self-initializing                   │
└────────────────────────────────────────┘
```

### State Pattern

```
┌────────────────────────────────────────┐
│           STATE PATTERN                │
├────────────────────────────────────────┤
│                                        │
│  EnemyController (Context)             │
│         │                              │
│         │ delegates to                 │
│         ▼                              │
│  EnemyState (Abstract)                 │
│         │                              │
│         ├──► ChaseState                │
│         └──► AttackState               │
│                                        │
│  • Behavior changes with state         │
│  • State-specific logic encapsulated   │
│  • Easy to add new states              │
└────────────────────────────────────────┘
```

### Object Pool Pattern

```
┌────────────────────────────────────────┐
│        OBJECT POOL PATTERN             │
├────────────────────────────────────────┤
│                                        │
│  ObjectPoolManager                     │
│         │                              │
│         ├──► Bullet Pool [○][○][○]    │
│         └──► Enemy Pool  [○][○][○]    │
│                                        │
│  Get()  ────► Dequeue ──► Activate    │
│  Return() ──► Deactivate ──► Enqueue  │
│                                        │
│  • Reuses objects                      │
│  • Prevents garbage collection         │
│  • Improves performance                │
└────────────────────────────────────────┘
```

---

## 📊 Data Flow Summary

```
┌─────────────────────────────────────────────────────────────┐
│                      DATA FLOW SUMMARY                      │
└─────────────────────────────────────────────────────────────┘

INPUT LAYER
───────────
Keyboard (WASD) ──────┐
Mouse (Position) ─────┼──► PlayerController
Mouse (Click) ────────┘

GAME LOGIC LAYER
────────────────
PlayerController ──► Projectile ──► IDamageable ──► EnemyController
                          │                              │
                          └──────────────────────────────┘
                                        │
                                        ▼
                            ObjectPoolManager (Reuse)
                                        │
                                        ▼
                            GameManager (Score, State)

AI LAYER
────────
EnemyController ──► EnemyState ──► ChaseState/AttackState
                        │
                        ├──► Move Transform
                        └──► Attack Player

OUTPUT LAYER
────────────
GameManager ──► UI (Score, Time)
PlayerController ──► Transform (Position, Rotation)
EnemyController ──► Transform (Position, Rotation)
Projectile ──► Transform (Position)
```

---

## 🎯 Communication Patterns

### 1. Direct Reference
```
Player ──(holds reference)──► Camera
Enemy ──(holds reference)──► Player
```

### 2. Singleton Access
```
Any Class ──(Instance)──► GameManager
Any Class ──(Instance)──► ObjectPoolManager
```

### 3. Interface Communication
```
Projectile ──(IDamageable)──► Any Damageable Object
```

### 4. State Delegation
```
EnemyController ──(delegates)──► CurrentState
```

### 5. Pool Request/Return
```
Spawner ──(GetFromPool)──► ObjectPoolManager
Object ──(ReturnToPool)──► ObjectPoolManager
```

---

## 🏗️ Extension Points

### Easy to Add:
```
1. New Enemy State
   EnemyState → PatrolState (extend)

2. New Damageable Object
   MonoBehaviour → Destructible (implement IDamageable)

3. New Pooled Object
   ObjectPoolManager → Add Pool Configuration
```

### Medium to Add:
```
1. New Player Ability
   PlayerController → Add Method

2. New Enemy Type
   EnemyController → Subclass or Composition

3. Power-ups
   Collectible → Implement Interface
```

### Advanced to Add:
```
1. Save System
   GameManager → Add Serialization

2. Network Multiplayer
   Add NetworkManager Singleton

3. Advanced AI
   Add more State types
```

---

**This diagram shows the complete architecture and how all components interact!**

