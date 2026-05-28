using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemySpawnEvent
{
    public EnemyType enemyType;

    [Tooltip("The delay (in seconds) after the wave starts before this specific enemy spawns.")]
    public float spawnDelay;

    public EnemyPathSO pathData;

}
[CreateAssetMenu(fileName = "NewEnemyWave", menuName = "SpaceShooter/Enemy Wave Configuration", order = 2)]
public class EnemyWaveSO : ScriptableObject
{
    public string waveName = "Standard Wave";

    [Tooltip("List of all individual enemy spawn timings and horizontal offsets for this pattern layout.")]
    public List<EnemySpawnEvent> spawnEvents;

    [Tooltip("Time to wait after all enemies in this wave have been spawned before advancing to the next wave configuration.")]
    public float timeAfterWaveClears = 4f;
}
