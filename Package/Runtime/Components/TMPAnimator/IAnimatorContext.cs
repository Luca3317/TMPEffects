using TMPEffects.CharacterData;
using TMPEffects.Databases;
using TMPEffects.Modifiers;

namespace TMPEffects.Components.Animator
{
    /// <summary>
    /// To be used with <see cref="TMPAnimator"/>.<br/>
    /// Contains context data of the respective <see cref="TMPAnimator"/>.
    /// </summary>
    public interface IAnimatorContext : IAnimatorDataProvider, ICharacterTimingsProvider, IAnimatorTimingsProvider
    {
    }

    /// <summary>
    /// Provides data about a <see cref="TMPAnimator"/>.
    /// </summary>
    public interface IAnimatorDataProvider
    {
        /// <summary>
        /// The animating <see cref="TMPAnimator"/>.
        /// </summary>
        public TMPAnimator Animator { get; }
        
        /// <summary>
        /// The current state of the CharData, with the previous animations applied.<br/>
        /// <p>
        /// <br/>
        /// A typical exemplary use case would be if you want an animation to enforce a character to be at a specific position,
        /// regardless of previously applied animations, you can reset the position of these Modifiers to undo previous changes.<br/>
        /// Another example would be if your animation requires the actual position of the character, that takes into account transformations
        /// made by previously applied animations, you can do so using <see href="Modifiers.CalculateVertexPositions"/>.
        /// </p>
        /// </summary>
        public CharDataModifiers Modifiers { get; }

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
        /// Whether animations use scaled time.
        /// </summary>
        public bool UseScaledTime { get; }
    }

    /// <summary>
    /// Provides timings of <see cref="TMPAnimator"/>.
    /// </summary>
    public interface IAnimatorTimingsProvider
    {
        /// <summary>
        /// The current delta time (=> time since last animation update).
        /// </summary>
        public float DeltaTime { get; }
        /// <summary>
        /// The time that has passed since the animator began animating.
        /// </summary>
        public float PassedTime { get; }
    }

    /// <summary>
    /// Provides visibility timings of <see cref="CharData"/>.
    /// </summary>
    public interface ICharacterTimingsProvider
    {
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

