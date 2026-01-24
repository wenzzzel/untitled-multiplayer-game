using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerMovementAnimation : NetworkBehaviour
{
    private Animator animator;
    private readonly NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private string currentAnimation = "";
    private const string RUN_ANIMATION_NAME = "Warrior_Run_Blue 0";
    private const string IDLE_ANIMATION_NAME = "Warrior_Idle_Blue";
    private const string ATTACK_ANIMATION_NAME = "Warrior_Attack_Blue";

#region Lifecycle calls

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsAttackAnimationPlaying())
        {
            AnimateMovementLocally();
        }
    }

#endregion
#region Private methods

    private bool IsAttackAnimationPlaying()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(ATTACK_ANIMATION_NAME);
    }

    private void AnimateMovementLocally()
    {
        string targetAnimation;
        
        if (isMoving.Value)
            targetAnimation = RUN_ANIMATION_NAME;
        else
            targetAnimation = IDLE_ANIMATION_NAME;
        
        // Only play if we're switching to a different animation
        if (currentAnimation != targetAnimation)
        {
            currentAnimation = targetAnimation;
            animator.Play(targetAnimation);
        }
    }

#endregion
#region Public methods

    public void AnimateMovement(bool moving)
    {
        if (isMoving.Value != moving)
            isMoving.Value = moving;
    }

#endregion

}
