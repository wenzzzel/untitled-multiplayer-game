using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tip : NetworkBehaviour
{
    // [Header("Input")]
    // [SerializeField] private InputActionAsset inputActions;

    // [Header("Tip Settings")]
    // [SerializeField] private float moveMultiplier = 10f;
    // [SerializeField] private float moveSpeed = 5f;


    // private Vector3 originalPosition;
    // private NetworkVariable<Vector3> networkTargetPosition = new NetworkVariable<Vector3>(
    //     default,
    //     NetworkVariableReadPermission.Everyone,
    //     NetworkVariableWritePermission.Server
    // );
    // private bool tipShouldTravel = false;
    // private InputAction fireAction;


    public void MoveTip()
    {
        Debug.Log("Tip MoveTip called");
    }




















































// #region Lifecycle calls

//     void Awake()
//     {
//         originalPosition = transform.localPosition;
        
//         if (inputActions == null)
//         {
//             Debug.LogError("InputActionAsset not assigned in Hook script.");
//             return;
//         }

//         fireAction = inputActions.FindActionMap("Player").FindAction("Attack");
//     }

//     public override void OnNetworkSpawn()
//     {   
//         base.OnNetworkSpawn();
//         networkTargetPosition.OnValueChanged += OnTargetPositionChanged; // Subscribe to network variable changes

//         if (!IsServer)
//             return;
    
//         networkTargetPosition.Value = originalPosition; // Initialize the network variable on the server
//     }

//     public override void OnNetworkDespawn()
//     {
//         base.OnNetworkDespawn();
//         networkTargetPosition.OnValueChanged -= OnTargetPositionChanged; // Unsubscribe from network variable changes
//     }

//     void OnEnable()
//     {
//         fireAction.performed += MoveTipOnServer;
//         fireAction.Enable();
//     }

//     void OnDisable()
//     {
//         fireAction.performed -= MoveTipOnServer;
//         fireAction.Disable();
//     }

//     void Update()
//     {
//         if (IsTipAtDestination())
//         {
//             ResetTipOnServer();
//             tipShouldTravel = false;
//             return;
//         }

//         if (IsTipFullyRetracted())
//         {
//             tipShouldTravel = false;
//             return;
//         }
        
//         // Hook wasn't fully extended or retracted yet, continue lerping
//         transform.localPosition = Vector3.Lerp(transform.localPosition, networkTargetPosition.Value, Time.deltaTime * moveSpeed);
//     }

// #endregion
// #region Custom methods

//     private bool IsTipAtDestination() => IsTipInSyncWithServer() && tipShouldTravel;

//     private bool IsTipFullyRetracted() => IsTipInSyncWithServer() && !tipShouldTravel;

//         private void OnTargetPositionChanged(Vector3 previousValue, Vector3 newValue)
//     {
//         tipShouldTravel = true;
//     }

//     private bool IsTipInSyncWithServer() => Vector3.Distance(transform.localPosition, networkTargetPosition.Value) < 0.01f;

//     private void ResetTipOnServer()
//     {
//         if (!IsOwner)
//             return;

//         ResetTipOnServerRpc();
//     }

//     private void MoveTipOnServer(InputAction.CallbackContext context)
//     {
//         if (!IsOwner) 
//             return;
        
//         MoveTipOnServerRpc();
//     }

// #endregion
// #region Server RPCs

//     [ServerRpc]
//     private void MoveTipOnServerRpc()
//     {
//         Vector3 targetPosition = new Vector3(originalPosition.x, originalPosition.y * moveMultiplier, originalPosition.z);
//         networkTargetPosition.Value = targetPosition;
//     }

//     [ServerRpc]
//     private void ResetTipOnServerRpc()
//     {
//         networkTargetPosition.Value = originalPosition;
//     }

// #endregion

}