using System;
using TMPEffects.CharacterData;
using TMPEffects.Databases;
using TMPEffects.Modifiers;

namespace TMPEffects.Components.Animator
{
    /// <summary>
    /// To be used with <see cref="TMPAnimator"/>.<br/>
    /// Contains readonly context data of the respective <see cref="TMPAnimator"/>.
    /// </summary>
    [System.Serializable]
    public class ReadOnlyAnimatorContext : IAnimatorContext
    {
        /// <inheritdoc/>
        public CharDataModifiers Modifiers => context.Modifiers;

        /// <inheritdoc/>
        public bool ScaleAnimations => context.ScaleAnimations;
        /// <inheritdoc/>
        public bool ScaleUniformly => context.ScaleUniformly;
        /// <inheritdoc/>
        public bool UseScaledTime => context.UseScaledTime;
        /// <inheritdoc/>
        public TMPAnimator Animator => context.Animator;
        /// <inheritdoc/>
        public float DeltaTime => context.DeltaTime;
        /// <inheritdoc/>
        public float PassedTime => context.PassedTime;

        /// <inheritdoc/>
        public float StateTime(CharData cData) => context.StateTime(cData);
        /// <inheritdoc/>
        public float VisibleTime(CharData cData) => context.VisibleTime(cData);

        /// <inheritdoc/>
        public float StateTime(int index) => context.StateTime(index);
        /// <inheritdoc/>
        public float VisibleTime(int index) => context.VisibleTime(index);

        public ReadOnlyAnimatorContext(IAnimatorContext context)
        {
            if (context == null) throw new System.ArgumentNullException(nameof(context));
            this.context = context;
        }
        
        public ReadOnlyAnimatorContext(TMPAnimator animator, bool scaleAnimations, bool useScaledTime, bool scaleUniformly, Func<int, float> getVisibleTime, Func<int, float> getStateTime)
            : this (new AnimatorContext(animator, scaleAnimations, useScaledTime, scaleUniformly, getVisibleTime, getStateTime))
        {}

        private IAnimatorContext context;
    }
}

