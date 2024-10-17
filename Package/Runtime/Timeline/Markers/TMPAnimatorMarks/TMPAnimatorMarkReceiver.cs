using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(TMPAnimator))]
public class TMPAnimatorMarkReceiver : MonoBehaviour, INotificationReceiver
{
    private TMPAnimator animator;

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (animator == null)
        {
            animator = GetComponent<TMPAnimator>();
            if (animator == null) return;
        }

        switch (notification)
        {
            case TMPStartAnimatingMarker:
                animator.StartAnimating();
                break;
            
            case TMPStopAnimatingMarker:
                animator.StopAnimating();
                break;

            case TMPUpdateAnimationsMarker uam:
                animator.UpdateAnimations(uam.DeltaTime);
                break;

            case TMPSetUpdateFromMarker sufm:
                animator.SetUpdateFrom(sufm.UpdateFrom);
                break;
            
            case TMPResetAnimationsMarker:
                animator.ResetAnimations();
                break;
            
            case TMPResetTimeMarker rtm:
                animator.ResetTime(rtm.Time);
                break;
        }
    }
}   