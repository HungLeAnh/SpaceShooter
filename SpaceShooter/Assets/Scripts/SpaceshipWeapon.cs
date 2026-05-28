using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipWeapon : MonoBehaviour
{
    [SerializeField] private List<Transform> muzzlePoint;
    [SerializeField] private BulletType currentActiveAmmo = BulletType.Standard;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private Animator weaponAnimator;

    private float nextFireTime;
    private void Awake()
    {
    }
    public void FireActiveWeapon()
    {
        if (Time.time < nextFireTime) 
            return;
        
        nextFireTime = Time.time + fireRate;
        foreach (var point in muzzlePoint)
        {
            GameObject projectile = MultiBulletPoolManager.Instance.GetBullet(currentActiveAmmo);

            if (projectile != null)
            {
                projectile.transform.position = point.position;
                projectile.transform.rotation = point.rotation;
                projectile.GetComponent<PooledBullet>().SetFireDirection(Vector3.up);
            }
        }
    }
}
