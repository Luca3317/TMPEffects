using TMPEffects.CharacterData;

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

