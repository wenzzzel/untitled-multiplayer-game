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

    public void AnimateDeath()
    {
        animator.Play("Dust 2 Animation");
        StartCoroutine(playerAnimationHelpersScript.SetAnimationDuration());
    }
}
