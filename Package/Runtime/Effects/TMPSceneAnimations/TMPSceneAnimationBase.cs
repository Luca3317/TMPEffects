using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using TMPEffects.CharacterData;
using TMPEffects.ObjectChanged;
using System.ComponentModel;
using TMPEffects.Databases;

namespace TMPEffects.TMPAnimations.Animations
{
    /// <summary>
    /// Base class for all SceneAnimations.
    /// </summary>
    public abstract class TMPSceneAnimationBase : MonoBehaviour, ITMPAnimation, INotifyObjectChanged
    {
        public abstract void Animate(CharData cData, IAnimationContext context);
        public abstract object GetNewCustomData();

        public abstract void SetParameters(object customData, IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywordDatabase);

        public abstract bool ValidateParameters(IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywordDatabase);

        public event ObjectChangedEventHandler ObjectChanged;

        protected virtual void OnValidate()
        {
            RaiseObjectChanged();
        }

        protected virtual void OnDestroy()
        {
            RaiseObjectChanged();
        }

        protected void RaiseObjectChanged()
        {
            ObjectChanged?.Invoke(this);
        }
    }
}