using Unity.Netcode;
using UnityEngine;

public class HookBody : NetworkBehaviour
{  
    [Header("Hook Settings")]
    [SerializeField] private float stretchMultiplier = 40f;
    [SerializeField] private float stretchSpeed = 10f;
    
    [Header("References to other scripts")]
    [SerializeField] private HookTipMovement tipMovementScript;
    [SerializeField] private HookTipGraple tipGrapleScript;
    [SerializeField] private PlayerMovement playerMovementScript;
    
    private Vector3 originalScale;
    private NetworkVariable<Vector3> networkTargetScale = new NetworkVariable<Vector3>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private bool hookShouldExtend = false;

#region Lifecycle calls

    void Awake()
    {
        originalScale = transform.localScale;

        if (tipMovementScript == null)
            Debug.LogError("TipMovement script reference not assigned in Hook script.");
        
        if (playerMovementScript == null)
            Debug.LogError("PlayerMovement script reference not assigned in Hook script.");
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

    void Update()
    {   
        if (IsHookFullyExtended())
        {
            ResetHookOnServer();
            hookShouldExtend = false;

            EnableMovement();

            return;
        }

        if (IsHookFullyRetracted())
        {
            tipGrapleScript.ReleaseGrappledPlayer();

            EnableMovement();
            
            return;
        }

        DisableMovement();
            
        transform.localScale = Vector3.Lerp(
            transform.localScale, 
            networkTargetScale.Value, 
            Time.deltaTime * stretchSpeed);
    }

#endregion
#region Private methods

    private bool IsHookFullyExtended() => IsHookInSyncWithServer() && hookShouldExtend;

    private bool IsHookFullyRetracted() => IsHookInSyncWithServer() && !hookShouldExtend;

    private void OnTargetScaleChanged(Vector3 previousValue, Vector3 newValue)
    {
        if (newValue == originalScale)
            hookShouldExtend = false;
        else
            hookShouldExtend = true;
        
        tipMovementScript.MoveTip(newValue);
    }

    private bool IsHookInSyncWithServer() => Vector3.Distance(transform.localScale, networkTargetScale.Value) < 0.01f;

    private void DisableMovement() => playerMovementScript.enabled = false;
    private void EnableMovement() => playerMovementScript.enabled = true;

#endregion
#region Public methods

    public void ExtendHookOnServer()
    {
        if (!IsOwner) 
            return;
        
        ExtendHookServerRpc();
    }

    public void ResetHookOnServer()
    {
        if (!IsOwner)
            return;

        ResetHookServerRpc();
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