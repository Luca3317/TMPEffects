using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using TMPEffects.CharacterData;
using TMPEffects.ObjectChanged;
using System.ComponentModel;

namespace TMPEffects.TMPSceneAnimations
{
    /// <summary>
    /// Base class for all SceneAnimations.
    /// </summary>
    public abstract class TMPSceneAnimationBase : MonoBehaviour, ITMPAnimation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void Animate(CharData cData, IAnimationContext context);
        public abstract object GetNewCustomData();

        public abstract void SetParameters(object customData, IDictionary<string, string> parameters);
        public abstract bool ValidateParameters(IDictionary<string, string> parameters);

        protected virtual void OnValidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }
}
