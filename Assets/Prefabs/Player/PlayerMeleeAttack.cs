using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMeleeAttack : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("References to other scripts")]
    [SerializeField] private MeleeWeapon meleeWeaponScript;

    private InputAction attackAction;

#region Lifecycle calls

    void Awake()
    {
        if (inputActions == null)
            Debug.LogError("InputActionAsset not assigned in PlayerMeleeAttack script.");

        attackAction = inputActions.FindActionMap("Player").FindAction("Melee Attack");
    }

    private void OnEnable()
    {
        attackAction.performed += OnAttackPerformed;
        attackAction.Enable();
    }

    private void OnDisable()
    {
        attackAction.performed -= OnAttackPerformed;
        attackAction.Disable();
    }

#endregion
#region Private methods

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;
            
        meleeWeaponScript.SwingWeapon();
    }

#endregion

}
