using System;
using UnityEngine;

// handles health and damage for player and enemies
public class Actor : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth { get; private set; }

    // events notify other scripts when health changes or actor dies
    public event Action<Actor> OnDeath;
    public event Action<int, int> OnHealthChanged; // passes current health and max health
    public event Action<int> OnDamageTaken;

    // initializes current health and notifies subscribers
    void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // subtracts health and fires damage or death events when needed
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        // make sure health never goes below zero
        currentHealth = Mathf.Max(0, currentHealth);

        OnDamageTaken?.Invoke(amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    // restores health up to the maximum value
    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        // make sure health never goes above max
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // updates the maximum health cap and clamps current health
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // directly sets the current health while respecting bounds
    public void SetCurrentHealth(int newCurrentHealth)
    {
        currentHealth = Mathf.Clamp(newCurrentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // raises the death event and removes the game object
    void Death()
    {
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    // returns whether the actor has any health remaining
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}
