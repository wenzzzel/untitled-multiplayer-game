using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Player Settings")]
    [SerializeField] private float speed = 5f;

    private Vector2 _velocity;
    private Vector2 _position;
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

    void OnEnable()
    {
        if (moveAction == null)
        {
            Debug.LogError("Move action not found in PlayerMovement script, so cannot enable.");
            return;
        }

        moveAction.performed += OnMove;
        moveAction.Enable();
    }

    void OnDisable()
    {
        if (moveAction == null)
        {
            Debug.LogError("Move action not found in PlayerMovement script, so cannot disable.");
            return;
        }

        moveAction.performed -= OnMove;
        moveAction.Disable();
    }

    private void Update()
    {
        if (!(IsOwner && IsClient))
            return;
        
        var input = moveAction.ReadValue<Vector2>();
        input = input.normalized;

        if (input.sqrMagnitude > 0f)
            SendInputServerRpc(input, Time.deltaTime);
    }

    [ServerRpc]
    private void SendInputServerRpc(Vector2 input, float dt)
    {
        // Simple authoritative movement: server applies input
        _velocity = input * speed;
        _position = (Vector2)transform.position + _velocity * dt;

        // Clamp to play area (optional)
        _position.x = Mathf.Clamp(_position.x, -8f, 8f);
        _position.y = Mathf.Clamp(_position.y, -4.5f, 4.5f);

        transform.position = _position;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        
        _position = new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
        transform.position = _position;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _velocity = context.ReadValue<Vector2>() * speed;
    }
}
