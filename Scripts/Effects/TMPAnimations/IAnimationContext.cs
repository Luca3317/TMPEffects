using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Basic interface for animation contexts.
    /// </summary>
    public interface IAnimationContext
    {
        /// <summary>
        /// The context of the animator.
        /// </summary>
        public ReadOnlyAnimatorContext animatorContext { get; set; }
        /// <summary>
        /// Data about the animation segment.
        /// </summary>
        public SegmentData segmentData { get; set; }

        /// <summary>
        /// Reset the context.
        /// </summary>
        public void ResetContext() { }
    }

    /// <summary>
    /// Basic interface for animation contexts for <see cref="TMPShowAnimation"/> and <see cref="TMPHideAnimation"/>.
    /// </summary>
    public interface IVisibilityAnimationContext : IAnimationContext
    {
        /// <summary>
        /// Since when the character has been in its current <see cref="VisibilityState"/>.
        /// </summary>
        public float StateTime { get; }
        /// <summary>
        /// Since when the character has been visible, i.e. since when its <see cref="VisibilityState"/> 
        /// has not been <see cref="VisibilityState.Hidden"/>.
        /// </summary>
        public float VisibleTime { get; }
    }
}
