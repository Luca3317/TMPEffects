using System.Collections.Generic;
using System;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.Databases;
using TMPEffects.Tags;
using TMPEffects.TMPAnimations;
using System.Diagnostics;

namespace TMPEffects.Components.Animator
{
    // Handles logic for caching animations
    internal class AnimationCacher : ITagCacher<CachedAnimation>
    {
        private readonly ITMPEffectDatabase<ITMPAnimation> database;
        private readonly IList<CharData> charData;
        private readonly AnimatorContext context;
        private readonly Predicate<char> animates;

        public AnimationCacher(ITMPEffectDatabase<ITMPAnimation> database, AnimatorContext context, IList<CharData> charData, Predicate<char> animates)
        {
            this.context = context;
            this.database = database;
            this.charData = charData;
            this.animates = animates;
        }

        public CachedAnimation CacheTag(EffectTag tag, EffectTagIndices indices)
        {
            ITMPAnimation animation = database.GetEffect(tag.Name);
            EffectTagIndices closedIndices = new EffectTagIndices(indices.StartIndex, indices.IsOpen ? charData.Count : indices.EndIndex, indices.OrderAtIndex);
            AnimationContext animationContext = new AnimationContext(new ReadOnlyAnimatorContext(context), new SegmentData(closedIndices, charData, animates), animation.GetNewCustomData());
            CachedAnimation ca = new CachedAnimation(
                tag, 
                closedIndices,
                animation, 
                animationContext
            );
            return ca;
        }
    }
}

