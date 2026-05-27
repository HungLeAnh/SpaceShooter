using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [Serializable]
    public struct PowerUpConfig
    {
        public PowerUpType type;
        public GameObject prefab;
        public int defaultCapacity;
        public int maxPoolSize;
    }

    [SerializeField] private List<PowerUpConfig> powerUpConfig;

    private Dictionary<PowerUpType, IObjectPool<GameObject>> powerUpPool;
    private Dictionary<PowerUpType, PowerUpConfig> powerUpConfigDictionary;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        powerUpPool= new Dictionary<PowerUpType, IObjectPool<GameObject>>();
        powerUpConfigDictionary = new Dictionary<PowerUpType, PowerUpConfig>();
        InitializePools();
    }
    private void InitializePools()
    {
        foreach (var config in powerUpConfig)
        {
            powerUpConfigDictionary[config.type] = config;

            PowerUpType currentType = config.type;

            var newPool = new ObjectPool<GameObject>(
                createFunc: () => CreatePowerUpInstance(currentType),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxPoolSize
            );

            powerUpPool.Add(currentType, newPool);
        }
    }
    private GameObject CreatePowerUpInstance(PowerUpType type)
    {
        GameObject prefab = powerUpConfigDictionary[type].prefab;
        GameObject instance = Instantiate(prefab, transform);

        if (instance.TryGetComponent<PowerUp>(out var powerUpComponent))
        {
            powerUpComponent.Initialize(this);
        }
        else
        {
            Debug.LogError($"[{name}] Prefab mapped to {type} is missing the PowerUp script component layer!", this);
        }

        return instance;
    }

    public GameObject GetPowerUp(PowerUpType type)
    {
        if (powerUpPool.TryGetValue(type, out var pool))
        {
            return pool.Get();
        }

        Debug.LogError($"[{name}] No active object pool configuration layer exists for PowerUpType: {type}!", this);
        return null;
    }

    public void ReleasePowerUp(PowerUp powerUp)
    {
        if (powerUpPool.TryGetValue(powerUp.PowerUpType, out var pool))
        {
            pool.Release(powerUp.gameObject);
        }
        else
        {
            Destroy(powerUp.gameObject);
        }
    }
}
