using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerDeathAnimation : NetworkBehaviour
{
    private Animator animator;
    private const string DEATH_ANIMATION_NAME = "Dust 2 Animation";

    [Header("References to other scripts")]
    [SerializeField] private PlayerAnimationHelpers playerAnimationHelpersScript;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (playerAnimationHelpersScript == null)
            Debug.LogError("PlayerAnimationHelpers script not assigned in PlayerDeathAnimation script.");
    }

    public IEnumerator AnimateDeath(AnimationResult result) 
    {
        animator.Play(DEATH_ANIMATION_NAME);
        yield return StartCoroutine(playerAnimationHelpersScript.SetAnimationDuration());
        result.Duration = playerAnimationHelpersScript.GetLastAnimationDuration();
    }
}
