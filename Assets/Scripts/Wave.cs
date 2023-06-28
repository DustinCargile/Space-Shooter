using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "enemyWave.asset", menuName = "ScriptableObjects/EnemyWave", order = 1)]
public class Wave : ScriptableObject
{
    [Tooltip("The name that will be displayed when the wave starts.")]
    public string waveName;
    [Tooltip("Zero-based identifier for the order of wave.")]
    public int currentWave;
    [Space(10)]
    [Header("Enemy Settings")]
    [Tooltip("Enemy prefabs go here.")]
    public GameObject[] enemies;

    [Tooltip("Higher numbers means faster, can use numbers less than 1.")]
    public float enemyFrequency;
    [Tooltip("The maximum enemies that will spawn during the wave.")]
    public int maxEnemies;

    [Space(10)]
    [Header("Powerup Settings")]
    [Tooltip("Power-up prefabs go here.")]
    public GameObject[] powerups;
    [Tooltip("Higher numbers means faster, can use numbers less than 1.")]
    public float powerupFrequency;
    [Tooltip("The maximum power-ups that will spawn during the wave.")]
    public int maxPowerups;

}
