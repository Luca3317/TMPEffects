using TMPEffects.CharacterData;
using UnityEngine;
using System;

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

    /// <summary>
    /// To be used with <see cref="TMPAnimator"/>.<br/>
    /// Contains context data of the respective <see cref="TMPAnimator"/>.
    /// </summary>
    [System.Serializable]
    public class AnimatorContext : IAnimatorContext
    {
        /// <inheritdoc/>
        public bool ScaleAnimations
        {
            get => scaleAnimations;
            set => scaleAnimations = value;
        }
        /// <inheritdoc/>
        public bool ScaleUniformly
        {
            get => scaleUniformly;
            set => scaleUniformly = value;
        }
        /// <inheritdoc/>
        public bool UseScaledTime
        {
            get => useScaledTime;
            set => useScaledTime = value;
        }
        public TMPAnimator Animator
        {
            get => tmpAnimator;
            set => tmpAnimator = value;
        }
        /// <inheritdoc/>
        public float DeltaTime
        {
            get => deltaTime;
            set => deltaTime = value;
        }
        /// <inheritdoc/>
        public float PassedTime
        {
            get => passed;
            set => passed = value;
        }

        [Tooltip("Whether to scale the animations. If true, they will look the same no matter how large or small the individual characters")]
        [SerializeField] private bool scaleAnimations = true;
        [Tooltip("Whether to scale the animations uniformly based on the default font size of the TMP_Text component, or on a per character basis.\nIgnored if ScaleAnimations is false")]
        [SerializeField] private bool scaleUniformly = true;
        [Tooltip("Whether to use scaled time (instead of real time)")]
        [SerializeField] private bool useScaledTime = true;

        [SerializeField, HideInInspector] private TMPAnimator tmpAnimator;

        [System.NonSerialized, HideInInspector] public float deltaTime = 0;
        [System.NonSerialized, HideInInspector] public float passed;
        [System.NonSerialized, HideInInspector] public Func<int, float> _StateTime;
        [System.NonSerialized, HideInInspector] public Func<int, float> _VisibleTime;

        public AnimatorContext() { }
        public AnimatorContext(TMPAnimator animator) { this.tmpAnimator = animator; }
        public AnimatorContext(TMPAnimator animator, bool scaleAnimations, bool useScaledTime, bool scaleUniformly, Func<int, float> getVisibleTime, Func<int, float> getStateTime)
        {
            this.tmpAnimator = animator;
            this.ScaleAnimations = scaleAnimations;
            this.scaleUniformly = scaleUniformly;
            this.UseScaledTime = useScaledTime;
            this.deltaTime = 0f;
            this.passed = 0f;

            this._StateTime = getVisibleTime;
            this._VisibleTime = getStateTime;
        }

        /// <inheritdoc/>
        public float StateTime(CharData cData) => _StateTime(cData.info.index);
        /// <inheritdoc/>
        public float VisibleTime(CharData cData) => _VisibleTime(cData.info.index);
        /// <inheritdoc/>
        public float StateTime(int index) => _StateTime(index);
        /// <inheritdoc/>
        public float VisibleTime(int index) => _VisibleTime(index);
    }

    [System.Serializable]
    public class ReadOnlyAnimatorContext : IAnimatorContext
    {
        /// <inheritdoc/>
        public bool ScaleAnimations => context.ScaleAnimations;
        /// <inheritdoc/>
        public bool ScaleUniformly => context.ScaleUniformly;
        /// <inheritdoc/>
        public bool UseScaledTime => context.UseScaledTime;
        /// <inheritdoc/>
        public TMPAnimator Animator => context.Animator;
        /// <inheritdoc/>
        public float DeltaTime => context.deltaTime;
        /// <inheritdoc/>
        public float PassedTime => context.passed;

        /// <inheritdoc/>
        public float StateTime(CharData cData) => context.StateTime(cData);
        /// <inheritdoc/>
        public float VisibleTime(CharData cData) => context.VisibleTime(cData);

        /// <inheritdoc/>
        public float StateTime(int index) => context.StateTime(index);
        /// <inheritdoc/>
        public float VisibleTime(int index) => context.VisibleTime(index);

        public ReadOnlyAnimatorContext(AnimatorContext context)
        {
            if (context == null) throw new System.ArgumentNullException(nameof(context));
            this.context = context;
        }

        private AnimatorContext context;
    }
}

