using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : NetworkBehaviour
{
    private Animator animator;
    private NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

#region Lifecycle calls

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        AnimateMovementLocally();
    }

#endregion
#region Private methods

    private void AnimateMovementLocally()
    {
        if (isMoving.Value)
            animator.Play("Warrior_Run_Blue 0");
        else
            animator.Play("Warrior_Idle_Blue");
    }

#endregion
#region Public methods

    public void AnimateMovement(bool moving)
    {
        if (isMoving.Value != moving)
            isMoving.Value = moving;
        
        if(isMoving.Value)
            animator.Play("Warrior_Run_Blue 0");
        else
            animator.Play("Warrior_Idle_Blue");
    }

#endregion
    
}
