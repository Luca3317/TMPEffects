using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Base interface for all TMPEffects animations.
    /// </summary>
    public interface ITMPAnimation
    {
        /// <summary>
        /// Animate the given character.
        /// </summary>
        /// <param name="cData">Data about the character.</param>
        /// <param name="context">Data about the animator.</param>
        public void Animate(CharData cData, IAnimationContext context);

        /// <summary>
        /// Validate the parameters.<br/>
        /// Used to validate tags.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="context"></param>
        /// <returns>true if the parameters were successfully validated; false otherwise.</returns>
        public bool ValidateParameters(IDictionary<string, string> parameters, IAnimatorContext context);

        /// <summary>
        /// Set the parameters for the animation.
        /// </summary>
        /// <param name="customData">The custom data for this animation.</param>
        /// <param name="parameters">Parameters as key-value-pairs.</param>
        /// <param name="context">The context object for this animation.</param>
        public void SetParameters(object customData, IDictionary<string, string> parameters, IAnimationContext context);

        /// <summary>
        /// Create and get a new custom data object for this animation.
        /// </summary>
        /// <param name="context">The context object for this animation.</param>
        /// <returns>The custom data object for this animation.</returns>
        public object GetNewCustomData(IAnimationContext context);
    }
} 