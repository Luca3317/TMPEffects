using TMPEffects.TextProcessing;
using TMPEffects.Tags;
using TMPEffects.TMPAnimations;
using System.Diagnostics;

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
        public readonly ITMPAnimation animation;
        public readonly AnimationContext context;
        public readonly ReadOnlyAnimationContext roContext;

        public CachedAnimation(EffectTag tag, EffectTagIndices indices, ITMPAnimation animation, AnimationContext context)
        {
            this.tag = tag;
            this.indices = indices;
            overrides = null;
            if (tag.Parameters != null)
            {
                bool tmp;
                foreach (var param in tag.Parameters.Keys)
                {
                    switch (param)
                    {
                        case "override":
                        case "or":
                            if (ParsingUtility.StringToBool(tag.Parameters[param], out tmp)) overrides = tmp;
                            break;
                    }
                }
            }

            this.animation = animation;
            this.context = context;
            this.roContext = new ReadOnlyAnimationContext(context);
        }
    }
}
 