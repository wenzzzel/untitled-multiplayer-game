using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    private Vector2 _serverVelocity;

    // Optional: track on server for cheats/lag handling
    private Vector2 _position;

    private void Update()
    {
        if (IsOwner && IsClient)
        {
            // Gather input locally (client) using new Input System
            Vector2 input = Vector2.zero;
            
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                    input.y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                    input.y -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    input.x += 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    input.x -= 1f;
            }
            
            input = input.normalized;
            if (input.sqrMagnitude > 0f)
                SendInputServerRpc(input, Time.deltaTime);
        }
    }

    [ServerRpc]
    private void SendInputServerRpc(Vector2 input, float dt)
    {
        // Simple authoritative movement: server applies input
        _serverVelocity = input * speed;
        _position = (Vector2)transform.position + _serverVelocity * dt;

        // Clamp to play area (optional)
        _position.x = Mathf.Clamp(_position.x, -8f, 8f);
        _position.y = Mathf.Clamp(_position.y, -4.5f, 4.5f);

        transform.position = _position;     // If using NetworkTransform
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Spawn at random positions (optional)
            _position = new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
            transform.position = _position;
        }
    }
}
