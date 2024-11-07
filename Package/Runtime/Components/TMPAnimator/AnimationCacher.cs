using System.Collections.Generic;
using System;
using TMPEffects.CharacterData;
using TMPEffects.Databases;
using TMPEffects.Tags;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Components.Animator
{
    // Handles logic for caching animations
    internal class AnimationCacher : ITagCacher<CachedAnimation>
    {
        private readonly ITMPEffectDatabase<ITMPAnimation> database;
        private readonly IList<CharData> charData;
        private readonly AnimatorContext context;
        private readonly Predicate<char> animates;
        private readonly CharDataModifiers modifiers;
        private readonly ReadOnlyAnimatorContext roContext;

        public AnimationCacher(ITMPEffectDatabase<ITMPAnimation> database, CharDataModifiers modifiers, AnimatorContext context, IList<CharData> charData, Predicate<char> animates)
        {
            this.context = context;
            this.database = database;
            this.charData = charData;
            this.animates = animates;
            this.modifiers = modifiers;
            roContext = new ReadOnlyAnimatorContext(context);
        }

        public CachedAnimation CacheTag(TMPEffectTag tag, TMPEffectTagIndices indices)
        {
            ITMPAnimation animation = database.GetEffect(tag.Name);
            TMPEffectTagIndices closedIndices = new TMPEffectTagIndices(indices.StartIndex, indices.IsOpen ? charData.Count : indices.EndIndex, indices.OrderAtIndex);

            SegmentData segmentData = new SegmentData(closedIndices, charData, animates);
            AnimationContext animationContext = new AnimationContext(roContext, modifiers, segmentData, null);
            
            // TODO this passes a writable context (but wrapped as readonly interface)
            // Prolly shouldnt do that
            object customAnimationData = animation.GetNewCustomData(animationContext);
            animationContext.CustomData = customAnimationData;
            animation.SetParameters(customAnimationData, tag.Parameters, animationContext);
            
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

