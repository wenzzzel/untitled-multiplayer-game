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

    // Keeps track of the duration of the last played animation. 
    // Can be used by others scripts that want to wait until the animation is done.
    private float lastAnimationDuration = 0f;

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
            StartCoroutine(SetAnimationDuration());
        }
    }

#endregion
#region Public methods

    public void AnimateMovement(bool moving)
    {
        if (isMoving.Value != moving)
            isMoving.Value = moving;
    }

    public void AnimateAttack()
    {
        if (!IsOwner) //TODO: Not sure if this is needed
            return;

        // Play locally immediately for instant feedback
        currentAnimation = "Warrior_Attack_Blue";
        animator.Play("Warrior_Attack_Blue");

        // Increment counter to trigger animation on all other clients
        attackTriggerCount.Value++;

        StartCoroutine(SetAnimationDuration());
    }

    public void AnimateDeath()
    {
        animator.Play("Dust 2 Animation");
        
        StartCoroutine(SetAnimationDuration());
    }

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
        return lastAnimationDuration;
    }

#endregion
#region Coroutines

    private IEnumerator SetAnimationDuration()
    {
        // Wait one frame to ensure animator state is updated
        yield return null;

        // Update the public field with the duration
        var duration = animator.GetCurrentAnimatorStateInfo(0).length;
        lastAnimationDuration = duration;
        Debug.Log($"Set lastAnimationDuration to {duration} seconds.");
    }

    /// <summary>
    /// Sets the animation duration for the attack animation on the server.
    /// Call this on the server before reading GetLastAnimationDuration().
    /// </summary>
    public IEnumerator SetAnimationDurationFromServer()
    {
        // Play the attack animation on the server so we can read its duration
        animator.Play("Warrior_Attack_Blue");
        
        // Wait one frame to ensure animator state is updated
        yield return null;

        // Update the field with the duration
        var duration = animator.GetCurrentAnimatorStateInfo(0).length;
        lastAnimationDuration = duration;
        Debug.Log($"Server: Set lastAnimationDuration to {duration} seconds.");
    }

#endregion

    
}
