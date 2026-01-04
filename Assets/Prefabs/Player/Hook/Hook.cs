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
    private Vector3 targetScale;
    private bool isStretching = false;
    private InputAction fireAction;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset not assigned in Hook script.");
            return;
        }

        fireAction = inputActions.FindActionMap("Player").FindAction("Attack");
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
        if (!(IsOwner && IsClient))
            return;

        // Smoothly interpolate to the target scale
        if (isStretching)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * stretchSpeed);
            
            // Check if we're close enough to the target scale
            if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
            {
                transform.localScale = targetScale;
                isStretching = false;
            }
        }
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        // Stretch the hook (10x length in Y axis, keep X and Z the same)
        targetScale = new Vector3(originalScale.x, originalScale.y * stretchMultiplier, originalScale.z);
        isStretching = true;
    }

    // Public method to reset the hook to its original size
    public void ResetHook()
    {
        targetScale = originalScale;
        isStretching = true;
    }
}
