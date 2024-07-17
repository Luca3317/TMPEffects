using TMPEffects.CharacterData;

namespace TMPEffects.Components.Animator
{
    /// <summary>
    /// To be used with <see cref="TMPAnimator"/>.<br/>
    /// Contains context data of the respective <see cref="TMPAnimator"/>.
    /// </summary>
    public interface IAnimatorContext
    {
        /// <summary>
        /// Whether animations are scaled.
        /// </summary>
        public bool ScaleAnimations { get; }
        /// <summary>
        /// Whether to scale animations uniformly (based on the default font size of the animator)
        /// or on a per character basis.<br/>
        /// Ignored if <see cref="ScaleAnimations"/> is false.
        /// </summary>
        public bool ScaleUniformly { get; }

        /// <summary>
        /// The animating <see cref="TMPAnimator"/>.
        /// </summary>
        public TMPAnimator Animator { get; }

        /// <summary>
        /// Whether animations use scaled time.
        /// </summary>
        public bool UseScaledTime { get; }

        /// <summary>
        /// The current delta time (=> time since last animation update).
        /// </summary>
        public float DeltaTime { get; }
        /// <summary>
        /// The time that has passed since the animator began animating.
        /// </summary>
        public float PassedTime { get; }

        /// <summary>
        /// Check how long the passed <see cref="CharData"/> has been in its current <see cref="VisibilityState"/>.<br/>
        /// Generally, to be used with show and hide animations.
        /// </summary>
        /// <param name="cData">The character to check.</param>
        /// <returns>How long the passed <see cref="CharData"/> has been in its current <see cref="VisibilityState"/>.</returns>
        public float StateTime(CharData cData);
        /// <summary>
        /// Check how long the passed <see cref="CharData"/> has been visible.
        /// </summary>
        /// <param name="cData">The character to check.</param>
        /// <returns>How long the passed <see cref="CharData"/> has been visible.</returns>
        public float VisibleTime(CharData cData);

        /// <summary>
        /// Check how long the <see cref="CharData"/> at the given index has been in its current <see cref="VisibilityState"/>.<br/>
        /// Generally, to be used with show and hide animations.
        /// </summary>
        /// <param name="index">The index of the character to check.</param>
        /// <returns>How long the <see cref="CharData"/> at the given index has been in its current <see cref="VisibilityState"/>.</returns>
        public float StateTime(int index);
        /// <summary>
        /// Check how long the <see cref="CharData"/> at the given index has been visible.
        /// </summary>
        /// <param name="cData">The character to check.</param>
        /// <returns>How long the <see cref="CharData"/> at the given index has been visible.</returns>
        public float VisibleTime(int index);
    }
}

