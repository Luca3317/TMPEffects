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
        public abstract void SetParameters(IDictionary<string, string> parameters);
        ///<inheritdoc/>
        public abstract bool ValidateParameters(IDictionary<string, string> parameters);
        ///<inheritdoc/>
        public abstract void ResetParameters();
        ///<inheritdoc/>
        public virtual object GetNewCustomData()
        {
            return null;
        }
    }

    /// <summary>
    /// Base class for animations that dont take parameters.
    /// </summary>
    public abstract class TMPAnimationParameterless : TMPAnimation
    {
        public override void SetParameters(IDictionary<string, string> parameters) { }
        public override bool ValidateParameters(IDictionary<string, string> parameters) => true;
    }
}