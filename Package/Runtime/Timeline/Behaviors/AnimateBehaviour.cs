// using System.Collections;
// using System.Collections.Generic;
// using TMPEffects.Components;
// using TMPEffects.Components.Animator;
// using UnityEngine;
// using UnityEngine.Playables;
//
// public class AnimateBehaviour : PlayableBehaviour
// {
//     public AnimateClip.Method methodToCallOnPlay;
//     public UpdateFrom updateFromOnPlay;
//
//     public AnimateClip.Method methodToCallOnPause;
//     public UpdateFrom updateFromOnPause;
//
//     private TMPAnimator animator;
//
//     public override void OnBehaviourPlay(Playable playable, FrameData info)
//     {
//         Debug.Log("Play");
//         animator = info.output.GetUserData() as TMPAnimator;
//
// #if UNITY_EDITOR
//         if (!Application.isPlaying) return;
// #endif
//
//         if (methodToCallOnPlay == AnimateClip.Method.StartAnimating)
//         {
//             if (animator.UpdateFrom == UpdateFrom.Script)
//                 animator.SetUpdateFrom(updateFromOnPlay);
//             animator.StartAnimating();
//         }
//         else if (methodToCallOnPlay == AnimateClip.Method.StopAnimating)
//         {
//             if (animator.UpdateFrom == UpdateFrom.Script)
//                 animator.SetUpdateFrom(updateFromOnPlay);
//             animator.StopAnimating();
//         }
//         else
//         {
//             if (animator.UpdateFrom != UpdateFrom.Script)
//                 animator.SetUpdateFrom(UpdateFrom.Script);
//             animator.UpdateAnimations(info.deltaTime);
//         }
//     }
//
//
//     public override void OnBehaviourPause(Playable playable, FrameData info)
//     {
//         if (animator == null) return;
//         Debug.Log("Pause");
// #if UNITY_EDITOR
//         if (!Application.isPlaying) return;
// #endif
//
//         if (methodToCallOnPause == AnimateClip.Method.StartAnimating)
//         {
//             if (animator.UpdateFrom == UpdateFrom.Script)
//                 animator.SetUpdateFrom(updateFromOnPause);
//             animator.StartAnimating();
//         }
//         else if (methodToCallOnPause== AnimateClip.Method.StopAnimating)
//         {
//             if (animator.UpdateFrom == UpdateFrom.Script)
//                 animator.SetUpdateFrom(updateFromOnPause);
//             animator.StopAnimating();
//         }
//         else
//         {
//             if (animator.UpdateFrom != UpdateFrom.Script)
//                 animator.SetUpdateFrom(UpdateFrom.Script);
//             animator.UpdateAnimations(info.deltaTime);
//         }
//     }
//
//     public override void ProcessFrame(Playable playable, FrameData info, object playerData)
//     {
// #if UNITY_EDITOR
//         // if (!Application.isPlaying)
//         // {
//         //     if (animator.UpdateFrom != UpdateFrom.Script)
//         //         animator.SetUpdateFrom(UpdateFrom.Script);
//         //     animator.UpdateAnimations(info.deltaTime);
//         //     return;
//         // }
// #endif
//     }
// }