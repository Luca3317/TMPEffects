using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPEffects
{
    /*TODO
     * Reparsing the parameters every call is wasteful.
     * Instead, have some CachedParameter class; returned by validateparameters (through out)
     * Then pass that into set parameters (with in)
     */

    /// <summary>
    /// Base class for animations.
    /// </summary>
    public abstract class TMPAnimation : ScriptableObject, ITMPAnimation
    {
        /// <summary>
        /// Animate the given character.
        /// </summary>
        /// <param name="charData">Data about the character.</param>
        /// <param name="context">Data about the animator.</param>
        public abstract void Animate(ref CharData charData, ref IAnimationContext context);
        /// <summary>
        /// Set the parameters for the animation.
        /// </summary>
        /// <param name="parameters">Parameters as key-value-pairs.</param>
        public abstract void SetParameters(Dictionary<string, string> parameters);
        /// <summary>
        /// Validate the parameters.<br/>
        /// Used to validate tags.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract bool ValidateParameters(Dictionary<string, string> parameters);
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

    /// <summary>
    /// Base class for animations that dont take parameters.
    /// </summary>
    public abstract class TMPAnimationParameterless : TMPAnimation
    {
        public override void SetParameters(Dictionary<string, string> parameters) { }
        public override bool ValidateParameters(Dictionary<string, string> parameters) => true;
    }
}