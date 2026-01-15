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

    [Header("References to other scripts")]
    [SerializeField] private PlayerAnimation playerAnimationScript;

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
        if (IsOwner && IsClient)
        {
            var input = moveAction.ReadValue<Vector2>().normalized;
            bool moving = input.sqrMagnitude > 0f;

            playerAnimationScript.AnimateMovement(moving);

            if (moving)
                MovePlayerServerRpc(input, Time.deltaTime);
        }
    }

    [ServerRpc]
    private void MovePlayerServerRpc(Vector2 input, float deltaTime)
    {
        var velocity = input * speed;
        var position = (Vector2)transform.position + velocity * deltaTime;

        transform.position = position;
    }
}
