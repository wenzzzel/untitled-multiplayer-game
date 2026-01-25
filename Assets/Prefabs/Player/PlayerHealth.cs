using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("References to other scripts")]
    [SerializeField] private HealthBar healthBarScript;
    [SerializeField] private PlayerAnimation playerAnimationScript;


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

#endregion
#region Private methods

    private void Die()
    {
        StartCoroutine(DespawnCoroutine());

        StartCoroutine(RespawnCoroutine());
    }

    private void Respawn() //TODO: Just a dummy method for now...
    {
        playerAnimationScript.EnableAnimator();
        Debug.Log("Respawned player.");
    }

    private void Despawn()
    {
        playerAnimationScript.DisableAnimator(); //TODO: Just a dummy method for now...
        Debug.Log("Despawned player.");
    }

    private void OnCurrentHealthChanged(int previousValue, int newHealthValue)
    {
        healthBarScript.UpdateHealthBar(newHealthValue);

        if (newHealthValue <= 0)
            Die();
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
#region Coroutines

    private IEnumerator DespawnCoroutine()
    {   
        var result = new AnimationResult();
        yield return StartCoroutine(playerAnimationScript.AnimateDeath(result));

        yield return new WaitForSeconds(result.Duration);

        Despawn();
    }

    private IEnumerator RespawnCoroutine()
    {
        Debug.Log("Waiting for 10 seconds before respawning...");
        var respawnTime = 10f;
        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

#endregion
}
