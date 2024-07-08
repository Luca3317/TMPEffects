using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;

namespace TMPEffects.TMPAnimations
{
    public class ReadOnlyAnimationContext : IAnimationContext
    {
        /// <inheritdoc/>
        public bool Finished(int index) => context.Finished(index);
        /// <inheritdoc/>
        public bool Finished(CharData cData) => context.Finished(cData);
        /// <inheritdoc/>
        public IAnimatorContext AnimatorContext => context.AnimatorContext;
        /// <inheritdoc/>
        public SegmentData SegmentData => context.SegmentData;
        /// <inheritdoc/>
        public object CustomData => context.CustomData;
        /// <inheritdoc/>
        public ICharDataState State => context.State;

        public ReadOnlyAnimationContext(AnimationContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public void FinishAnimation(CharData cData) => context.FinishAnimation(cData);

        private AnimationContext context;
    }
}
