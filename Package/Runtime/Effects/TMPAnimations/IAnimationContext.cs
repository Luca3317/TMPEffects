using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using System.Diagnostics;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Basic interface for animation contexts.
    /// </summary>
    public interface IAnimationContext : IAnimationData, IAnimationFinished, IAnimationFinisher
    {

    }

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
