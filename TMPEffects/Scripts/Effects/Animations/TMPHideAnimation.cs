using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.Animations
{
    public abstract class TMPHideAnimation : ScriptableObject, ITMPAnimation
    {
        /// <summary>
        /// Animate the given character.
        /// </summary>
        /// <param name="charData">Data about the character.</param>
        /// <param name="context">Data about the animator.</param>
        public abstract void Animate(ref CharData charData, IAnimationContext context);
        /// <summary>
        /// Set the parameters for the animation.
        /// </summary>
        /// <param name="parameters">Parameters as key-value-pairs.</param>
        public abstract void SetParameters(IDictionary<string, string> parameters);
        /// <summary>
        /// Validate the parameters.<br/>
        /// Used to validate tags.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract bool ValidateParameters(IDictionary<string, string> parameters);
        /// <summary>
        /// Reset the parameters.
        /// </summary>
        public abstract void ResetParameters();

        public virtual IAnimationContext GetNewContext()
        {
            return new DefaultAnimationContext();
        }

        private struct DefaultAnimationContext : IAnimationContext
        {
            public AnimatorContext animatorContext { get => settings; set => settings = value; }
            private AnimatorContext settings;

            public SegmentData segmentData { get; set; }
        }
    }
}