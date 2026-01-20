using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthBarFill : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private float fullHealthSize = 1;
    [SerializeField] private float stepHealthSize = 0.01f;


    private SpriteRenderer spriteRenderer;
    
#region Lifecycle calls

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(fullHealthSize, transform.localScale.y, transform.localScale.z);
    }

#endregion
#region Public methods

    public void UpdateFill(int currentHealth)
    {
        var newSize = Mathf.Max(0f, currentHealth * stepHealthSize);
        
        transform.localScale = new Vector3(newSize, transform.localScale.y, transform.localScale.z);
    }

#endregion
}
