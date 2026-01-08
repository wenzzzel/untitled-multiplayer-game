using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class RotateToFaceMouse : NetworkBehaviour
{
    void Update()
    {
        if (!(IsOwner && IsClient))
            return;

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
    private void RotatePlayerServerRpc(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
}
