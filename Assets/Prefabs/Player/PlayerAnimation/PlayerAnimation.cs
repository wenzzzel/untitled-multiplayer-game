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

#region Lifecycle calls

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

#endregion
#region Public methods

    public void AnimateMovement(bool moving) => playerMovementAnimationScript.AnimateMovement(moving);

    public IEnumerator AnimateAttack(AnimationResult result)
    {
        yield return StartCoroutine(playerAttackAnimationScript.AnimateAttack(result));
    }

    public IEnumerator AnimateAttackOnServer(AnimationResult result)
    {
        yield return StartCoroutine(playerAttackAnimationScript.AnimateAttackOnServer(result));
    }

    public IEnumerator AnimateDeath(AnimationResult result) 
    {
        yield return StartCoroutine(playerDeathAnimationScript.AnimateDeath(result));
    }

    public void DisableAnimator()
    {
        animator.enabled = false;
    }

    public void EnableAnimator()
    {
        animator.enabled = true;
    }

#endregion

}
