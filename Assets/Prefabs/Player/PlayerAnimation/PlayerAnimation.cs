using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : NetworkBehaviour
{
    private Animator animator;

    [Header("References to other scripts")]
    [SerializeField] private PlayerMovementAnimation playerMovementAnimationScript;
    [SerializeField] private PlayerAttackAnimation playerAttackAnimationScript;
    [SerializeField] private PlayerDeathAnimation playerDeathAnimationScript;
    [SerializeField] private PlayerAnimationHelpers playerAnimationHelpersScript;

#region Lifecycle calls

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

#endregion
#region Public methods

    public void AnimateMovement(bool moving) => playerMovementAnimationScript.AnimateMovement(moving);

    public void AnimateAttack() => playerAttackAnimationScript.AnimateAttack();

    public void AnimateDeath() => playerDeathAnimationScript.AnimateDeath();

    public void DisableAnimator()
    {
        animator.enabled = false;
    }

    public void EnableAnimator()
    {
        animator.enabled = true;
    }

    ///<summary>
    /// Returns the duration of the last played animation.
    /// Always call this from a coroutine after waiting one frame to ensure the value is updated.
    ///</summary>
    public float GetLastAnimationDuration()
    {
        return playerAnimationHelpersScript.GetLastAnimationDuration();
    }

#endregion
#region Coroutines

    private IEnumerator SetAnimationDuration()
    {
        yield return StartCoroutine(playerAnimationHelpersScript.SetAnimationDuration());
    }

    /// <summary>
    /// Sets the animation duration for the attack animation on the server.
    /// Call this on the server before reading GetLastAnimationDuration().
    /// </summary>
    public IEnumerator SetAnimationDurationFromServer()
    {
        playerAttackAnimationScript.AnimateAttack2();

        yield return StartCoroutine(SetAnimationDuration());
    }

#endregion

}
