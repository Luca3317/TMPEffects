using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Databases;
using UnityEngine;

namespace TMPEffects.TMPAnimations.Animations
{
    /// <summary>
    /// A stack of <see cref="TMPAnimation"/>, allowing you to easily combine animations in one tag.
    /// </summary>
    [Serializable]
    internal class BasicAnimationStack : AnimationStack<TMPAnimation> { }

    /// <summary>
    /// A stack of <see cref="TMPShowAnimation"/>, allowing you to easily combine show animations in one tag.
    /// </summary>
    [Serializable]
    internal class ShowAnimationStack : AnimationStack<TMPShowAnimation>
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data data = context.CustomData as Data;
            PopulateContextCache(data, context);

            foreach (var anim in animations)
            {
                (data.ContextCache[anim.animation] as AnimContext).ResetFinished(cData);
            }

            foreach (var anim in animations)
            {
                if (anim.animation == null) continue;
                anim.animation.Animate(cData, data.ContextCache[anim.animation]);
            }

            bool allDone = true;

            foreach (var anim in animations)
            {
                if (!data.ContextCache[anim.animation].Finished(cData))
                {
                    allDone = false;
                    break;
                }
            }

            if (allDone)
            {
                context.FinishAnimation(cData);
            }
        }
    }

    /// <summary>
    /// A stack of <see cref="TMPHideAnimation"/>, allowing you to easily combine hide animations in one tag.
    /// </summary>
    [Serializable]
    internal class HideAnimationStack : AnimationStack<TMPHideAnimation>
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data data = context.CustomData as Data;
            PopulateContextCache(data, context);

            foreach (var anim in animations)
            {
                (data.ContextCache[anim.animation] as AnimContext).ResetFinished(cData);
            }

            foreach (var anim in animations)
            {
                if (anim.animation == null) continue;
                anim.animation.Animate(cData, data.ContextCache[anim.animation]);
            }

            bool allDone = true;

            foreach (var anim in animations)
            {
                if (!data.ContextCache[anim.animation].Finished(cData))
                {
                    allDone = false;
                    break;
                }
            }

            if (allDone)
            {
                context.FinishAnimation(cData);
            }
        }
    }

    /// <summary>
    /// A stack of <see cref="ITMPAnimation"/>, allowing you to easily combine animations in one tag.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    internal class AnimationStack<T> : ITMPAnimation where T : ITMPAnimation
    {
        [SerializeField] protected List<AnimPrefixTuple> animations = new List<AnimPrefixTuple>();
        internal List<AnimPrefixTuple> Animations => animations;

        public virtual void Animate(CharData cData, IAnimationContext context)
        {
            Data data = context.CustomData as Data;
            PopulateContextCache(data, context);

            foreach (var anim in animations)
            {
                if (anim.animation == null) continue;
                anim.animation.Animate(cData, data.ContextCache[anim.animation]);
            }
        }

        protected void PopulateContextCache(Data data, IAnimationContext context)
        {
            if (data.ContextCache.Count == 0)
            {
                foreach (var anim in animations)
                {
                    if (anim.animation == null) continue;

                    AnimContext animContext = new AnimContext(context, data.ObjectCache[anim.animation]);
                    // ReadOnlyAnimationContext animContext = new ReadOnlyAnimationContext(context.AnimatorContext, context.State, context.SegmentData, data.ObjectCache[anim.animation]);
                    data.ContextCache[anim.animation] = animContext;
                }
            }
        }

        public object GetNewCustomData()
        {
            Data data = new Data();

            foreach (var anim in animations)
            {
                if (anim.animation == null) continue;
                data.ObjectCache[anim.animation] = anim.animation.GetNewCustomData();
            }

            return data;
        }

        public void SetParameters(object customData, IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase)
        {
            Data data = customData as Data;

            foreach (var anim in animations)
            {
                if (anim.animation == null) continue;

                var animParams = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(parameters
                    .Where(x => x.Key.StartsWith(anim.prefix))
                    .Select(x => new KeyValuePair<string, string>(x.Key.Substring(anim.prefix.Length), x.Value))));

                anim.animation.SetParameters(data.ObjectCache[anim.animation], animParams, keywordDatabase);
            }
        }

        public bool ValidateParameters(IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase)
        {
            foreach (var anim in animations)
            {
                if (anim.animation == null) continue;

                var animParams = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(parameters
                    .Where(x => x.Key.StartsWith(anim.prefix))
                    .Select(x => new KeyValuePair<string, string>(x.Key.Substring(anim.prefix.Length), x.Value))));

                if (!anim.animation.ValidateParameters(animParams, keywordDatabase)) return false;
            }

            return true;
        }

        public class AnimContext : IAnimationContext
        {
            public IAnimatorContext AnimatorContext => context.AnimatorContext;

            public SegmentData SegmentData => context.SegmentData;

            public object CustomData => customData;

            public AnimContext(IAnimationContext context, object customData)
            {
                this.context = context;
                this.customData = customData;
            }

            public void FinishAnimation(CharData cData)
            {
                finishedDict[cData.info.index] = true;
            }

            public bool Finished(int index)
            {
                return finishedDict[index];
            }

            public bool Finished(CharData cData)
            {
                return finishedDict[cData.info.index];
            }

            public void ResetFinished(CharData cData)
            {
                finishedDict[cData.info.index] = false;
            }

            private IAnimationContext context;
            private object customData;

            private Dictionary<int, bool> finishedDict = new Dictionary<int, bool>();
        }

        [Serializable]
        public struct AnimPrefixTuple
        {
            public T animation;
            public string prefix;

            public AnimPrefixTuple(T animation, string prefix)
            {
                this.animation = animation;
                this.prefix = prefix;
            }
        }

        public class Data
        {
            public Dictionary<ITMPAnimation, object> ObjectCache = new Dictionary<ITMPAnimation, object>();
            public Dictionary<ITMPAnimation, IAnimationContext> ContextCache = new Dictionary<ITMPAnimation, IAnimationContext>();
        }
    }
}
