using System.Collections;
using UnityEngine;

/// <summary>
/// Simple wrapper class to hold the animation duration result from a coroutine.
/// </summary>
public class AnimationResult
{
    public float Duration { get; set; }
}

[RequireComponent(typeof(Animator))]
public class PlayerAnimationHelpers : MonoBehaviour
{
    private Animator animator;

    private float lastAnimationDuration = 0f;

#region Lifecycle calls

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

#endregion
#region Public methods

    ///<summary>
    /// Returns the duration of the last played animation.
    /// Always call this from a coroutine after waiting one frame to ensure the value is updated.
    ///</summary>
    public float GetLastAnimationDuration()
    {
        return lastAnimationDuration;
    }

#endregion
#region Coroutines

    public IEnumerator SetAnimationDuration()
    {
        // Wait one frame to ensure animator state is updated
        yield return null;

        // Update the public field with the duration
        var duration = animator.GetCurrentAnimatorStateInfo(0).length;
        lastAnimationDuration = duration;
        Debug.Log($"Set lastAnimationDuration to {duration} seconds.");
    }

#endregion

}
