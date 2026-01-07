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
    [SerializeField] private Tip tipScript; //TODO: Make required
    
    private Vector3 originalScale;
    private NetworkVariable<Vector3> networkTargetScale = new NetworkVariable<Vector3>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private bool hookShouldExtend = false;
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

    void OnEnable()
    {
        fireAction.performed += ExtendHookOnServer;
        fireAction.Enable();
    }

    void OnDisable()
    {
        fireAction.performed -= ExtendHookOnServer;
        fireAction.Disable();
    }

    void Update()
    {   
        if (IsHookFullyExtended())
        {
            ResetHookOnServer();
            hookShouldExtend = false;
            return;
        }

        if (IsHookFullyRetracted())
        {
            hookShouldExtend = false;
            return;
        }
        
        // Hook wasn't fully extended or retracted yet, continue lerping
        transform.localScale = Vector3.Lerp(transform.localScale, networkTargetScale.Value, Time.deltaTime * stretchSpeed);
    }

#endregion
#region Custom methods

    private bool IsHookFullyExtended() => IsHookInSyncWithServer() && hookShouldExtend;

    private bool IsHookFullyRetracted() => IsHookInSyncWithServer() && !hookShouldExtend;

    private void OnTargetScaleChanged(Vector3 previousValue, Vector3 newValue)
    {
        hookShouldExtend = true;
        tipScript.MoveTip(); //TODO: Maybe not the correct place to control the tip, but now we now we can access it at least
    }

    private bool IsHookInSyncWithServer() => Vector3.Distance(transform.localScale, networkTargetScale.Value) < 0.01f;

    private void ResetHookOnServer()
    {
        if (!IsOwner)
            return;

        ResetHookServerRpc();
    }

    private void ExtendHookOnServer(InputAction.CallbackContext context)
    {
        if (!IsOwner) 
            return;
        
        ExtendHookServerRpc();
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