using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;

namespace TMPEffects.Components.Animator
{
    [Serializable]
    internal class AnimationUpdater
    {
        public uint MaxUpdatesPerSecond
        {
            get => maxUpdatesPerSecond;
        }

        public float AdditionalTimeScaling
        {
            get => additionalTimeScaling;
            set
            {
                additionalTimeScaling = value;
            }
        }
        
        [SerializeField]
        private uint maxUpdatesPerSecond = 60;

        [SerializeField] private float additionalTimeScaling = 1;
        
        private float delta;
        private float updateTiming;
        private float over;

        private Action<float> updateAction;

        public AnimationUpdater(Action<float> updateAction, uint maxUpdatesPerSecond, float timeScale)
        {
            this.updateAction = updateAction;
            this.maxUpdatesPerSecond = maxUpdatesPerSecond;
            this.additionalTimeScaling = timeScale;
            updateTiming = 1f / maxUpdatesPerSecond;
            delta = 0;
            over = 0;
        }

        public void SetMaxUpdatesPerSecond(uint maxUpdatesPerSecond)
        {
            this.maxUpdatesPerSecond = maxUpdatesPerSecond;
            updateTiming = 1f / maxUpdatesPerSecond;
            delta = 0;
            over = 0;
        }

        public bool Update(float deltaTime)
        {
            delta += deltaTime;

            if (delta + over >= updateTiming)
            {
                over = (delta + over) % updateTiming;
                updateAction.Invoke(delta * additionalTimeScaling);
                delta = 0;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            delta = 0;
            over = 0;
        }
    }
}
