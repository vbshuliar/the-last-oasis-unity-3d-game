using System;
using UnityEngine;

// handles health and damage for player and enemies
public class Actor : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth { get; private set; } // private set means only this class can change it

    // events notify other scripts when health changes or actor dies
    public event Action<Actor> OnDeath;
    public event Action<int, int> OnHealthChanged; // passes current health and max health
    public event Action<int> OnDamageTaken;

    void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth); // mathf max ensures health never goes below 0

        OnDamageTaken?.Invoke(amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth); // mathf min ensures health never exceeds max
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Death()
    {
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}
