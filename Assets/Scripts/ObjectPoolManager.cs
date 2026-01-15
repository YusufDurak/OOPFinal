using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OOP PRINCIPLE: SINGLETON PATTERN + OBJECT POOLING (Performance Pattern)
/// This manager implements the Object Pool pattern to reuse game objects
/// instead of constantly instantiating and destroying them (expensive operations).
/// The Singleton ensures centralized pool management across the entire game.
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    // Singleton instance
    private static ObjectPoolManager _instance;
    
    public static ObjectPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("ObjectPoolManager instance is null! Make sure there's an ObjectPoolManager in the scene.");
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION
    /// Dictionary stores pools of objects by tag/identifier.
    /// Private field prevents external modification while allowing controlled access through methods.
    /// </summary>
    private Dictionary<string, Queue<GameObject>> _poolDictionary;
    
    /// <summary>
    /// Serialized dictionary for inspector setup
    /// Maps pool tags to their corresponding prefabs
    /// </summary>
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    
    [Header("Pool Configuration")]
    [Tooltip("Define pools for different object types (Bullets, Enemies, etc.)")]
    public List<Pool> pools;
    
    private void Awake()
    {
        // Singleton initialization
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        
        InitializePools();
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ENCAPSULATION & ABSTRACTION
    /// Hides the complexity of pool initialization from external classes.
    /// </summary>
    private void InitializePools()
    {
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();
        
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            
            // Pre-instantiate objects for the pool
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(this.transform); // Organize in hierarchy
                objectPool.Enqueue(obj);
            }
            
            _poolDictionary.Add(pool.tag, objectPool);
            Debug.Log($"Pool '{pool.tag}' initialized with {pool.size} objects");
        }
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ABSTRACTION
    /// Retrieves an object from the pool, abstracting away the complexity of:
    /// - Queue management
    /// - Object activation
    /// - Position/rotation setup
    /// - Dynamic instantiation if pool is empty
    /// </summary>
    public GameObject GetFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist!");
            return null;
        }
        
        GameObject objectToSpawn;
        
        // If pool is empty, create a new object dynamically
        if (_poolDictionary[tag].Count == 0)
        {
            Debug.LogWarning($"Pool '{tag}' is empty! Creating new object dynamically.");
            Pool poolConfig = pools.Find(p => p.tag == tag);
            if (poolConfig != null)
            {
                objectToSpawn = Instantiate(poolConfig.prefab);
                objectToSpawn.transform.SetParent(this.transform);
            }
            else
            {
                Debug.LogError($"Pool configuration for '{tag}' not found!");
                return null;
            }
        }
        else
        {
            objectToSpawn = _poolDictionary[tag].Dequeue();
        }
        
        // Setup object
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        
        return objectToSpawn;
    }
    
    /// <summary>
    /// OOP PRINCIPLE: ABSTRACTION
    /// Returns object to pool, hiding the details of:
    /// - Object deactivation
    /// - Queue management
    /// - Parent hierarchy organization
    /// </summary>
    public void ReturnToPool(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Trying to return null object to pool!");
            return;
        }
        
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        
        // Find which pool this object belongs to
        string poolTag = obj.name.Replace("(Clone)", "").Trim();
        
        foreach (var pool in pools)
        {
            if (poolTag.Contains(pool.prefab.name))
            {
                if (_poolDictionary.ContainsKey(pool.tag))
                {
                    _poolDictionary[pool.tag].Enqueue(obj);
                    return;
                }
            }
        }
        
        Debug.LogWarning($"Could not find pool for object: {obj.name}");
    }
}

