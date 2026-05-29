using System.Collections.Generic;
using UnityEngine;



public class Boss : EnemyController
{
    [SerializeField] private GameObject ShieldObject;
    [SerializeField] private float shieldMax;

    private float currentShield;
    private bool isInitShield = false;
    public override void Initialize(EnemyPathSO data)
    {
        base.Initialize(data);
        isInitShield = false;
        currentShield = 0;
    }
    protected override void Fire()
    {
        if (Time.time < nextFireTime || isDestroyed)
            return;

       
        nextFireTime = Time.time + enemyData.fireRate;
        PlayAnimation(AnimationType.Fire);
        foreach (var point in muzzlePoint)
        {
            GameObject projectile = MultiBulletPoolManager.Instance.GetBullet(enemyData.bulletType[0]);
            if (projectile != null)
            {
                projectile.transform.position = point.position;
                projectile.transform.rotation = point.rotation;
                projectile.GetComponent<PooledBullet>().SetFireDirection(Vector3.down);
            }
        }
        
    }
    public override void Damage(float damage)
    {
        if(currentShield <= 0)
        {
            base.Damage(damage);
            if(!isInitShield && currentHealth / enemyData.maxHealth <= 0.75 &&
            currentHealth / enemyData.maxHealth > 0.25)
            {
                isInitShield = true;
                currentShield = shieldMax;
                ShieldObject.SetActive(true);
            }
        }
        else
        {
            currentShield -= damage;
            if (currentShield <= 0) 
            {
                ShieldObject.SetActive(false);
            }
        }
    }
    protected override void Die()
    {
        base.Die();
        GameManager.Instance.ChangeState(GameStateType.VicTory);
    }
}
