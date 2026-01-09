using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("References to other scripts")]
    [SerializeField] private HealthBar healthBarScript;

    public int currentHealth;

#region Lifecycle calls

    void Start()
    {
        if (healthBarScript == null)
        {
            Debug.LogError("HealthBar script reference not assigned in Health script.");
        }

        currentHealth = maxHealth;
    }

    void Update()
    {
        
    }

#endregion
#region Private methods

    private void Die()
    {
        Debug.Log("Player has died.");
        // Add respawn or game over logic here
    }

#endregion
#region Public methods

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Player took {damageAmount} damage. Current health: {currentHealth}");

        healthBarScript.UpdateHealthBar(damageAmount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

#endregion
}
