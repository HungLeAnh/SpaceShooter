using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public struct EnemySpawnConfig
    {
        public EnemyType type;
        public GameObject prefab;
        public int defaultCapacity;
        public int maxPoolSize;
    }
    public static EnemyManager Instance;

    [Header("Configuration Database")]
    [SerializeField] private List<EnemySpawnConfig> enemyConfig;
    [SerializeField] private Transform[] spawnPoints;

    private Dictionary<EnemyType, EnemySpawnConfig> enemyConfigDictionary; 
    private Dictionary<EnemyType, IObjectPool<GameObject>> enemyPool;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);


    }
    private void Start()
    {
        enemyConfigDictionary = new Dictionary<EnemyType, EnemySpawnConfig>();

        enemyPool = new Dictionary<EnemyType, IObjectPool<GameObject>>();
        InitializePools();
    }
    private void InitializePools()
    {
        foreach (var config in enemyConfig)
        {
            enemyConfigDictionary[config.type] = config;

            EnemyType currentType = config.type;

            var newPool = new ObjectPool<GameObject>(
                createFunc: () => CreateEnemyInstance(currentType),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxPoolSize
            );

            enemyPool.Add(currentType, newPool);
        }
    }
    private GameObject CreateEnemyInstance(EnemyType type)
    {
        GameObject prefab = enemyConfigDictionary[type].prefab;
        GameObject instance = Instantiate(prefab, transform);

        if (instance.TryGetComponent<EnemyController>(out var enemy))
        {
            enemy.Initialize(this);
        }
        else
        {
            Debug.LogError($"[{name}] Prefab mapped to {type} is missing the enemy script component layer!", this);
        }

        return instance;
    }

    public GameObject GetEnemy(EnemyType type)
    {
        if (enemyPool.TryGetValue(type, out var pool))
        {
            return pool.Get();
        }

        Debug.LogError($"[{name}] No active object pool configuration layer exists for Enemy: {type}!", this);
        return null;
    }

    public void ReleaseEnemy(EnemyController enemy)
    {
        if (enemyPool.TryGetValue(enemy.Type, out var pool))
        {
            pool.Release(enemy.gameObject);
        }
        else
        {
            Destroy(enemy.gameObject);
        }
    }
    // Quick testing method wrapper
    [ContextMenu("Test Spawn Fighter")]
    private void TestSpawn()
    {
        if (spawnPoints.Length > 0)
        {
            SpawnEnemy(EnemyType.Fighter, spawnPoints[0].position);
        }
    }
    public void SpawnEnemy(EnemyType type, Vector2 position)
    {
        if (!enemyConfigDictionary.TryGetValue(type, out var config))
        {
            Debug.LogError($"[{name}] No configuration found for EnemyType: {type}");
            return;
        }

        GameObject enemyObj = GetEnemy(type);

        if (enemyObj.TryGetComponent<EnemyController>(out var controller))
        {
            // Inject the corresponding structural data into the runtime controller layout
            controller.Initialize(this);
        }
    }
}