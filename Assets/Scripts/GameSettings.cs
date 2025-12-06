using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Game Duration")]
    public float gameDuration = 300f; // 5 minutes in seconds

    [Header("Scoring")]
    public int pointsPerKill = 10;
    public int pointsPerSecond = 1;
    public int pointsPerPickup = 5;

    [Header("Difficulty Presets")]
    public DifficultySettings easySettings;
    public DifficultySettings mediumSettings;
    public DifficultySettings hardSettings;
}

