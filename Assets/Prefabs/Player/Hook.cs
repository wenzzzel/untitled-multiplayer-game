using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hook : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    
    [Header("Hook Settings")]
    [SerializeField] private float stretchMultiplier = 10f;
    [SerializeField] private float stretchSpeed = 5f;
    
    private Vector3 originalScale;
    private NetworkVariable<Vector3> networkTargetScale = new NetworkVariable<Vector3>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private bool hookShouldExtend = false; //TODO: The name on this bool is incorrect. It's more like isExtendingOrRetracting. Also, it's always being 'false' except when hook is extending, then it's 'false' and 'true' every other frame
    private InputAction fireAction;

#region Lifecycle calls

    void Awake()
    {
        originalScale = transform.localScale;
        
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset not assigned in Hook script.");
            return;
        }

        fireAction = inputActions.FindActionMap("Player").FindAction("Attack");
    }

    public override void OnNetworkSpawn()
    {   
        base.OnNetworkSpawn();
        networkTargetScale.OnValueChanged += OnTargetScaleChanged; // Subscribe to network variable changes

        if (!IsServer)
            return;

        networkTargetScale.Value = originalScale; // Initialize the network variable on the server
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        networkTargetScale.OnValueChanged -= OnTargetScaleChanged; // Unsubscribe from network variable changes
    }

    void OnEnable() => SetFireActionSubscriptionState(true);

    void OnDisable() => SetFireActionSubscriptionState(false);

    void Update()
    {
        Debug.Log(hookShouldExtend);

        if (!hookShouldExtend)
            return;
        
        ExtendHookLocally();
        
        if (HookNotFullyExtended())
            return;
        
        ResetHook();
    }

#endregion
#region Custom methods

    private void OnTargetScaleChanged(Vector3 previousValue, Vector3 newValue)
    {
        hookShouldExtend = true;
    }

    private void ExtendHookLocally()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, networkTargetScale.Value, Time.deltaTime * stretchSpeed);
    }

    private void ExtendHook(InputAction.CallbackContext context)
    {
        if (!IsOwner) 
            return;
        
        ExtendHookServerRpc();
    }

    private void ResetHook()
    {
        transform.localScale = networkTargetScale.Value;
        hookShouldExtend = false;
        
        // Only the owner resets the hook
        if (IsOwner && transform.localScale != originalScale)
        {
            ResetHookServerRpc();
        }
    }

    private bool HookNotFullyExtended() => !(Vector3.Distance(transform.localScale, networkTargetScale.Value) < 0.01f);

#endregion
#region Button click subscription

    private void SetFireActionSubscriptionState(bool on)
    {
        if (on)
        {
            fireAction.performed += ExtendHook;
            fireAction.Enable();
        }
        else
        {
            fireAction.performed -= ExtendHook;
            fireAction.Disable();
        }
    }

#endregion
#region Server RPCs

    [ServerRpc]
    private void ExtendHookServerRpc()
    {
        Vector3 stretchedScale = new Vector3(originalScale.x, originalScale.y * stretchMultiplier, originalScale.z);
        networkTargetScale.Value = stretchedScale;
    }

    [ServerRpc]
    private void ResetHookServerRpc()
    {
        networkTargetScale.Value = originalScale;
    }

#endregion

}