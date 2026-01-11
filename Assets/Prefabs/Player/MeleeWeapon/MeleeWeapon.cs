using System.Collections;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class MeleeWeapon : NetworkBehaviour
{
    [SerializeField] private float swingDuration = 0.3f; // Duration of the swing in seconds
    private bool isSwinging = false;



#region Lifecycle calls

    void Start()
    {
        
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
        
        // Execute swing locally for instant feedback
        StartCoroutine(SwingCoroutine());
        
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
        // Server-side hit detection and damage logic would go here
        // For now, we just broadcast the visual to other clients
        
        // Notify all clients except the one who initiated the swing
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
