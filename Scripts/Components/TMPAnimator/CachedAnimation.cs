using TMPEffects.TextProcessing;
using TMPEffects.Tags;
using TMPEffects.TMPAnimations;

namespace TMPEffects.Components.Animator
{
    internal class CachedAnimation : ITagWrapper
    {
        public EffectTag Tag => tag;
        public EffectTagIndices Indices => indices;

        private EffectTag tag;
        private EffectTagIndices indices;

        public readonly bool? overrides;
        public readonly ITMPAnimation animation;
        public readonly IAnimationContext context;

        public CachedAnimation(EffectTag tag, EffectTagIndices indices, ITMPAnimation animation)
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
            this.context = animation.GetNewContext();
        }
    }
}
