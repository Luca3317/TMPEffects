// #if TIMELINE_INSTALLED
// using System.Collections;
// using System.Collections.Generic;
// using TMPEffects.Components.Animator;
// using UnityEngine;
// using UnityEngine.Playables;
//
// public class AnimateClip : TMPAnimatorClip
// {
//     public enum Method
//     {
//         StartAnimating,
//         StopAnimating,
//         AnimateManually
//     }
//
//     public Method methodToCallOnPlay;
//     public UpdateFrom updateFromOnPlay;
//
//     public Method methodToCallOnPause;
//     public UpdateFrom updateFromOnPause;
//
//     public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
//     {
//         var playable = ScriptPlayable<AnimateBehaviour>.Create(graph);
//         
//         AnimateBehaviour ab = playable.GetBehaviour();
//         ab.methodToCallOnPlay = methodToCallOnPlay;
//         ab.updateFromOnPlay = updateFromOnPlay;
//         ab.methodToCallOnPause = methodToCallOnPause;
//         ab.updateFromOnPause = updateFromOnPause;
//         
//         return playable;
//     }
// }
// #endif
