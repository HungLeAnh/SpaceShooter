using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Wave Sequence")]
    [SerializeField] private List<EnemyWaveSO> waveTimeline;
    [SerializeField] private bool loopSequence = true;
    [Header("Spawn Line Geometry")]
    [Tooltip("The vertical height coordinate off-screen where enemies materialize.")]
    [SerializeField] private float spawnYCoordinate = 6f;
    [Tooltip("Padding from the extreme left/right viewport edges to keep entities safely on screen.")]
    [SerializeField] private float sideEdgePadding = 0.75f;

    private Dictionary<EnemyType, EnemySpawnConfig> enemyConfigDictionary; 
    private Dictionary<EnemyType, IObjectPool<GameObject>> enemyPool;
    private List<GameObject> activeEnemy;
    private int _currentWaveIndex = 0;
    private bool _isSpawningActive = false;

    public int CurrentWaveIndex { get => _currentWaveIndex; set => _currentWaveIndex = value; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        activeEnemy = new List<GameObject>();
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
            var instance = pool.Get();
            activeEnemy.Add(instance);
            return instance;
        }

        Debug.LogError($"[{name}] No active object pool configuration layer exists for Enemy: {type}!", this);
        return null;
    }

    public void ReleaseEnemy(EnemyController enemy)
    {
        if (enemyPool.TryGetValue(enemy.Type, out var pool))
        {
            activeEnemy.Remove(enemy.gameObject);
            pool.Release(enemy.gameObject);
        }
        else
        {
            Destroy(enemy.gameObject);
        }
    }
    public GameObject SpawnEnemy(EnemyType type)
    {
        if (!enemyConfigDictionary.TryGetValue(type, out var config))
        {
            Debug.LogError($"[{name}] No configuration found for EnemyType: {type}");
            return null;
        }

        GameObject enemyObj = GetEnemy(type);

        if (enemyObj.TryGetComponent<EnemyController>(out var controller))
        {
            // Inject the corresponding structural data into the runtime controller layout
            return enemyObj;
        }
        return null;
    }
    public void StartSpawningSequence()
    {
        _currentWaveIndex = 0;
        StartCoroutine(ExecuteWaveSequenceRoutine());
    }

    private IEnumerator ExecuteWaveSequenceRoutine()
    {
        _isSpawningActive = true;

        while (_currentWaveIndex < waveTimeline.Count)
        {
            EnemyWaveSO currentWave = waveTimeline[_currentWaveIndex];

            HUDController.Instance.ShowElement(HUDStateType.Wave);
            // Execute pattern spawn events in parallel using individual tracked timers
            yield return StartCoroutine(SpawnPatternRoutine(currentWave));

            // Wait for padding buffer time before starting the next wave configuration block
            yield return new WaitForSeconds(currentWave.timeAfterWaveClears);

            _currentWaveIndex++;
            if (loopSequence && _currentWaveIndex >= waveTimeline.Count)
            {
                _currentWaveIndex = 0; // Wrap around for endless testing validation loops
            }
        }

        _isSpawningActive = false;
    }

    private IEnumerator SpawnPatternRoutine(EnemyWaveSO wave)
    {
        float spawnTime = 0;
        int spawnedCount = 0;
        int totalEvents = wave.spawnEvents.Count;

        while (spawnedCount < totalEvents)
        {
            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0)
            {
                var enemy = SpawnEnemy(wave.spawnEvents[spawnedCount].enemyType);
                enemy.GetComponent<Transform>().position = new Vector3(0, spawnYCoordinate, 0);
                enemy.GetComponent<EnemyController>().Initialize(wave.spawnEvents[spawnedCount].pathData);
                if (spawnTime < totalEvents)
                    spawnTime = wave.spawnEvents[spawnedCount].spawnDelay;
                spawnedCount++;

            }

            yield return null;
        }
    }

    public void DestroyAllEnenmy()
    {
        StopAllCoroutines();
        foreach(var enemyGameObject in activeEnemy.ToList())
        {
            enemyGameObject.GetComponent<EnemyController>().ReturnToPool();
        }
    }
}