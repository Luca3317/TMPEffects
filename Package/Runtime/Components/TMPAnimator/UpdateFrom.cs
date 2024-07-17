namespace TMPEffects.Components.Animator
{
    /// <summary>
    /// To be used with <see cref="TMPAnimator"/>'s <see cref="TMPAnimator.SetUpdateFrom(UpdateFrom)"/> method.<br/>
    /// Defines where the <see cref="TMPAnimator"/> is updated from.
    /// </summary>
    [System.Serializable]
    public enum UpdateFrom : int
    {
        /// <summary>
        /// <see cref="TMPAnimator"/> is updated in the Update method.
        /// </summary>
        Update = 0,
        /// <summary>
        /// <see cref="TMPAnimator"/> is updated in the LateUpdate method.
        /// </summary>
        LateUpdate = 5,
        /// <summary>
        /// <see cref="TMPAnimator"/> is updated in the FixedUpdate method.
        /// </summary>
        FixedUpdate = 10,
        /// <summary>
        /// <see cref="TMPAnimator"/> is not automatically updated; you will need to update it from your own script.
        /// </summary>
        Script = 15
    }
}
