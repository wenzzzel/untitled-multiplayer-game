using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
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
        
        RotateTowardsMouse();
    }

    private void RotateTowardsMouse()
    {
        var mouseWorldPosition = GetMousePositionInWorld();
        
        // Calculate direction from player to mouse
        var direction = (mouseWorldPosition - transform.position).normalized;
        
        // Calculate angle in degrees
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Apply rotation (subtract 90 if your sprite faces up by default)
        var targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        
        RotatePlayerServerRpc(targetRotation);
    }

    private Vector3 GetMousePositionInWorld()
    {
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPosition.z = 0f; // Keep on the same Z plane as player

        return mouseWorldPosition;
    }

    [ServerRpc]
    private void MovePlayerServerRpc(Vector2 input, float deltaTime)
    {
        var velocity = input * speed;
        var position = (Vector2)transform.position + velocity * deltaTime;

        transform.position = position;
    }

    [ServerRpc]
    private void RotatePlayerServerRpc(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
}
