using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.ObjectChanged;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Base class for all show animations.
    /// </summary>
    public abstract class TMPShowAnimation : ScriptableObject, ITMPAnimation, INotifyObjectChanged
    {
        ///<inheritdoc/>
        public abstract void Animate(CharData cData, IAnimationContext context);
        ///<inheritdoc/>
        public abstract void SetParameters(object customData, IDictionary<string, string> parameters);
        ///<inheritdoc/>
        public abstract bool ValidateParameters(IDictionary<string, string> parameters);
        ///<inheritdoc/>
        public virtual object GetNewCustomData()
        {
            return null;
        }

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
