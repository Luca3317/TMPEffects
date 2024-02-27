using TMPEffects.Components.Animator;

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
        public AnimatorContext animatorContext { get; set; }
        /// <summary>
        /// Data about the animation segment.
        /// </summary>
        public SegmentData segmentData { get; set; }

        /// <summary>
        /// Reset the context.
        /// </summary>
        public void ResetContext() { }
    }
}
