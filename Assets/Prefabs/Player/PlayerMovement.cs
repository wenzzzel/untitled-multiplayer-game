using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Player Settings")]
    [SerializeField] private float speed = 5f;

    private InputAction moveAction;

    void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset not assigned in PlayerMovement script.");
            return;
        }

        moveAction = inputActions.FindActionMap("Player").FindAction("Move");
    }

    private void Update()
    {
        if (!(IsOwner && IsClient))
            return;
        
        var input = moveAction.ReadValue<Vector2>().normalized;

        if (input.sqrMagnitude > 0f) // Only send changes to server if something actually changed
            MovePlayerServerRpc(input, Time.deltaTime);
    }

    [ServerRpc]
    private void MovePlayerServerRpc(Vector2 input, float deltaTime)
    {
        var velocity = input * speed;
        var position = (Vector2)transform.position + velocity * deltaTime;

        transform.position = position;
    }
}
