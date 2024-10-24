using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

internal static class Extensions
{
    public static TimelineClip.ClipExtrapolation ConvertExtrapolation(this AnimationStep.ExtrapolationMode mode)
    {
        return mode switch
        {
            AnimationStep.ExtrapolationMode.None => TimelineClip.ClipExtrapolation.None,
            AnimationStep.ExtrapolationMode.PingPong => TimelineClip.ClipExtrapolation.PingPong,
            AnimationStep.ExtrapolationMode.Continue => TimelineClip.ClipExtrapolation.Continue,
            AnimationStep.ExtrapolationMode.Loop => TimelineClip.ClipExtrapolation.Loop,
            AnimationStep.ExtrapolationMode.Hold => TimelineClip.ClipExtrapolation.Hold,
            _ => throw new System.ArgumentException("Invalid ExtrapolationMode")
        };
    }
    
    public static AnimationStep.ExtrapolationMode ConvertExtrapolation(this TimelineClip.ClipExtrapolation mode)
    {
        return mode switch
        {
            TimelineClip.ClipExtrapolation.None => AnimationStep.ExtrapolationMode.None,
            TimelineClip.ClipExtrapolation.PingPong => AnimationStep.ExtrapolationMode.PingPong,
            TimelineClip.ClipExtrapolation.Continue => AnimationStep.ExtrapolationMode.Continue,
            TimelineClip.ClipExtrapolation.Loop => AnimationStep.ExtrapolationMode.Loop,
            TimelineClip.ClipExtrapolation.Hold => AnimationStep.ExtrapolationMode.Hold,
            _ => throw new System.ArgumentException("Invalid ClipExtrapolation")
        };
    }
}
