using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;

namespace TMPEffects.Components.Animator
{
    /// <summary>
    /// Simple component that lets you manually update a <see cref="TMPAnimator"/>.
    /// </summary>
    [RequireComponent(typeof(TMPAnimator))]
    public class TMPAnimatorUpdater : MonoBehaviour
    {
        /// <summary>
        /// The update limit per second.
        /// </summary>
        public uint MaxUpdatesPerSecond => maxUpdatesPerSecond;
        /// <summary>
        /// The time scaling to use.<br/>
        /// Used as scalar to the delta time.<br/>
        /// Naming chosen so as not to be confused with <see cref="IAnimatorContext.UseScaledTime"/>.
        /// </summary>
        public float AdditionalTimeScaling => additionalTimeScaling;
        
        [SerializeField] private uint maxUpdatesPerSecond = 144;
        [SerializeField] private float additionalTimeScaling = 1;
        
        [System.NonSerialized] AnimationUpdater animUpdater;    

        /// <summary>
        /// Set the update limit per second.
        /// </summary>
        /// <param name="maxUpdatesPerSecond"></param>
        public void SetMaxUpdatesPerSecond(uint maxUpdatesPerSecond) => animUpdater.SetMaxUpdatesPerSecond(maxUpdatesPerSecond);

        /// <summary>
        /// Set the scalar to the deltaTime.
        /// </summary>
        /// <param name="timeScaling"></param>
        public void SetAdditionalTimeScaling(float timeScaling) => animUpdater.AdditionalTimeScaling = timeScaling;

        private void OnEnable()
        {
            TMPAnimator anim = GetComponent<TMPAnimator>();
            anim.SetUpdateFrom(UpdateFrom.Script);
            animUpdater = new AnimationUpdater(anim.UpdateAnimations, maxUpdatesPerSecond, additionalTimeScaling);
        }

        void Update()
        {
            animUpdater.Update(Time.deltaTime);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (animUpdater != null)
            {
                animUpdater.Reset();
                animUpdater.SetMaxUpdatesPerSecond(maxUpdatesPerSecond);
                animUpdater.AdditionalTimeScaling = additionalTimeScaling;
            }
        }
#endif
    }
}