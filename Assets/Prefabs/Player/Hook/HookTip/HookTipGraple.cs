using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class HookTipGraple : MonoBehaviour
{
    [Header("References to other scripts")]
    [SerializeField] private HookBody hookScript;

    private Transform grappledPlayer = null;
    private Rigidbody2D grappledPlayerRb = null;

#region Lifecycle calls

    void Awake()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;

        if (hookScript == null)
            Debug.LogError("Hook script reference not assigned in HookTipGraple script.");
    }

    void FixedUpdate()
    {
        // Move the grappled player along with the Tip using physics
        if (grappledPlayer != null && grappledPlayerRb != null)
        {
            grappledPlayerRb.MovePosition(transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CollisionIsNotWithAnotherPlayer(other))
            return;

        if (CollisionWithMyself(other.transform))
            return;

        if (grappledPlayer != null) // Already grappling someone
            return;
        
        // Grapple the player
        grappledPlayer = other.transform;
        grappledPlayerRb = other.GetComponent<Rigidbody2D>();
        Debug.Log("Other player hooked and grappled!");

        // Make the hook retract as soon as a grapple hits
        hookScript.ResetHookOnServer();
    }

#endregion
#region Public methods

    public void ReleaseGrappledPlayer()
    {
        if (grappledPlayer != null)
        {
            Debug.Log("Player released!");
            grappledPlayer = null;
            grappledPlayerRb = null;
        }
    }

#endregion
#region Private methods

    private bool CollisionWithMyself(Transform collidingWith)
    {
        return collidingWith.IsChildOf(this.transform) || this.transform.IsChildOf(collidingWith);
    }

    private bool CollisionIsNotWithAnotherPlayer(Collider2D other)
    {
        return !other.CompareTag("Player");
    }

#endregion

}