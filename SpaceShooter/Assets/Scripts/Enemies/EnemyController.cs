using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Battlecruiser,
    Bomber,
    Dreadnought,
    Fighter,
    Frigate,
    Scout,
    Support,      
    Tortedo
}

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour,IDamageable
{
    [Header("Data Configuration")]
    [SerializeField] protected EnemyDataSO enemyData;
    [SerializeField] protected List<Transform> muzzlePoint;
    protected float currentHealth;
    protected EnemyManager enemyManager;
    private float nextFireTime;
    public EnemyType Type => enemyData != null ? enemyData.enemyType : EnemyType.Scout;

    protected virtual void Awake()
    {
    }

    public virtual void Initialize(EnemyManager enemyManager)
    {
        this.enemyManager = enemyManager;
        currentHealth = enemyData.maxHealth;
    }
    private void Update()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + enemyData.fireRate;
        foreach (var point in muzzlePoint)
        {
            GameObject projectile = MultiBulletPoolManager.Instance.GetBullet(enemyData.bulletType);

            if (projectile != null)
            {
                projectile.transform.position = point.position;
                projectile.transform.rotation = point.rotation;
            }
        }
    }
    protected virtual void FixedUpdate()
    {
        if (enemyData == null) return;
        Move();
    }
    protected virtual void Move()
    {
    }
    public virtual void Damage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        if (enemyManager != null && gameObject.activeSelf)
        {
            enemyManager.ReleaseEnemy(this);
        }
    }


}
