using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : NetworkBehaviour
{
    private Animator animator;
    private readonly NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<int> attackTriggerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private int lastAttackTriggerCount = 0;
    private string currentAnimation = "";

    public float lastAnimationDuration = 0f;

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

    void Update()
    {
        // Check if attack animation is currently playing
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlayingAttack = stateInfo.IsName("Warrior_Attack_Blue");

        bool blockMovementForAttack = isPlayingAttack;

        // Only reset to movement if attack is truly done
        if (!blockMovementForAttack)
        {
            AnimateMovementLocally();
        }
    }

#endregion
#region Private methods

    private void AnimateMovementLocally()
    {
        string targetAnimation;
        
        if (isMoving.Value)
            targetAnimation = "Warrior_Run_Blue 0";
        else
            targetAnimation = "Warrior_Idle_Blue";
        
        // Only play if we're switching to a different animation
        if (currentAnimation != targetAnimation)
        {
            currentAnimation = targetAnimation;
            animator.Play(targetAnimation);
        }
    }

    private void OnAttackTriggerCountChanged(int previousValue, int newValue)
    {
        // Only play if the count actually changed (means a new attack was triggered)
        if (newValue != lastAttackTriggerCount && !IsOwner)
        {
            lastAttackTriggerCount = newValue;
            animator.Play("Warrior_Attack_Blue");
        }
    }

#endregion
#region Public methods

    public void AnimateMovement(bool moving)
    {
        if (isMoving.Value != moving)
            isMoving.Value = moving;
    }

    public void PlayAttackAnimation()
    {
        if (!IsOwner)
            return;

        // Play locally immediately for instant feedback
        currentAnimation = "Warrior_Attack_Blue";
        animator.Play("Warrior_Attack_Blue");

        // Increment counter to trigger animation on all other clients
        attackTriggerCount.Value++;

        StartCoroutine(SetAnimationDuration());
    }

    public float PlayDeathAnimation()
    {
        animator.Play("Dust 2 Animation");
        var animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        return animationLength;
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
#region Coroutines

    public IEnumerator SetAnimationDuration()
    {
        // Wait one frame to ensure animator state is updated
        yield return null;

        // Update the public field with the duration
        lastAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
    }

#endregion
    
}
