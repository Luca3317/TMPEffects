using UnityEngine;

namespace TMPEffects.Components.Animator
{
    /// <summary>
    /// To be used with <see cref="TMPAnimator"/>.<br/>
    /// Identifies a specific type of animation.
    /// </summary>
    public enum TMPAnimationType : short
    {
        /// <summary>
        /// The "standard" animation type.<br/>
        /// Animations of this type are updated continuously.
        /// </summary>
        Basic = 0,
        /// <summary>
        /// Animations of this type are played when a character is shown.
        /// </summary>
        Show = 5,
        /// <summary>
        /// Animations of this type are played when a character is hidden.
        /// </summary>
        Hide = 10
    }
}
