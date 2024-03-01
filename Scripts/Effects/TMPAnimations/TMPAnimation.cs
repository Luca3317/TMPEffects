using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Base class for animations.
    /// </summary>
    public abstract class TMPAnimation : ScriptableObject, ITMPAnimation
    {
        ///<inheritdoc/>
        public abstract void Animate(CharData charData, IAnimationContext context);
        ///<inheritdoc/>
        public abstract bool ValidateParameters(IDictionary<string, string> parameters);
        ///<inheritdoc/>
        public abstract object GetNewCustomData();

        ///<inheritdoc/>
        public abstract void SetParameters(object customData, IDictionary<string, string> parameters);
    }

    /// <summary>
    /// Base class for animations that dont take parameters.
    /// </summary>
    public abstract class TMPAnimationParameterless : TMPAnimation
    {
        ///<inheritdoc/>
        public override void SetParameters(object customData, IDictionary<string, string> parameters) { }
        ///<inheritdoc/>
        public override bool ValidateParameters(IDictionary<string, string> parameters) => true;
    }
}