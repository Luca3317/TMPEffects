using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[DisplayName("TMPEffects Clip/TMPAnimation Clip")]
public class TMPAnimationClip : TMPEffectsClip, ITimelineClipAsset
{
    public GenericAnimation animation;
    [NonSerialized] public TimelineClip Clip = null;
    
    public ClipCaps clipCaps => ClipCaps.Extrapolation;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TMPAnimationBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.Clip = Clip;
        behaviour.animation = animation;
        return playable;
    }
}
