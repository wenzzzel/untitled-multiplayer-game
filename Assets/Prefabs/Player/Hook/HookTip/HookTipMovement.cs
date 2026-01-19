using UnityEngine;

public class HookTipMovement : MonoBehaviour
{

    private float spriteSizeInPixels = 16f; //TODO: Fetch the actual sprite size instead of hardcoding.
    private bool shouldLerp = false;
    private Vector3 targetPosition = new();
    private float tipMovementSpeed = 5f;

    void Update()
    {
        if (shouldLerp)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                Time.deltaTime * tipMovementSpeed
            );
        }
    }

    public void MoveTip(Vector3 newScaleValue, float hookBodyStretchSpeed)
    {
        this.tipMovementSpeed = hookBodyStretchSpeed;

        var positionChangeFactor = spriteSizeInPixels / 100f;

        var newPosition = newScaleValue.y * positionChangeFactor;

        // Keep X and Z at 0 so the tip stays centered on the hook's axis
        targetPosition = new Vector3(0f, newPosition, 0f);

        shouldLerp = true;
    }
}
