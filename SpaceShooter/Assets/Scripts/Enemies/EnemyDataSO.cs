using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "SpaceShooter/Enemy Data", order = 1)]
public class EnemyDataSO : ScriptableObject
{
    [Header("Identity")]
    public EnemyType enemyType;
    public string enemyName;

    [Header("Base Attributes")]
    public float maxHealth = 50f;
    public float moveSpeed = 3f;
    public int scoreReward = 100;

    [Header("Combat Settings")]
    public float fireRate = 2f;
    public List<BulletType> bulletType;
}
