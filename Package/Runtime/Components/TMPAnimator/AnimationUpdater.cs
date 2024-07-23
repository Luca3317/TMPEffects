using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;

namespace TMPEffects.Components.Animator
{
    [Serializable]
    public class AnimationUpdater
    {
        public uint MaxUpdatesPerSecond => maxUpdatesPerSecond;

        uint maxUpdatesPerSecond;

        float delta;
        float updateTiming;
        float over;

        Action<float> updateAction;

        public AnimationUpdater(Action<float> updateAction, uint maxUpdatesPerSecond)
        {
            this.updateAction = updateAction;
            this.maxUpdatesPerSecond = maxUpdatesPerSecond;
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
                updateAction.Invoke(delta);
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
