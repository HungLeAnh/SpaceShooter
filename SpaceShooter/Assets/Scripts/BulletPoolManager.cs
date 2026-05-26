using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public enum BulletType
{
    Standard,
    HeavyLaser,
    SpreadPlasma,
    HomingMissile
}
public class MultiBulletPoolManager : MonoBehaviour
{
    public static MultiBulletPoolManager Instance;

    [System.Serializable]
    public struct BulletPoolConfig
    {
        public BulletType type;
        public GameObject prefab;
        public int defaultCapacity;
        public int maxPoolSize;
    }

    [Header("Pool Configurations")]
    [SerializeField] private List<BulletPoolConfig> poolConfigurations;

    // Dictionary storage mapping the Enum to the respective Native Unity Object Pool interface
    private Dictionary<BulletType, IObjectPool<GameObject>> _pools;
    private Dictionary<BulletType, BulletPoolConfig> _configLookup;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _pools = new Dictionary<BulletType, IObjectPool<GameObject>>();
        _configLookup = new Dictionary<BulletType, BulletPoolConfig>();

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var config in poolConfigurations)
        {
            // Cache config data lookup maps for performance
            _configLookup[config.type] = config;

            // Capture the specific type context safely inside local scope for the lambda expressions
            BulletType currentType = config.type;

            // Generate an isolated native Unity object pool instance for this specific type index
            var newPool = new ObjectPool<GameObject>(
                createFunc: () => CreateBulletInstance(currentType),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxPoolSize
            );

            _pools.Add(currentType, newPool);
        }
    }

    private GameObject CreateBulletInstance(BulletType type)
    {
        GameObject prefab = _configLookup[type].prefab;
        GameObject instance = Instantiate(prefab, transform);

        if (instance.TryGetComponent<PooledBullet>(out var bulletComponent))
        {
            bulletComponent.Initialize(this);
        }
        else
        {
            Debug.LogError($"[{name}] Prefab mapped to {type} is missing the PooledBullet script component layer!", this);
        }

        return instance;
    }

    // Public API: Request an asset instance sequence from a specific sub-pool category
    public GameObject GetBullet(BulletType type)
    {
        if (_pools.TryGetValue(type, out var pool))
        {
            return pool.Get();
        }

        Debug.LogError($"[{name}] No active object pool configuration layer exists for BulletType: {type}!", this);
        return null;
    }

    // Public API: Reclaim an active bullet component asset safely back to its specific pool
    public void ReleaseBullet(PooledBullet bullet)
    {
        if (_pools.TryGetValue(bullet.Type, out var pool))
        {
            pool.Release(bullet.gameObject);
        }
        else
        {
            // Fallback safety catch to prevent memory leaks if objects are manipulated unexpectedly
            Destroy(bullet.gameObject);
        }
    }
}