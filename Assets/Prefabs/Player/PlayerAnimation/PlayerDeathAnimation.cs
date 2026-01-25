using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerDeathAnimation : NetworkBehaviour
{
    private Animator animator;

    [Header("References to other scripts")]
    [SerializeField] private PlayerAnimationHelpers playerAnimationHelpersScript;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public IEnumerator AnimateDeath(AnimationResult result) 
    {
        animator.Play("Dust 2 Animation");
        yield return StartCoroutine(playerAnimationHelpersScript.SetAnimationDuration());
        result.Duration = playerAnimationHelpersScript.GetLastAnimationDuration();
    }
}
