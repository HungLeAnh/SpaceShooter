using System;
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
    Torpedo
}
public enum AnimationType
{
    Idle,
    Fire,
    Die,
}

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour,IDamageable
{
    [Header("Data Configuration")]
    [SerializeField] protected EnemyDataSO enemyData;
    [SerializeField] protected List<Transform> muzzlePoint;
    [SerializeField] protected Animator enemyAnimator;
    [SerializeField] private float arrivalDistance = 0.2f;

    protected float currentHealth;
    protected EnemyManager enemyManager;
    private float nextFireTime;
    private bool isDestroyed;

    private EnemyPathSO pathData;
    private Vector2[] _worldWaypoints;
    private int _currentWaypointIndex = 0;
    private bool _hasPath = false;
    private Rigidbody2D _rb;
    public EnemyType Type => enemyData != null ? enemyData.enemyType : EnemyType.Scout;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Initialize(EnemyPathSO data)
    {
        this.enemyManager = EnemyManager.Instance;
        currentHealth = enemyData.maxHealth;
        isDestroyed = false;

        pathData = data;
        if (pathData == null || pathData.localWaypoints.Length == 0) return;

        // Convert local pattern offsets into absolute world positions based on current spawn location
        _worldWaypoints = new Vector2[pathData.localWaypoints.Length];
        Vector2 spawnPosition = _rb.position;

        for (int i = 0; i < pathData.localWaypoints.Length; i++)
        {
            _worldWaypoints[i] = spawnPosition + pathData.localWaypoints[i];
        }

        _currentWaypointIndex = 0;
        _hasPath = true;
    }
    private void Update()
    {
         Fire();
    }

    private void Fire()
    {
        if (Time.time < nextFireTime || isDestroyed)
            return;

        nextFireTime = Time.time + enemyData.fireRate;
        PlayAnimation(AnimationType.Fire);
        foreach (var point in muzzlePoint)
        {
            GameObject projectile = MultiBulletPoolManager.Instance.GetBullet(enemyData.bulletType);
            if (projectile != null)
            {
                projectile.transform.position = point.position;
                projectile.transform.rotation = point.rotation;
                projectile.GetComponent<PooledBullet>().SetFireDirection(Vector3.down);
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (enemyData == null) return;
   
        if (!_hasPath) return;
        Move();
    }
    protected virtual void Move()
    {
        //Debug.Log("moving");
        if (_currentWaypointIndex >= _worldWaypoints.Length)
        {
            // Path finished: Continue flying downwards off-screen
            _rb.velocity = Vector2.down * enemyData.moveSpeed;
            return;
        }

        Vector2 targetPosition = _worldWaypoints[_currentWaypointIndex];
        Vector2 currentPosition = _rb.position;

        // 1. Calculate direction vector to the next node
        Vector2 direction = targetPosition - currentPosition;
        float distanceToTarget = direction.magnitude;

        // 2. Check if we arrived at the current node index
        if (distanceToTarget <= arrivalDistance)
        {
            _currentWaypointIndex++;
            if (pathData.loopPath && _currentWaypointIndex >= _worldWaypoints.Length)
            {
                _currentWaypointIndex = 0; // Loop the flight pattern
            }
            return;
        }

        // 3. Apply normalized direction speed to the physical body velocity matrix
        _rb.velocity = direction.normalized * enemyData.moveSpeed;
    }
    public virtual void Damage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            StartCoroutine(PlayDieAniamtion());
            GameManager.Instance.SpaceShip.UpdateScore(enemyData.scoreReward);
        }
    }
    IEnumerator PlayDieAniamtion()
    {
        isDestroyed = true;
        PlayAnimation(AnimationType.Die);
        int random = UnityEngine.Random.Range(0, 10);
        if (random >= 2)
        {
            int randomPowerUp = UnityEngine.Random.Range(0, Enum.GetValues(typeof(PowerUpType)).Length);
            var powerup = PowerUpManager.Instance.GetPowerUp((PowerUpType)randomPowerUp);
            powerup.transform.position = transform.position;
        }
        yield return new WaitForSeconds(0.5f);
        Die();
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

    protected void PlayAnimation(AnimationType type)
    {
        switch (type)
        {
            case AnimationType.Idle:
                enemyAnimator.Play(AnimationType.Idle.ToString());
                break;
            case AnimationType.Fire:
                enemyAnimator.Play(AnimationType.Fire.ToString());
                break;
            case AnimationType.Die:
                enemyAnimator.Play(AnimationType.Die.ToString());
                break;
        }

    }
}


