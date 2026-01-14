using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : NetworkBehaviour
{
    private Animator animator;
    private NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Player Settings")]
    [SerializeField] private float speed = 5f;

    private InputAction moveAction;


    void Awake()
    {
        animator = GetComponent<Animator>();
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
            if (isMoving.Value != moving)
                isMoving.Value = moving;
            if (moving)
                MovePlayerServerRpc(input, Time.deltaTime);
        }

        // Animate based on networked movement state
        if (isMoving.Value)
            animator.Play("Warrior_Run_Blue 0");
        else
            animator.Play("Warrior_Idle_Blue");
    }

    [ServerRpc]
    private void MovePlayerServerRpc(Vector2 input, float deltaTime)
    {
        var velocity = input * speed;
        var position = (Vector2)transform.position + velocity * deltaTime;

        transform.position = position;
    }
}
