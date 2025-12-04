using UnityEngine;

[CreateAssetMenu(fileName = "DifficultySettings", menuName = "Game/Difficulty Settings")]
public class DifficultySettings : ScriptableObject
{
    [Header("Enemy Settings")]
    public float enemySpawnRate = 1f; // Enemies per second
    public float enemySpeed = 3.5f;
    public int enemyDamage = 1;
    public int enemyHealth = 3;
    public int maxEnemiesOnScreen = 20;

    [Header("Item Settings")]
    public float itemSpawnRate = 0.5f; // Items per second
    public float itemSpawnChance = 0.3f; // Chance per spawn interval

    [Header("Player Settings")]
    public float playerHealthMultiplier = 1f;
    public float playerDamageMultiplier = 1f;
    public float playerSpeedMultiplier = 1f;

    [Header("Score Multiplier")]
    public float scoreMultiplier = 1f;
}

