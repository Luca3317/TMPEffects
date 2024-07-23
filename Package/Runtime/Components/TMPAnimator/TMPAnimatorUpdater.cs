using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;

namespace TMPEffects.Components.Animator
{
    [RequireComponent(typeof(TMPAnimator))]
    public class TMPAnimatorUpdater : MonoBehaviour
    {
        public uint MaxUpdatesPerSecond => maxUpdatesPerSecond;
        [SerializeField] private uint maxUpdatesPerSecond;
        [System.NonSerialized] AnimationUpdater animUpdater;

        public void SetMaxUpdatesPerSecond(uint maxUpdatesPerSecond) => animUpdater.SetMaxUpdatesPerSecond(maxUpdatesPerSecond);

        private void OnEnable()
        {
            TMPAnimator anim = GetComponent<TMPAnimator>();
            anim.SetUpdateFrom(UpdateFrom.Script);
            animUpdater = new AnimationUpdater(anim.UpdateAnimations, maxUpdatesPerSecond);
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
            }
        }
#endif
    }
}