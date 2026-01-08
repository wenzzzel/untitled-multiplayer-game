using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Tip : MonoBehaviour
{
    private float spriteSizeInPixels = 16f; //TODO: Fetch the actual sprite size instead of hardcoding.
    private bool shouldLerp = false;
    private Vector3 targetPosition = new();

#region Lifecycle calls

    void Awake()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    void Update()
    {
        if (shouldLerp)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                Time.deltaTime * 5
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CollisionIsNotWithAnotherPlayer(other))
            return;

        if (CollisionWithMyself(other.transform))
            return;
        
        Debug.Log("Other player hooked!"); //TODO: This is where hooking should be initialized.
    }

#endregion
#region Private methods

    private bool CollisionWithMyself(Transform collidingWith)
    {
        return collidingWith.IsChildOf(this.transform) || this.transform.IsChildOf(collidingWith);
    }

    private bool CollisionIsNotWithAnotherPlayer(Collider2D other)
    {
        return !other.CompareTag("Player");
    }

#endregion
#region Public methods

    public void MoveTip(Vector3 newScaleValue)
    {
        var positionChangeFactor = spriteSizeInPixels / 100f;

        var newPosition = newScaleValue.y * positionChangeFactor;

        targetPosition = new Vector3(
            transform.localPosition.x,
            newPosition,
            transform.localPosition.z
        );

        shouldLerp = true;
    }

#endregion

}