using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;

[Serializable]
public class AnimationUpdater
{
    public uint MaxUpdatesPerSecond => maxUpdatesPerSecond;

    uint maxUpdatesPerSecond;

    float delta;
    TMPAnimator animator;
    float updateTiming;
    float over;

    public AnimationUpdater(TMPAnimator animator, uint maxUpdatesPerSecond)
    {
        this.animator = animator;
        this.animator.SetUpdateFrom(TMPEffects.Components.Animator.UpdateFrom.Script);
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
            animator.UpdateAnimations(delta);
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
