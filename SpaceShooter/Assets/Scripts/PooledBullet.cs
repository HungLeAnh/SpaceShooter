using UnityEngine;

public class PooledBullet : MonoBehaviour
{
    [SerializeField] private BulletType bulletType;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float maxLifetime = 3f;

    private MultiBulletPoolManager _poolManager;
    private float _lifetimeTimer;
    private Transform _transform;

    public BulletType Type => bulletType;

    private void Awake()
    {
        _transform = transform;
    }

    public void Initialize(MultiBulletPoolManager manager)
    {
        _poolManager = manager;
    }

    private void OnEnable()
    {
        _lifetimeTimer = 0f;
    }

    private void Update()
    {
        _transform.Translate(Vector3.up * (speed * Time.deltaTime));

        _lifetimeTimer += Time.deltaTime;
        if (_lifetimeTimer >= maxLifetime)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_poolManager != null && gameObject.activeSelf)
        {
            _poolManager.ReleaseBullet(this);
        }
    }
}