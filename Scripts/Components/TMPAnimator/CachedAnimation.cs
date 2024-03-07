using TMPEffects.TextProcessing;
using TMPEffects.Tags;
using TMPEffects.TMPAnimations;
using static TMPEffects.ParameterUtility;
using UnityEngine;

namespace TMPEffects.Components.Animator
{
    internal class CachedAnimation : ITagWrapper
    {
        public EffectTag Tag => tag;
        public EffectTagIndices Indices => indices;

        public bool Finished(int index) => context.Finished(index);

        private EffectTag tag;
        private EffectTagIndices indices;

        public readonly bool? overrides;
        public readonly bool late;
        public readonly ITMPAnimation animation;
        public readonly AnimationContext context;
        public readonly ReadOnlyAnimationContext roContext;
        public readonly int firstAffectingAnimationIndex = -1;

        public CachedAnimation(EffectTag tag, EffectTagIndices indices, ITMPAnimation animation, AnimationContext context)
        {
            this.tag = tag;
            this.indices = indices;
            
            overrides = null;
            if (TryGetBoolParameter(out bool b, tag.Parameters, "override", "or"))
            {
                overrides = b;
            }

            late = tag.Parameters.ContainsKey("late");
            if (late) Debug.Log("Late true for " + tag.Name);

            this.animation = animation;
            this.context = context;
            this.roContext = new ReadOnlyAnimationContext(context);
        }
    }
}
