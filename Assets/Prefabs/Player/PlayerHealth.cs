using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("References to other scripts")]
    [SerializeField] private HealthBar healthBarScript;


    private NetworkVariable<int> networkCurrentHealth = new NetworkVariable<int>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

#region Lifecycle calls

    void Start()
    {
        if (healthBarScript == null)
            Debug.LogError("HealthBar script reference not assigned in Health script.");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        networkCurrentHealth.OnValueChanged += OnCurrentHealthChanged; // Subscribe to network variable changes

        if (!IsServer)
            return;

        networkCurrentHealth.Value = maxHealth; // Initialize the network variable on the server
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

    private void OnCurrentHealthChanged(int previousValue, int newHealthValue)
    {
        Debug.Log($"Current health updated via network variable: {newHealthValue}");
        healthBarScript.UpdateHealthBar(newHealthValue);
    }

#endregion
#region Public methods

    public void TakeDamage(int damageAmount)
    {
        if (!IsServer)
            return;
        
        networkCurrentHealth.Value = Math.Max(0, networkCurrentHealth.Value - damageAmount);
    }

#endregion
}
