using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttackAnimation : NetworkBehaviour
{
    private Animator animator;
    private readonly NetworkVariable<int> attackTriggerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("References to other scripts")]
    [SerializeField] private PlayerAnimationHelpers playerAnimationHelpersScript;

    private const string ATTACK_ANIMATION_NAME = "Warrior_Attack_Blue";


#region Lifecycle calls

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        attackTriggerCount.OnValueChanged += OnAttackTriggerCountChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        attackTriggerCount.OnValueChanged -= OnAttackTriggerCountChanged;
    }

#endregion
#region Private methods

    private void OnAttackTriggerCountChanged(int previousValue, int newValue)
    {
        animator.Play(ATTACK_ANIMATION_NAME);
        StartCoroutine(playerAnimationHelpersScript.SetAnimationDuration());
    }

#endregion
#region Public methods

    /// <summary>
    /// Animates the attack, waits one frame, and returns the animation duration via the result object.
    /// Usage: yield return StartCoroutine(AnimateAttack(result)); then read result.Duration
    /// </summary>
    public IEnumerator AnimateAttack(AnimationResult result)
    {
        if (!IsOwner)
        {
            result.Duration = 0f;
            yield break;
        }

        // Play locally immediately for instant feedback
        animator.Play(ATTACK_ANIMATION_NAME);

        // Increment counter to trigger animation on all other clients
        attackTriggerCount.Value++;

        // Wait one frame and set the animation duration
        yield return StartCoroutine(playerAnimationHelpersScript.SetAnimationDuration());

        // Set the result
        result.Duration = playerAnimationHelpersScript.GetLastAnimationDuration();
    }

    /// <summary>
    /// Plays the attack animation without waiting. Used by the server.
    /// </summary>
    public IEnumerator AnimateAttackOnServer(AnimationResult result)
    {
        animator.Play(ATTACK_ANIMATION_NAME);

        // Wait one frame and set the animation duration
        yield return StartCoroutine(playerAnimationHelpersScript.SetAnimationDuration());

        // Set the result
        result.Duration = playerAnimationHelpersScript.GetLastAnimationDuration();
    }

#endregion

}
