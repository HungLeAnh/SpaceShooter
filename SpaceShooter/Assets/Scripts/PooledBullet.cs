using System;
using System.Collections;
using UnityEngine;

public class PooledBullet : MonoBehaviour
{
    [SerializeField] private BulletType bulletType;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float maxLifetime = 3f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject explosionEffect;

    private MultiBulletPoolManager _poolManager;
    private float _lifetimeTimer;
    private Vector3 dir;
    private bool isMoving = true;
    public BulletType Type => bulletType;

    private void Awake()
    {
    }

    public void Initialize(MultiBulletPoolManager manager)
    {
        _poolManager = manager;

    }
    public void SetFireDirection(Vector3 fireDir)
    {
        dir = fireDir;
        isMoving = true;
        bullet.SetActive(true);
        explosionEffect.SetActive(false);
    }
    private void OnEnable()
    {
        _lifetimeTimer = 0f;
    }

    private void Update()
    {
        if(isMoving)
            transform.position += dir * (speed * Time.deltaTime);

        _lifetimeTimer += Time.deltaTime;
        if (_lifetimeTimer >= maxLifetime)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.Damage(damage);
            StartCoroutine(PlayExplosionAnimation());
        }

    }

    IEnumerator PlayExplosionAnimation()
    {
        isMoving = false;
        bullet.SetActive(false);
        explosionEffect.SetActive(true);
        yield return new WaitForSeconds(0.4f);
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