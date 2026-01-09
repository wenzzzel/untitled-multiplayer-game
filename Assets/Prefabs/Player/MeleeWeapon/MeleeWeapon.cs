using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private float swingDuration = 0.3f; // Duration of the swing in seconds
    private bool isSwinging = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Swing()
    {
        if (!isSwinging)
        {
            StartCoroutine(SwingCoroutine());
        }
    }

    private IEnumerator SwingCoroutine()
    {
        isSwinging = true;
        float elapsed = 0f;
        float swingRadius = 0.2f;
        Vector3 originalPosition = Vector3.zero;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / swingDuration;
            float angle = progress * 2f * Mathf.PI; // Full circle (0 to 2π)

            // Calculate position on circle
            float x = Mathf.Cos(angle) * swingRadius;
            float y = Mathf.Sin(angle) * swingRadius;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            yield return null;
        }

        // Ensure we end at the original position
        transform.localPosition = originalPosition;
        isSwinging = false;
    }
}
