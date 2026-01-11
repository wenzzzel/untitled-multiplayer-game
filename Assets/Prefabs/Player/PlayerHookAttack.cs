using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHookAttack : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("References to other scripts")]
    [SerializeField] private Hook hookScript;

    private InputAction fireAction;
    
#region Lifecycle calls

    void Awake()
    {
        fireAction = inputActions.FindActionMap("Player").FindAction("Attack");
    }

    void OnEnable()
    {
        fireAction.performed += ExtendHook;
        fireAction.Enable();
    }

    void OnDisable()
    {
        fireAction.performed -= ExtendHook;
        fireAction.Disable();
    }

#endregion
#region Private methods

    private void ExtendHook(InputAction.CallbackContext context)
    {
        if (IsOwner)
            hookScript.ExtendHookOnServer();
    }

#endregion

}
