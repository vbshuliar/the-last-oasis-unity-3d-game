using UnityEngine;
using UnityEngine.UI;
using TMPro;

// manages all the ui elements on screen like health bar timer score etc
public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI itemsText;

    [Header("References")]
    [SerializeField] private Actor playerActor;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        
        // subscribe to events when gamemanager fires these events our methods get called
        if (gameManager != null)
        {
            gameManager.OnTimeUpdated += UpdateTimer;
            gameManager.OnScoreUpdated += UpdateScore;
            gameManager.OnKillsUpdated += UpdateKills;
            gameManager.OnItemsCollectedUpdated += UpdateItems;
        }

        if (playerActor == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerActor = player.GetComponent<Actor>();
            }
        }

        if (playerActor != null)
        {
            playerActor.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(playerActor.currentHealth, playerActor.maxHealth);
        }

        UpdateScore(0);
        UpdateKills(0);
        UpdateItems(0);
    }

    void Update()
    {
        if (gameManager != null && gameManager.CurrentState == GameState.Playing)
        {
            float timeRemaining = gameManager.GetTimeRemaining();
            UpdateTimer(timeRemaining);
        }
    }

    void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void UpdateTimer(float timeRemaining)
    {
        if (timerText != null)
        {
            // convert seconds to minutes and seconds format as mm ss
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    void UpdateKills(int kills)
    {
        if (killsText != null)
        {
            killsText.text = "Kills: " + kills.ToString();
        }
    }

    void UpdateItems(int items)
    {
        if (itemsText != null)
        {
            itemsText.text = "Items: " + items.ToString();
        }
    }

    void OnDestroy()
    {
        // unsubscribe from events to prevent memory leaks
        if (gameManager != null)
        {
            gameManager.OnTimeUpdated -= UpdateTimer;
            gameManager.OnScoreUpdated -= UpdateScore;
            gameManager.OnKillsUpdated -= UpdateKills;
            gameManager.OnItemsCollectedUpdated -= UpdateItems;
        }

        if (playerActor != null)
        {
            playerActor.OnHealthChanged -= UpdateHealthBar;
        }
    }
}

