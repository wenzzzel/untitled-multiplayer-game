using UnityEngine;

public class Tip : MonoBehaviour
{

    private float spriteSizeInPixels = 16f; //TODO: Fetch the actual sprite size instead of hardcoding.
    private bool shouldLerp = false;
    private Vector3 targetPosition = new();

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

}