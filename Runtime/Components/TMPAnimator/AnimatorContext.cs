using TMPEffects.CharacterData;
using UnityEngine;
using System;

namespace TMPEffects.Components.Animator
{
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
}

