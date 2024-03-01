using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.ObjectChanged;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Base class for all hide animations.
    /// </summary>
    public abstract class TMPHideAnimation : ScriptableObject, ITMPAnimation, INotifyObjectChanged
    {
        [SerializeField] protected float duration;

        ///<inheritdoc/>
        public abstract void Animate(CharData charData, IAnimationContext context);
        ///<inheritdoc/>
        public abstract void SetParameters(object customData, IDictionary<string, string> parameters);
        ///<inheritdoc/>
        public abstract bool ValidateParameters(IDictionary<string, string> parameters);
        ///<inheritdoc/>
        public virtual object GetNewCustomData()
        {
            return null;
        }

#if UNITY_EDITOR
        public event ObjectChangedEventHandler ObjectChanged;

        protected virtual void OnValidate()
        {
            RaiseObjectChanged();
        }

        protected void RaiseObjectChanged()
        {
            ObjectChanged?.Invoke(this);
        }
#endif
    }
}