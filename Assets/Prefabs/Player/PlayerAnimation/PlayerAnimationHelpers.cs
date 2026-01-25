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
    /// Use after calling SetAnimationDuration coroutine.
    ///</summary>
    public float GetLastAnimationDuration()
    {
        return lastAnimationDuration;
    }

#endregion
#region Public Coroutines

    public IEnumerator SetAnimationDuration()
    {
        // Wait one frame to ensure animator state is updated
        yield return null;

        lastAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
    }

#endregion

}
