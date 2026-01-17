using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class NetworkFlipHorizontalToFaceMouse : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;
    private NetworkVariable<bool> isFlipped = new NetworkVariable<bool>(
        false, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner
    );

#region Lifecycle calls

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
            Debug.LogError("NetworkFlipHorizontalToFaceMouse requires a SpriteRenderer component!");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        isFlipped.OnValueChanged += OnFlipChanged;
        
        spriteRenderer.flipX = isFlipped.Value;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        isFlipped.OnValueChanged -= OnFlipChanged;
    }

    void Update()
    {
        if (!(IsOwner && IsClient))
            return;

        FlipToFaceMouse();
    }

#endregion
#region Private methods

    private void FlipToFaceMouse()
    {
        var mouseWorldPosition = GetMousePositionInWorld();
        
        var direction = mouseWorldPosition - transform.position;
        
        bool shouldFlipLeft = direction.x < 0;
        
        if (isFlipped.Value != shouldFlipLeft)
            isFlipped.Value = shouldFlipLeft;
    }

    private void OnFlipChanged(bool previousValue, bool newValue)
    {
        spriteRenderer.flipX = newValue;
    }

    private Vector3 GetMousePositionInWorld()
    {
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPosition.z = 0f; // Keep on the same Z plane

        return mouseWorldPosition;
    }

#endregion

}
