using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Base class for all hide animations.
    /// </summary>
    public abstract class TMPHideAnimation : ScriptableObject, ITMPAnimation
    {
        ///<inheritdoc/>
        public abstract void Animate(ref CharData charData, IAnimationContext context);
        ///<inheritdoc/>
        public abstract void SetParameters(IDictionary<string, string> parameters);
        ///<inheritdoc/>
        public abstract bool ValidateParameters(IDictionary<string, string> parameters);
        ///<inheritdoc/>
        public abstract void ResetParameters();
        ///<inheritdoc/>
        public virtual IAnimationContext GetNewContext()
        {
            return new DefaultAnimationContext();
        }

        private struct DefaultAnimationContext : IAnimationContext
        {
            public ReadOnlyAnimatorContext animatorContext { get => settings; set => settings = value; }
            private ReadOnlyAnimatorContext settings;

            public SegmentData segmentData { get; set; }
        }
    }
}