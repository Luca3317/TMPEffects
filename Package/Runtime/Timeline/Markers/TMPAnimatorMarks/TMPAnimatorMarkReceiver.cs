using TMPEffects.Components;
using UnityEngine;
using UnityEngine.Playables;

namespace TMPEffects.Timeline.Markers
{
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
                    float delta = uam.DeltaTime;
                    if (delta < 0)
                    {
                        if (delta == -1) delta = Time.deltaTime;
                        else if (delta == -2) delta = Time.fixedDeltaTime;
                        else delta = 0;
                    }
                    animator.UpdateAnimations(delta);
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
}