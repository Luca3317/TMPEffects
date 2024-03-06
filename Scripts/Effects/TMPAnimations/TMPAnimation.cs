using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.CharacterData;
using TMPEffects.ObjectChanged;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Base class for animations.
    /// </summary>
    public abstract class TMPAnimation : ScriptableObject, ITMPAnimation, INotifyObjectChanged
    {
        ///<inheritdoc/>
        public abstract void Animate(CharData cData, IAnimationContext context);
        ///<inheritdoc/>
        public abstract bool ValidateParameters(IDictionary<string, string> parameters);
        ///<inheritdoc/>
        public abstract object GetNewCustomData();

        ///<inheritdoc/>
        public abstract void SetParameters(object customData, IDictionary<string, string> parameters);


#if UNITY_EDITOR
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
#endif
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