# Unity Setup Checklist ✅

## Quick Start Guide - Get Running in 10 Minutes!

---

## 📋 Pre-Setup Verification

- [ ] Unity project is open
- [ ] All C# scripts are in `Assets/Scripts/` folder
- [ ] No compilation errors in Console (check bottom of Unity)

---

## 🎮 Step-by-Step Setup

### STEP 1: Create Managers (2 minutes)
```
1. Right-click in Hierarchy
2. Create Empty GameObject
3. Name it "GameManager"
4. In Inspector, click "Add Component"
5. Search for "GameManager"
6. Add the component

Repeat for "ObjectPoolManager"
```

- [ ] GameManager GameObject exists with GameManager script
- [ ] ObjectPoolManager GameObject exists with ObjectPoolManager script

---

### STEP 2: Create Player (3 minutes)
```
1. Hierarchy → Create → 3D Object → Capsule
2. Rename to "Player"
3. In Inspector → Tag dropdown → Select "Player"
   (If "Player" tag doesn't exist, create it:
    Edit → Project Settings → Tags and Layers → Add "Player")
4. Add Component → "Player Controller"
5. Add Component → "Rigidbody"
   • Freeze Rotation X, Y, Z (expand Constraints)
6. Right-click Player → Create Empty
   • Name it "FirePoint"
   • Position: X=0, Y=0, Z=1
```

**Player Inspector Settings:**
```
PlayerController:
  • Move Speed: 5
  • Max Health: 100
  • Bullet Pool Tag: "Bullet"
  • Fire Point: [Drag FirePoint object here]
  • Main Camera: [Drag Main Camera here]
  
Rigidbody:
  • Mass: 1
  • Drag: 0
  • Angular Drag: 0.05
  • Use Gravity: ✓
  • Is Kinematic: ✗
  • Constraints: Freeze Rotation X, Y, Z ✓
```

- [ ] Player capsule created and renamed
- [ ] Player tagged as "Player"
- [ ] PlayerController component added
- [ ] Rigidbody added with frozen rotations
- [ ] FirePoint child object created at (0, 0, 1)
- [ ] FirePoint assigned in PlayerController
- [ ] Main Camera assigned in PlayerController

---

### STEP 3: Create Bullet Prefab (2 minutes)
```
1. Hierarchy → Create → 3D Object → Sphere
2. Rename to "Bullet"
3. Transform Scale: X=0.2, Y=0.2, Z=0.2
4. Add Component → "Projectile"
5. Add Component → "Rigidbody"
   • Use Gravity: OFF
   • Is Kinematic: ON
6. Sphere Collider → Is Trigger: ON
7. Optional: Add Material for color
   • Create → Material
   • Set color (yellow/orange)
   • Drag onto Bullet
8. Drag Bullet from Hierarchy to Project folder (create prefab)
9. Delete Bullet from Hierarchy
```

**Bullet Inspector Settings:**
```
Projectile:
  • Speed: 20
  • Damage: 25
  • Lifetime: 5

Rigidbody:
  • Use Gravity: ✗
  • Is Kinematic: ✓

Sphere Collider:
  • Is Trigger: ✓
  • Radius: 0.5
```

- [ ] Bullet sphere created (scale 0.2, 0.2, 0.2)
- [ ] Projectile component added
- [ ] Rigidbody added (no gravity, kinematic)
- [ ] Collider set to trigger
- [ ] Prefab created in Project folder
- [ ] Bullet removed from scene

---

### STEP 4: Create Enemy Prefab (2 minutes)
```
1. Hierarchy → Create → 3D Object → Capsule
2. Rename to "Enemy"
3. Add Component → "Enemy Controller"
4. Optional: Add Material for color (red)
   • Create → Material
   • Set color (red)
   • Drag onto Enemy
5. Capsule Collider → Is Trigger: OFF
6. Drag Enemy from Hierarchy to Project folder (create prefab)
7. Delete Enemy from Hierarchy
```

**Enemy Inspector Settings:**
```
EnemyController:
  • Max Health: 50
  • Score Value: 10
  • Pool Tag: "Enemy"

Capsule Collider:
  • Is Trigger: ✗
  • Radius: 0.5
  • Height: 2
```

- [ ] Enemy capsule created
- [ ] EnemyController component added
- [ ] Material applied (red color)
- [ ] Collider is NOT trigger
- [ ] Prefab created in Project folder
- [ ] Enemy removed from scene

---

### STEP 5: Configure Object Pool Manager (2 minutes)
```
1. Select ObjectPoolManager in Hierarchy
2. In Inspector, expand "Pools" list
3. Set Size to 2
4. Configure Element 0 (Bullet Pool):
   • Tag: "Bullet"
   • Prefab: [Drag Bullet prefab here]
   • Size: 20
5. Configure Element 1 (Enemy Pool):
   • Tag: "Enemy"
   • Prefab: [Drag Enemy prefab here]
   • Size: 10
```

**ObjectPoolManager Settings:**
```
Pools: Size = 2
  Element 0:
    • Tag: "Bullet"
    • Prefab: Bullet
    • Size: 20
  Element 1:
    • Tag: "Enemy"
    • Prefab: Enemy
    • Size: 10
```

- [ ] ObjectPoolManager selected
- [ ] Pools list size set to 2
- [ ] Bullet pool configured (tag="Bullet", size=20)
- [ ] Enemy pool configured (tag="Enemy", size=10)
- [ ] Prefabs assigned to both pools

---

### STEP 6: Create Enemy Spawner (1 minute)
```
1. Hierarchy → Create Empty GameObject
2. Rename to "EnemySpawner"
3. Add Component → "Enemy Spawner"
4. In Inspector:
   • Enemy Pool Tag: "Enemy"
   • Spawn Interval: 2
   • Spawn Radius: 15
   • Max Enemies: 10
   • Player: [Drag Player object here]
```

**EnemySpawner Settings:**
```
EnemySpawner:
  • Enemy Pool Tag: "Enemy"
  • Spawn Interval: 2
  • Spawn Radius: 15
  • Max Enemies: 10
  • Player: [Player GameObject]
```

- [ ] EnemySpawner GameObject created
- [ ] EnemySpawner component added
- [ ] Settings configured
- [ ] Player reference assigned

---

### STEP 7: Setup Camera (1 minute)
```
1. Select Main Camera in Hierarchy
2. Position: X=0, Y=10, Z=0
3. Rotation: X=90, Y=0, Z=0
4. Projection: Orthographic
5. Size: 10
```

**Camera Settings (Top-Down View):**
```
Transform:
  • Position: (0, 10, 0)
  • Rotation: (90, 0, 0)

Camera:
  • Projection: Orthographic
  • Size: 10
  • Clipping Planes: Near=0.3, Far=1000
```

- [ ] Camera positioned above scene
- [ ] Camera rotated to look down (90° on X)
- [ ] Camera set to Orthographic
- [ ] Camera size set to 10

---

### STEP 8: Setup Ground/Environment (Optional, 1 minute)
```
1. Hierarchy → Create → 3D Object → Plane
2. Scale: X=5, Y=1, Z=5 (makes 50x50 area)
3. Position: X=0, Y=-1, Z=0
4. Optional: Add material/texture
```

- [ ] Ground plane created
- [ ] Ground scaled and positioned
- [ ] (Optional) Material applied

---

## ✅ FINAL VERIFICATION

### Console Check
- [ ] Open Console (Window → General → Console)
- [ ] NO red errors showing
- [ ] If errors exist, double-click to see details

### Hierarchy Check
Your Hierarchy should look like this:
```
Scene
├── Main Camera
├── Directional Light
├── GameManager
├── ObjectPoolManager
├── EnemySpawner
├── Player
│   └── FirePoint
└── Plane (ground, optional)
```

- [ ] All GameObjects present
- [ ] FirePoint is child of Player

### Project Folder Check
```
Assets
└── Scripts
    ├── (All .cs files)
    ├── Bullet (prefab)
    └── Enemy (prefab)
```

- [ ] Bullet prefab exists
- [ ] Enemy prefab exists

---

## 🎮 PLAY TEST!

### Test 1: Basic Setup
```
1. Press Play button
2. Check Console for initialization messages:
   • "Pool 'Bullet' initialized with 20 objects"
   • "Pool 'Enemy' initialized with 10 objects"
```

- [ ] No errors on Play
- [ ] Pools initialized correctly

### Test 2: Player Movement
```
1. Press WASD keys
2. Player should move in those directions
3. Move mouse around
4. Player should rotate to face mouse
```

- [ ] WASD movement works
- [ ] Player rotates toward mouse

### Test 3: Shooting
```
1. Left-click mouse
2. Bullets should spawn at FirePoint
3. Bullets should move forward
4. Check Console: "Player fired a bullet!"
```

- [ ] Bullets spawn on click
- [ ] Bullets move forward
- [ ] Bullets disappear after 5 seconds

### Test 4: Enemy Spawning
```
1. Wait 2 seconds
2. Enemies should spawn around player
3. Check Console: "Enemy spawned! Total: 1/10"
4. Max 10 enemies should spawn
```

- [ ] Enemies spawn periodically
- [ ] Enemies spawn in circle around player
- [ ] Max limit respected (10 enemies)

### Test 5: Enemy AI
```
1. Watch enemies
2. They should move toward player (Chase)
3. When close, they should attack
4. Check Console: "Enemy entered Chase State" / "Attack State"
```

- [ ] Enemies chase player
- [ ] Enemies switch to attack when close
- [ ] Enemies face player
- [ ] State transitions logged

### Test 6: Combat
```
1. Shoot enemies with bullets
2. Enemies should take damage
3. Check Console: "Enemy took 25 damage!"
4. Enemy should die after ~2 shots (50 HP / 25 damage)
5. Score should increase
6. Enemy should disappear (return to pool)
```

- [ ] Bullets hit enemies
- [ ] Damage applied correctly
- [ ] Enemies die at 0 HP
- [ ] Score increases
- [ ] Enemies return to pool (check hierarchy - deactivated)

### Test 7: Player Damage
```
1. Let enemy get close to player
2. Player should take damage
3. Check Console: "Player took 10 damage!"
4. After ~10 hits (100 HP / 10 damage), player dies
5. Game should end (check Console: "Game Over!")
```

- [ ] Enemies damage player when close
- [ ] Player health decreases
- [ ] Player dies at 0 HP
- [ ] Game Over triggered

---

## 🐛 Troubleshooting

### ❌ "GameManager instance is null!"
**Fix**: Make sure GameManager GameObject exists in scene with GameManager component

### ❌ "ObjectPoolManager instance is null!"
**Fix**: Make sure ObjectPoolManager GameObject exists with component and pools configured

### ❌ Bullets don't shoot
**Fix**: 
- Check FirePoint is assigned in PlayerController
- Check Bullet pool is configured with tag="Bullet"
- Check Bullet prefab is assigned

### ❌ Enemies don't spawn
**Fix**:
- Check Enemy pool is configured with tag="Enemy"
- Check Enemy prefab is assigned
- Check Player reference is assigned in EnemySpawner
- Make sure Player has "Player" tag

### ❌ Bullets don't damage enemies
**Fix**:
- Check Bullet has Collider with "Is Trigger" ON
- Check Enemy has Collider
- Check EnemyController implements IDamageable (it does in code)
- Open Physics settings: Edit → Project Settings → Physics
  - Check Layer Collision Matrix

### ❌ Player doesn't move
**Fix**:
- Check Rigidbody is attached
- Check Rigidbody is NOT kinematic
- Check Input settings: Edit → Project Settings → Input Manager
  - Horizontal/Vertical axes should be configured (default)

### ❌ Enemies don't chase
**Fix**:
- Ensure Player has "Player" tag
- Check Console for "Player not found" errors
- Verify EnemyController component is on enemy prefab

---

## 🎓 Testing for Professor Demo

### Demonstration Checklist
- [ ] Show Player movement (WASD + mouse aim)
- [ ] Show Shooting mechanics (click to fire)
- [ ] Show Enemy spawning (wait 2 seconds)
- [ ] Show Enemy AI (Chase → Attack states)
- [ ] Show Combat (bullets kill enemies)
- [ ] Show Score system (Console or UI)
- [ ] Show Object Pooling (Hierarchy - objects deactivate, not destroy)
- [ ] Show Game Over (let player die)

### Code Review Points
- [ ] Show IDamageable interface (Abstraction)
- [ ] Show EnemyState hierarchy (Inheritance & Polymorphism)
- [ ] Show Singleton pattern (GameManager, ObjectPoolManager)
- [ ] Show State pattern (EnemyController + States)
- [ ] Show Object Pool pattern (ObjectPoolManager)
- [ ] Show Interface usage (GetComponent<IDamageable>)
- [ ] Point out OOP comments in code

---

## 📊 Performance Verification

### Check Object Pooling Works
```
1. Play the game
2. Open Hierarchy
3. Expand ObjectPoolManager
4. You should see:
   - 20 Bullet objects (some active, some inactive)
   - 10 Enemy objects (some active, some inactive)
5. Shoot bullets - watch them deactivate, NOT destroy
6. Watch enemies - they deactivate when killed, NOT destroy
```

- [ ] Pool objects visible in Hierarchy
- [ ] Objects SetActive(false) instead of Destroy()
- [ ] Objects reused when GetFromPool() called

---

## ✅ SETUP COMPLETE!

If all checkboxes are marked:
- ✅ Your game is fully functional
- ✅ All OOP principles are implemented
- ✅ Ready for professor review
- ✅ Ready for extension/modification

---

## 🚀 Next Steps

### Add Visual Polish:
1. Create Materials for Player, Enemies, Bullets (different colors)
2. Add particle effects for hits (Unity Particle System)
3. Add sound effects (AudioSource components)

### Add UI:
1. Create Canvas (UI → Canvas)
2. Add Text elements for Score, Health, Time
3. Update from GameManager and PlayerController

### Extend Gameplay:
1. Add PatrolState for enemies
2. Add power-ups (health packs, weapons)
3. Add wave system (increasing difficulty)
4. Add different enemy types

---

**You're all set! Press Play and enjoy your OOP-compliant Arena Shooter! 🎮**

