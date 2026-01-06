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
    private bool isStretching = false;
    private InputAction fireAction;

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
        // base.OnNetworkSpawn();
        
        // Subscribe to network variable changes
        networkTargetScale.OnValueChanged += OnTargetScaleChanged;
        
        // Initialize the network variable on the server
        if (!IsServer)
            return;

        networkTargetScale.Value = originalScale;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        networkTargetScale.OnValueChanged -= OnTargetScaleChanged;
    }

    void OnEnable()
    {
        if (fireAction == null)
        {
            Debug.LogError("Fire action not found in Hook script, so cannot enable.");
            return;
        }

        fireAction.performed += OnFire;
        fireAction.Enable();
    }

    void OnDisable()
    {
        if (fireAction == null)
        {
            Debug.LogError("Fire action not found in Hook script, so cannot disable.");
            return;
        }

        fireAction.performed -= OnFire;
        fireAction.Disable();
    }

    void Update()
    {
        // Smoothly interpolate to the target scale (happens on all clients)
        if (isStretching)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, networkTargetScale.Value, Time.deltaTime * stretchSpeed);
            
            // Check if we're close enough to the target scale
            if (Vector3.Distance(transform.localScale, networkTargetScale.Value) < 0.01f)
            {
                transform.localScale = networkTargetScale.Value;
                isStretching = false;
                
                // Only the owner resets the hook
                if (IsOwner && transform.localScale != originalScale)
                {
                    ResetHookServerRpc();
                }
            }
        }
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        // Only the owner can fire the hook
        if (!IsOwner) return;
        
        // Tell the server to fire the hook
        FireHookServerRpc();
    }

    [ServerRpc]
    private void FireHookServerRpc()
    {
        // Server updates the network variable, which syncs to all clients
        Vector3 stretchedScale = new Vector3(originalScale.x, originalScale.y * stretchMultiplier, originalScale.z);
        networkTargetScale.Value = stretchedScale;
    }

    [ServerRpc]
    private void ResetHookServerRpc()
    {
        // Server resets the hook
        networkTargetScale.Value = originalScale;
    }

    private void OnTargetScaleChanged(Vector3 previousValue, Vector3 newValue)
    {
        // When the network variable changes, start stretching/resetting
        isStretching = true;
    }

    // Public method to reset the hook to its original size
    public void ResetHook()
    {
        if (IsServer)
        {
            networkTargetScale.Value = originalScale;
        }
        else if (IsOwner)
        {
            ResetHookServerRpc();
        }
    }
}
