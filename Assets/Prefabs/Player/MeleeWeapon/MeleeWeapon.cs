using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class MeleeWeapon : NetworkBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private float swingDuration = 0.2f;
    [SerializeField] private float swingRadius = 0.4f;
    [SerializeField] private int damage = 10;

    [Header("References to other scripts")]
    [SerializeField] private PlayerAnimation playerAnimationScript;

    private bool isSwinging = false;
    private CircleCollider2D weaponCollider;
    private readonly HashSet<Collider2D> hitsThisSwing = new HashSet<Collider2D>();

#region Lifecycle calls

    void Start()
    {
        weaponCollider = GetComponent<CircleCollider2D>();

        if (playerAnimationScript == null)
            Debug.LogError("PlayerAnimation script not assigned in MeleeWeapon script.");
    }

#endregion
#region Public methods

    public void SwingWeapon()
    {
        if (isSwinging)
            return;
        
        playerAnimationScript.PlayAttackAnimation();
        
        StartCoroutine(Swing(runningOnServer: false)); // Execute swing locally for instant feedback
        
        if (IsOwner)
            SwingServerRpc();

    }

#endregion
#region Network methods

    [ServerRpc]
    private void SwingServerRpc()
    {
        StartCoroutine(Swing(runningOnServer: true));
        
        SwingClientRpc();
    }

    [ClientRpc]
    private void SwingClientRpc()
    {
        // Only execute on clients that are NOT the owner (owner already executed swing locally)
        if (!IsOwner && !isSwinging)
        {
            StartCoroutine(Swing(runningOnServer: false));
        }
    }

#endregion
#region Private methods

    private IEnumerator Swing(bool runningOnServer)
    {
        // Prep state before swing
        isSwinging = true;
        var originalPosition = Vector3.zero;
        var elapsed = 0f;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            var progress = elapsed / swingDuration;
            var angle = progress * 2f * Mathf.PI; // Full circle (0 to 2π)

            // Calculate position on circle
            var x = Mathf.Cos(angle) * swingRadius;
            var y = Mathf.Sin(angle) * swingRadius;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            if (runningOnServer)
                PerformHitDetectionOnServer();

            yield return null;
        }

        // Reset state after swing
        isSwinging = false;
        transform.localPosition = originalPosition;
        if (runningOnServer)
            hitsThisSwing.Clear();    
    }

    private void PerformHitDetectionOnServer()
    {
        if (weaponCollider == null)
            return;

        var weaponColliderRadius = weaponCollider.radius * transform.lossyScale.x;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, weaponColliderRadius, LayerMask.GetMask("Player"));

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (HitOurSelf(hitCollider) || AlreadyHitThisTarget(hitCollider))
                continue;

            hitsThisSwing.Add(hitCollider);

            hitCollider.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }

    private bool HitOurSelf(Collider2D hitCollider) => hitCollider.transform == transform || hitCollider.transform.IsChildOf(transform.parent);

    private bool AlreadyHitThisTarget(Collider2D hitCollider) => hitsThisSwing.Contains(hitCollider);

#endregion

}
