using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.Components
{
    /// <summary>
    /// Base class for a all SceneAnimations.
    /// </summary>
    public abstract class TMPSceneAnimationBase : MonoBehaviour, ITMPAnimation
    {
        public abstract void Animate(CharData charData, IAnimationContext context);
        public abstract object GetNewCustomData();

        public abstract void ResetParameters();

        public abstract void SetParameters(IDictionary<string, string> parameters);
        public abstract bool ValidateParameters(IDictionary<string, string> parameters);
    }
}
