using UnityEngine;
using UnityEngine.UI;
using TMPro;

// manages all the ui elements on screen like health bar timer score etc
public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private Image healthBar;
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

        FindAndSubscribeToPlayer();

        UpdateScore(0);
        UpdateKills(0);
        UpdateItems(0);
    }

    void FindAndSubscribeToPlayer()
    {
        // Try to find player if not assigned
        if (playerActor == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerActor = player.GetComponent<Actor>();
                if (playerActor == null)
                {
                    Debug.LogError("UIManager: Player GameObject found but Actor component is missing!");
                    return;
                }
            }
        }

        // Subscribe to health changes (unsubscribe first to avoid double subscription)
        if (playerActor != null)
        {
            playerActor.OnHealthChanged -= UpdateHealthBar; // Unsubscribe first to avoid duplicates
            playerActor.OnHealthChanged += UpdateHealthBar;
            // Update health bar immediately with current health
            UpdateHealthBar(playerActor.currentHealth, playerActor.maxHealth);
            Debug.Log($"UIManager: Subscribed to player health changes. Current: {playerActor.currentHealth}/{playerActor.maxHealth}");
        }
        else
        {
            Debug.LogWarning("UIManager: Player Actor not found! Make sure Player has 'Player' tag and Actor component.");
        }
    }

    private float lastHealthCheckTime = 0f;
    private const float HEALTH_CHECK_INTERVAL = 0.1f; // Check every 0.1 seconds as fallback

    void Update()
    {
        // Try to find player if still not found (in case player spawns after UI)
        if (playerActor == null)
        {
            FindAndSubscribeToPlayer();
        }
        // Fallback: Directly poll health if event system isn't working
        else if (Time.time - lastHealthCheckTime > HEALTH_CHECK_INTERVAL)
        {
            lastHealthCheckTime = Time.time;
            // Update health bar directly as fallback (in case event didn't fire)
            UpdateHealthBar(playerActor.currentHealth, playerActor.maxHealth);
        }

        if (gameManager != null && gameManager.CurrentState == GameState.Playing)
        {
            float timeRemaining = gameManager.GetTimeRemaining();
            UpdateTimer(timeRemaining);
        }
    }

    private int lastHealth = -1;
    private int lastMaxHealth = -1;

    void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBar == null)
        {
            Debug.LogWarning("UIManager: Health bar Image is not assigned!");
            return;
        }

        if (maxHealth <= 0)
        {
            Debug.LogWarning($"UIManager: Max health is 0 or negative! Current: {currentHealth}, Max: {maxHealth}");
            healthBar.fillAmount = 0f;
            return;
        }

        // Calculate fill amount (0.0 to 1.0) for Image with Filled type
        // Clamp to ensure it's always between 0 and 1
        float fillAmount = Mathf.Clamp01((float)currentHealth / (float)maxHealth);
        healthBar.fillAmount = fillAmount;
        
        // Only log when health actually changes (to reduce console spam)
        if (currentHealth != lastHealth || maxHealth != lastMaxHealth)
        {
            Debug.Log($"UIManager: Health bar updated! Health: {currentHealth}/{maxHealth}, Fill Amount: {fillAmount}");
            lastHealth = currentHealth;
            lastMaxHealth = maxHealth;
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

    void OnEnable()
    {
        // Re-subscribe if GameObject is re-enabled
        if (playerActor != null)
        {
            playerActor.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(playerActor.currentHealth, playerActor.maxHealth);
        }
    }

    void OnDisable()
    {
        // Unsubscribe when disabled
        if (playerActor != null)
        {
            playerActor.OnHealthChanged -= UpdateHealthBar;
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

