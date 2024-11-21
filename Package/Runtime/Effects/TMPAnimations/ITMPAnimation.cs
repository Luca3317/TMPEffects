using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Databases;
using TMPEffects.TMPCommands;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Base interface for all TMPEffects animations.
    /// TODO Set / ValidateParameters should be in their own interface
    /// (which is also reused then by ITMPCommand)
    /// </summary>
    public interface ITMPAnimation : ITMPParameterValidator
    {
        /// <summary>
        /// Animate the given character.
        /// </summary>
        /// <param name="cData">Data about the character.</param>
        /// <param name="context">Data about the animator.</param>
        public void Animate(CharData cData, IAnimationContext context);

        /// <summary>
        /// Set the parameters for the animation.
        /// </summary>
        /// <param name="customData">The custom data for this animation.</param>
        /// <param name="parameters">Parameters as key-value-pairs.</param>
        /// <param name="context">The context object for this animation.</param>
        public void SetParameters(object customData, IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase);

        /// <summary>
        /// Create and get a new custom data object for this animation.
        /// </summary>
        /// <returns>The custom data object for this animation.</returns>
        public object GetNewCustomData(); // TODO Should this still get the context? Prolly not id say
    }
}