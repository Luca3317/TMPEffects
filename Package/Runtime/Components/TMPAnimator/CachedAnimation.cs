using TMPEffects.Databases;
using TMPEffects.Tags;
using TMPEffects.TMPAnimations;
using static TMPEffects.Parameters.TMPParameterUtility;

namespace TMPEffects.Components.Animator
{
    internal class CachedAnimation : ITagWrapper
    {
        public TMPEffectTag Tag => tag;
        public TMPEffectTagIndices Indices => indices;

        public bool Finished(int index) => context.Finished(index);

        public readonly ExtendedAnimationTagData TagData;
        
        private TMPEffectTag tag;
        private TMPEffectTagIndices indices;

        public bool? overrides => TagData.overrides;
        public bool late => TagData.late;
        public readonly ITMPAnimation animation;
        public readonly AnimationContext context;
        public readonly ReadOnlyAnimationContext roContext;
        public readonly int firstAffectingAnimationIndex = -1;

        public CachedAnimation(TMPEffectTag tag, TMPEffectTagIndices indices, ITMPAnimation animation, AnimationContext context, ExtendedAnimationTagData tagData)
        {
            this.tag = tag;
            this.indices = indices;
            this.animation = animation;
            this.context = context;
            this.roContext = new ReadOnlyAnimationContext(context);
            this.TagData = tagData;
        }
    }

    internal class ExtendedAnimationTagData
    {
        public readonly bool late;
        public readonly bool? overrides;

        public ExtendedAnimationTagData(TMPEffectTag tag, ITMPKeywordDatabase keywordDatabase)
        {
            late = tag.Parameters.ContainsKey("late");
            if (TryGetBoolParameter(out bool b, tag.Parameters, keywordDatabase, "override", "or"))
            {
                overrides = b;
            }
        }
    }
}
