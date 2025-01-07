using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Databases;
using TMPEffects.Parameters;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Base interface for all TMPEffects animations.
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
        /// <param name="keywordDatabase">The keyword database used for parsing the parameter values.</param>
        public void SetParameters(object customData, IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase);

        /// <summary>
        /// Get a new custom data object for this animation.
        /// </summary>
        /// <returns>The custom data object for this animation.</returns>
        public object GetNewCustomData();
    }
}