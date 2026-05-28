using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPathData", menuName = "SpaceShooter/Movement Path")]
[Serializable]
public class EnemyPathSO : ScriptableObject
{
    [Tooltip("Local positions relative to the spawn point that the enemy will travel through.")]
    public Vector2[] localWaypoints;
    public bool loopPath = false;
}