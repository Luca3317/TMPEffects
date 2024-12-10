using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using System.Diagnostics;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Interface for animation contexts.
    /// </summary>
    public interface IAnimationContext : IAnimationData, IAnimationFinished, IAnimationFinisher
    {
    }

    /// <summary>
    /// Provides general data about the animation.
    /// </summary>
    public interface IAnimationData : IAnimationFinished
    {
        /// <summary>
        /// The context of the animating TMPAnimator.
        /// </summary>
        public IAnimatorContext AnimatorContext { get; }
        
        /// <summary>
        /// Data about the animation segment.
        /// </summary>
        public SegmentData SegmentData { get; }

        /// <summary>
        /// The custom data object.
        /// </summary>
        public object CustomData { get; }
    }

    /// <summary>
    /// Provides checks for whether an animation is done animating a specific <see cref="CharData"/>.
    /// </summary>
    public interface IAnimationFinished
    {
        /// <summary>
        /// Check if the animation is considered finished for the character at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Finished(int index);
        /// <summary>
        /// Check if the animation is considered finished for the given character.
        /// </summary>
        /// <param name="cData"></param>
        /// <returns></returns>
        public bool Finished(CharData cData);
    }

    /// <summary>
    /// Provides the ability to mark an animation as done animating a specific <see cref="CharData"/>.
    /// </summary>
    public interface IAnimationFinisher
    {
        /// <summary>
        /// Set the animation to be considered finished for the given character.
        /// </summary>
        /// <param name="cData"></param>
        /// <returns></returns>
        public void FinishAnimation(CharData cData);
    }
}
