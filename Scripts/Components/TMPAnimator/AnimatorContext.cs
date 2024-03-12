using TMPEffects.Components.CharacterData;
using UnityEngine;
using System;

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
        [System.NonSerialized, HideInInspector] public float passed;

        [System.NonSerialized, HideInInspector] public Func<int, float> StateTime;
        [System.NonSerialized, HideInInspector] public Func<int, float> VisibleTime;
        
        public AnimatorContext() { }
        public AnimatorContext(bool scaleAnimations, bool useScaledTime, Func<int, float> getVisibleTime, Func<int, float> getStateTime)
        {
            this.scaleAnimations = scaleAnimations;
            this.useScaledTime = useScaledTime;
            this.deltaTime = 0f;
            this.passed = 0f;

            this.StateTime = getVisibleTime;
            this.VisibleTime = getStateTime;
        }
    }

    [System.Serializable]
    public class ReadOnlyAnimatorContext
    {
        public bool ScaleAnimations => context.scaleAnimations;
        public bool UseScaledTime => context.useScaledTime;
        public float DeltaTime => context.deltaTime;
        public float PassedTime => context.passed;

        public float StateTime(CharData cData) => context.StateTime(cData.info.index);
        public float VisibleTime(CharData cData) => context.VisibleTime(cData.info.index);

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

