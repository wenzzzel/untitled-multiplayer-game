using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class MeleeWeapon : NetworkBehaviour
{
    [SerializeField] private float swingDuration = 0.3f; // Duration of the swing in seconds
    private bool isSwinging = false;
    private CircleCollider2D weaponCollider;
    private readonly HashSet<Collider2D> hitThisSwing = new HashSet<Collider2D>();



#region Lifecycle calls

    void Start()
    {
        weaponCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        
    }

#endregion
#region Public methods

    /// <summary>
    /// Initiates a melee weapon swing. Called by the owner client.
    /// Executes locally for instant feedback and syncs to other clients via RPC.
    /// </summary>
    public void Swing()
    {
        if (isSwinging)
            return;
        
        StartCoroutine(SwingCoroutine()); // Execute swing locally for instant feedback
        
        // Notify server and other clients
        if (IsOwner)
        {
            SwingServerRpc();
        }
    }

#endregion
#region Network methods

    /// <summary>
    /// Server RPC called by the owner client when they swing.
    /// Server processes hit detection and notifies all other clients.
    /// </summary>
    [ServerRpc]
    private void SwingServerRpc()
    {
        // Start server-side swing with continuous hit detection
        StartCoroutine(SwingCoroutineWithHitDetection());
        
        // Notify other clients to show the swing animation
        SwingClientRpc();
    }

    /// <summary>
    /// Client RPC to show the swing animation on all non-owner clients.
    /// The owner already executed the swing locally for instant feedback.
    /// </summary>
    [ClientRpc]
    private void SwingClientRpc()
    {
        // Only execute on clients that are NOT the owner
        // (owner already executed swing locally)
        if (!IsOwner && !isSwinging)
        {
            StartCoroutine(SwingCoroutine());
        }
    }

#endregion
#region Private methods

    /// <summary>
    /// Server-side swing coroutine that performs continuous hit detection during the swing.
    /// Only runs on the server to ensure authoritative hit validation.
    /// </summary>
    private IEnumerator SwingCoroutineWithHitDetection()
    {
        isSwinging = true;
        hitThisSwing.Clear(); // Reset hit tracking for new swing
        
        float elapsed = 0f;
        float swingRadius = 0.2f;
        Vector3 originalPosition = Vector3.zero;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / swingDuration;
            float angle = progress * 2f * Mathf.PI; // Full circle (0 to 2π)

            // Calculate position on circle
            float x = Mathf.Cos(angle) * swingRadius;
            float y = Mathf.Sin(angle) * swingRadius;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            // Perform hit detection at this frame
            PerformHitDetection();

            yield return null;
        }

        // Ensure we end at the original position
        transform.localPosition = originalPosition;
        isSwinging = false;
    }

    /// <summary>
    /// Performs hit detection on the server using the weapon's CircleCollider2D.
    /// This runs server-side to ensure authoritative hit validation.
    /// Tracks hits to prevent hitting the same target multiple times in one swing.
    /// </summary>
    private void PerformHitDetection()
    {
        if (weaponCollider == null)
            return;

        // Get the world position and radius of the weapon collider
        Vector2 weaponPosition = transform.position;
        float radius = weaponCollider.radius * transform.lossyScale.x; // Account for scale

        // Only check for hits on the "Player" layer
        int playerLayerMask = LayerMask.GetMask("Player");

        // Perform overlap circle check to detect all colliders in range on the Player layer
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(weaponPosition, radius, playerLayerMask);

        foreach (Collider2D hitCollider in hitColliders)
        {
            // Skip if we hit ourselves (the weapon or the player wielding it)
            if (hitCollider.transform == transform || hitCollider.transform.IsChildOf(transform.parent))
                continue;

            // Skip if we already hit this collider during this swing
            if (hitThisSwing.Contains(hitCollider))
                continue;

            // Mark as hit for this swing
            hitThisSwing.Add(hitCollider);

            // Log the hit
            Debug.Log($"[SERVER] Melee weapon hit: {hitCollider.gameObject.name} at position {hitCollider.transform.position}");

            // Apply damage to the hit object here when you implement a Health system
        }
    }

    private IEnumerator SwingCoroutine()
    {
        isSwinging = true;
        float elapsed = 0f;
        float swingRadius = 0.2f;
        Vector3 originalPosition = Vector3.zero;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / swingDuration;
            float angle = progress * 2f * Mathf.PI; // Full circle (0 to 2π)

            // Calculate position on circle
            float x = Mathf.Cos(angle) * swingRadius;
            float y = Mathf.Sin(angle) * swingRadius;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            yield return null;
        }

        // Ensure we end at the original position
        transform.localPosition = originalPosition;
        isSwinging = false;
    }

#endregion

}
