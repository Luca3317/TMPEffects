using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPEffects.Components.Animator
{
    /// <summary>
    /// To be used with <see cref="TMPAnimator"/>.<br/>
    /// Contains context data of the respective <see cref="TMPAnimator"/>.
    /// </summary>
    [System.Serializable]
    public class AnimatorContext
    {
        /// <summary>
        /// Whether to scale the animations.
        /// </summary>
        public bool scaleAnimations = true;

        /// <summary>
        /// Whether to use scaled time.
        /// </summary>
        public bool useScaledTime = true;

        /// <summary>
        /// The deltaTime since the last animation update.
        /// </summary>
        [System.NonSerialized, HideInInspector] public float deltaTime = 0;

        /// <summary>
        /// How much time has passed since the animator started playing its animations.
        /// </summary>
        [HideInInspector] public float passedTime { get => passed; set { passed = value; } }
        private float passed = 0;

        public AnimatorContext(bool scaleAnimations, bool useScaledTime)
        {
            this.scaleAnimations = scaleAnimations;
            this.useScaledTime = useScaledTime;
            this.deltaTime = 0f;
            this.passed = 0f;
        }
    }

    [System.Serializable]
    public class ReadOnlyAnimatorContext
    {
        public bool ScaleAnimations => context.scaleAnimations;
        public bool UseScaledTime => context.useScaledTime;
        public float DeltaTime => context.deltaTime;
        public float PassedTime => context.passedTime;

        public ReadOnlyAnimatorContext(AnimatorContext context)
        {
            if (context == null) throw new System.ArgumentNullException(nameof(context));
            this.context = context;
        }

        private AnimatorContext context;
    }
}

