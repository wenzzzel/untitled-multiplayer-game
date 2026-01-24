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

    public void AnimateAttack()
    {
        if (!IsOwner) //TODO: Not sure if this is needed
            return;

        // Play locally immediately for instant feedback
        animator.Play(ATTACK_ANIMATION_NAME);
        StartCoroutine(playerAnimationHelpersScript.SetAnimationDuration());

        // Increment counter to trigger animation on all other clients
        attackTriggerCount.Value++;
    }

    public void AnimateAttack2()
    {
        animator.Play(ATTACK_ANIMATION_NAME);
    }

#endregion

}
