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
        public bool ScaleAnimations { get; }
        public bool UseScaledTime { get; }

        public float DeltaTime { get; }
        public float Passed { get; }

        public float StateTime(CharData cData);
        public float VisibleTime(CharData cData);
    }

    /// <summary>
    /// To be used with <see cref="TMPAnimator"/>.<br/>
    /// Contains context data of the respective <see cref="TMPAnimator"/>.
    /// </summary>
    [System.Serializable]
    public class AnimatorContext /*: IAnimatorContext*/
    {
        public bool ScaleAnimations
        {
            get => scaleAnimations;
            set => scaleAnimations = value;
        }
        public bool UseScaledTime
        {
            get => useScaledTime;
            set => useScaledTime = value;
        }




        /// <summary>
        /// Whether to scale the animations.
        /// </summary>
        [SerializeField] private bool scaleAnimations = true;

        /// <summary>
        /// Whether to use scaled time.
        /// </summary>
        [SerializeField] private bool useScaledTime = true;

        /// <summary>
        /// The deltaTime since the last animation update.
        /// </summary>
        [System.NonSerialized, HideInInspector] public float deltaTime = 0;

        /// <summary>
        /// How much time has passed since the animator started playing its animations.
        /// </summary>
        [System.NonSerialized, HideInInspector] public float passed;

        [System.NonSerialized, HideInInspector] public Func<int, float> _StateTime;
        [System.NonSerialized, HideInInspector] public Func<int, float> _VisibleTime;

        public AnimatorContext() { }
        public AnimatorContext(bool scaleAnimations, bool useScaledTime, Func<int, float> getVisibleTime, Func<int, float> getStateTime)
        {
            this.ScaleAnimations = scaleAnimations;
            this.UseScaledTime = useScaledTime;
            this.deltaTime = 0f;
            this.passed = 0f;

            this._StateTime = getVisibleTime;
            this._VisibleTime = getStateTime;
        }

        public float StateTime(CharData cData) => _StateTime(cData.info.index);
        public float VisibleTime(CharData cData) => _VisibleTime(cData.info.index);
        public float StateTime(int index) => _StateTime(index);
        public float VisibleTime(int index) => _VisibleTime(index);
    }

    [System.Serializable]
    public class ReadOnlyAnimatorContext
    {
        /// <summary>
        /// Whether animation
        /// </summary>
        public bool ScaleAnimations => context.ScaleAnimations;
        public bool UseScaledTime => context.UseScaledTime;
        public float DeltaTime => context.deltaTime;
        public float PassedTime => context.passed;

        public float StateTime(CharData cData) => context.StateTime(cData);
        public float VisibleTime(CharData cData) => context.VisibleTime(cData);

        public float StateTime(int index) => context.StateTime(index);
        public float VisibleTime(int index) => context.VisibleTime(index);

        public ReadOnlyAnimatorContext(AnimatorContext context)
        {
            if (context == null) throw new System.ArgumentNullException(nameof(context));
            this.context = context;
        }

        private AnimatorContext context;
    }
}

