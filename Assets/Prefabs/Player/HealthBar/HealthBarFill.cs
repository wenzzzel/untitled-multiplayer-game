using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthBarFill : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private float fullHealthSize = 1;
    [SerializeField] private float stepHealthSize = 0.01f;

    private float currentHealthSize;


    private SpriteRenderer spriteRenderer;
    
#region Lifecycle calls

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(fullHealthSize, transform.localScale.y, transform.localScale.z);
        currentHealthSize = fullHealthSize;
    }

#endregion
#region Public methods

    public void UpdateFill(int currentHealth) //TODO: This one needs to update on the server and all clients
    {
        var newSize = Mathf.Max(0f, currentHealth * stepHealthSize);
        Debug.Log($"Updating health bar size to: {newSize}");
        transform.localScale = new Vector3(newSize, transform.localScale.y, transform.localScale.z);
        currentHealthSize = newSize;
    }

#endregion
}
