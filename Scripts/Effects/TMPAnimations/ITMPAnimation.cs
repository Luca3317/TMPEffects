using System.Collections.Generic;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations
{
    public interface ITMPAnimation
    {
        /// <summary>
        /// Animate the given character.
        /// </summary>
        /// <param name="charData">Data about the character.</param>
        /// <param name="context">Data about the animator.</param>
        public void Animate(CharData charData, IAnimationContext context);

        /// <summary>
        /// Set the parameters for the animation.
        /// </summary>
        /// <param name="parameters">Parameters as key-value-pairs.</param>
        public void SetParameters(IDictionary<string, string> parameters);
        /// <summary>
        /// Validate the parameters.<br/>
        /// Used to validate tags.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>true if the parameters were successfully validated; false otherwise.</returns>
        public bool ValidateParameters(IDictionary<string, string> parameters);
        /// <summary>
        /// Reset the parameters.
        /// </summary>
        public void ResetParameters();

        /// <summary>
        /// Get the context for this animation.
        /// </summary>
        /// <returns>The context for this animation.</returns>
        public IAnimationContext GetNewContext();
    }
}